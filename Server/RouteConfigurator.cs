using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Spectre.Console;
using TinyProxy.Infrastructure;
using TinyProxy.OpenAPI;
using Yarp.ReverseProxy.Forwarder;

namespace TinyProxy.Server;

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
                RemoteServerBaseUrl = server.Url.ToString(),
                RemoteServer = server.Name
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
            {RelativePath = r, Prefix = server.Prefix, RemoteServer = server.Name, RemoteServerBaseUrl = server.Url.ToString()}).ToList();
    }

    public void MapEndpoints(IEndpointRouteBuilder routeBuilder, List<ProxyRoute> routes)
    {
        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false
        });

        var requestOptions = new ForwarderRequestConfig {ActivityTimeout = TimeSpan.FromSeconds(100)};
        var uniquePaths = routes.GroupBy(kvp => string.Join(kvp.Prefix,kvp.RelativePath), kvp => kvp);
        foreach (var pathKvp in uniquePaths)
        {
            routeBuilder.Map(pathKvp.Key, async httpContext =>
            {
                var allHandlers = pathKvp.ToList();
                var verb = httpContext.Request.Method;
                var handler = allHandlers.FirstOrDefault(
                    h => h.Verb?.ToString() == verb && pathKvp.Key == string.Join(h.Prefix, h.RelativePath));
                if (handler == null)
                {
                    AnsiConsole.MarkupLine($"[red]Handler for {pathKvp.Key} was not found[/]");
                    return;
                }
                if (!string.IsNullOrEmpty(handler.Prefix))
                {
                    httpContext.Request.Path = httpContext.Request.Path.Value?.Replace(handler.Prefix, "");
                }
                ProxyMetrics.IncomingRequest(handler);
                var error = await _forwarder.SendAsync(httpContext, handler.RemoteServerBaseUrl, httpClient,
                    requestOptions);
                if (error != ForwarderError.None)
                {
                    var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                    var exception = errorFeature?.Exception;
                    _logger.LogError(exception, "failed proxying {}", pathKvp.Key);
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