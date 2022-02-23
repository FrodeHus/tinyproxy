using System.Net;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Forwarder;

namespace SwaggerProxy;

public class RouteConfigurator
{
    private readonly IHttpForwarder _forwarder;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ProxyConfig _config;
    public RouteConfigurator(IOptions<ProxyConfig> config, IHttpForwarder forwarder, IHttpClientFactory httpClientFactory)
    {
        _forwarder = forwarder;
        _httpClientFactory = httpClientFactory;
        _config = config.Value;
    }

    private List<string> ParseSwagger(UpstreamServer server)
    {
        var client = _httpClientFactory.CreateClient();
        var uriBuilder = new UriBuilder(server.Url);
        uriBuilder.Path += server.SwaggerEndpoint;
        var swaggerJson = client.GetStringAsync(uriBuilder.Uri.ToString()).Result;
        var swagger = JsonDocument.Parse(swaggerJson);
        var paths = swagger.RootElement.GetProperty("paths");
        var endpoints = paths.EnumerateObject().Select(path => path.Name).ToList();
        return endpoints;
    }
    public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false
        });

        var requestOptions = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };
        foreach (var server in _config.UpstreamServers)
        {
            var endpoints = ParseSwagger(server);
            foreach (var endpoint in endpoints)
            {
                routeBuilder.Map(endpoint, async httpContext =>
                {
                    var error = await _forwarder.SendAsync(httpContext, server.Url.ToString(), httpClient, requestOptions);
                    if (error != ForwarderError.None)
                    {
                        var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                        var exception = errorFeature.Exception;
                    }
                });
            }
        }
        
        routeBuilder.Map("/{**catch-all}", async httpContext =>
        {
            var error = await _forwarder.SendAsync(httpContext, "http://localhost:4444", httpClient, requestOptions);
            if (error != ForwarderError.None)
            {
                var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                var exception = errorFeature.Exception;
            }
        });
    }
}