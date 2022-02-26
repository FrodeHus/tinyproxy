using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Forwarder;

namespace TinyProxy;

public class RouteConfigurator
{
    private readonly IHttpForwarder _forwarder;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RouteConfigurator> _logger;

    public RouteConfigurator(IHttpForwarder forwarder,
        IHttpClientFactory httpClientFactory, ILogger<RouteConfigurator> logger)
    {
        _forwarder = forwarder;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private ProxyConfig? LoadConfig(string configFile)
    {
        var configJson = File.ReadAllText(configFile);
        return JsonSerializer.Deserialize<ProxyConfig>(configJson);
    }

    private List<string> ParseSwagger(UpstreamServer server)
    {
        var parameterMatcher = new Regex(@"{(?<paramName>[\w_]+)}");
        var client = _httpClientFactory.CreateClient();
        var uriBuilder = new UriBuilder(server.Url);
        uriBuilder.Path += server.SwaggerEndpoint;
        var swaggerJson = client.GetStringAsync(uriBuilder.Uri.ToString()).Result;
        var swagger = JsonDocument.Parse(swaggerJson);
        var paths = swagger.RootElement.GetProperty("paths");
        var endpoints = new List<string>();
        foreach (var path in paths.EnumerateObject())
        {
            var endpoint = path.Name;
            var parameters = parameterMatcher.Matches(endpoint);
            var paramIndex = 0;
            foreach (Match param in parameters)
            {
                endpoint = endpoint.Replace(param.Value, $"{{param{paramIndex}}}");
                paramIndex++;
            }

            if (endpoints.Contains(endpoint)) continue;
            endpoints.Add(endpoint);
        }

        return endpoints;
    }

    private IEnumerable<ProxyRoute> GetSwaggerRoutes(UpstreamServer server)
    {
        if (string.IsNullOrEmpty(server.SwaggerEndpoint))
        {
            return new List<ProxyRoute>();
        }

        try
        {
            var endpoints = ParseSwagger(server);
            return endpoints.Select(e => new ProxyRoute
            {
                Prefix = server.Prefix,
                RelativePath = e,
                RemoteServerBaseUrl = server.Url.ToString()
            }).ToList();
        }
        catch (AggregateException ae) when (ae.InnerExceptions.Any(ie => ie.GetType() == typeof(HttpRequestException)))
        {
            _logger.LogError("failed to retrieve swagger definition from {}{}", server.Url.ToString(),
                server.SwaggerEndpoint);
            return new List<ProxyRoute>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error when trying to read swagger definition from {}{}",
                server.Url.ToString(), server.SwaggerEndpoint);
            return new List<ProxyRoute>();
        }
    }

    private IEnumerable<ProxyRoute> GetStaticRoutes(UpstreamServer server)
    {
        return server.Routes.Select(r => new ProxyRoute
            {RelativePath = r, Prefix = server.Prefix, RemoteServerBaseUrl = server.Url.ToString()}).ToList();
    }

    public void MapEndpoints(IEndpointRouteBuilder routeBuilder, string configFile)
    {
        var config = LoadConfig(configFile);
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }
        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false
        });

        var requestOptions = new ForwarderRequestConfig {ActivityTimeout = TimeSpan.FromSeconds(100)};
        var aggregatedRoutes = new List<ProxyRoute>();
        foreach (var server in config.UpstreamServers)
        {
            var swaggerRoutes = GetSwaggerRoutes(server);
            var staticRoutes = GetStaticRoutes(server);
            var serverRoutes = staticRoutes.Concat(swaggerRoutes).ToList();
            _logger.LogInformation("Got {} endpoints for {} [{}]", serverRoutes.Count, server.Name,
                server.Url.ToString());

            foreach (var route in serverRoutes)
            {
                var existingRoute = aggregatedRoutes.Find(r =>
                    r.RelativePath == route.RelativePath && r.Prefix == route.Prefix);
                if (existingRoute != null)
                {
                    if (!server.Preferred)
                        continue;
                    aggregatedRoutes.Remove(existingRoute);
                }

                aggregatedRoutes.Add(route);
            }
        }

        _logger.LogInformation("Proxying {} endpoints", aggregatedRoutes.Count);
        foreach (var route in aggregatedRoutes)
        {
            var endpoint = route.Prefix + route.RelativePath;
            routeBuilder.Map(endpoint, async httpContext =>
            {
                if (!string.IsNullOrEmpty(route.Prefix))
                {
                    httpContext.Request.Path = httpContext.Request.Path.Value?.Replace(route.Prefix, "");
                }

                var error = await _forwarder.SendAsync(httpContext, route.RemoteServerBaseUrl, httpClient,
                    requestOptions);
                if (error != ForwarderError.None)
                {
                    var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                    var exception = errorFeature?.Exception;
                    _logger.LogError(exception, "failed proxying {}", endpoint);
                }
            });
        }

        routeBuilder.Map("/{**catch-all}", async httpContext =>
        {
            var error = await _forwarder.SendAsync(httpContext, "http://localhost:4444", httpClient, requestOptions);
            if (error != ForwarderError.None)
            {
                var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                var exception = errorFeature?.Exception;
                _logger.LogError(exception, "failed proxying {}", httpContext.Request.Query);
            }
        });
    }
}