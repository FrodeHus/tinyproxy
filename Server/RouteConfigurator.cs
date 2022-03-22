using System.Net;
using Spectre.Console;
using TinyProxy.Infrastructure;
using Yarp.ReverseProxy.Forwarder;

namespace TinyProxy.Server;

public class RouteConfigurator
{
    private readonly IHttpForwarder _forwarder;
    private readonly ILogger<RouteConfigurator> _logger;

    public RouteConfigurator(IHttpForwarder forwarder, ILogger<RouteConfigurator> logger)
    {
        _forwarder = forwarder;
        _logger = logger;
    }

    public void MapEndpoints(IEndpointRouteBuilder routeBuilder, List<ProxyRoute> routes, Action<HttpContext, ProxyRoute>? requestHandler = null)
    {
        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.All,
            UseCookies = false
        });

        var requestOptions = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };
        var uniquePaths = routes.GroupBy(GetPrefixedPath, kvp => kvp,
            (path, handlers) => new { Path = path, Handlers = handlers });
        foreach (var item in uniquePaths)
        {
            routeBuilder.Map(item.Path, async httpContext =>
            {
                var verb = httpContext.Request.Method;
                if (!TryFindHandler(item.Handlers, item.Path, verb, out var handler))
                {
                    return;
                }

                if (!string.IsNullOrEmpty(handler.Prefix))
                {
                    var requestPath = httpContext.Request.Path.Value;
                    httpContext.Request.Path = requestPath?[(requestPath.IndexOf(handler.Prefix, StringComparison.Ordinal) + handler.Prefix.Length)..];
                    ;
                }

                ProxyMetrics.IncomingRequest(handler);
                var error = await _forwarder.SendAsync(httpContext, handler.RemoteServerBaseUrl, httpClient,
                    requestOptions);
                requestHandler?.Invoke(httpContext, handler);
                if (error != ForwarderError.None)
                {
                    var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                    var exception = errorFeature?.Exception;
                    _logger.LogError(exception, "failed proxying {}", item.Path);
                }
            });
        }
        var catchAllConfigured = routeBuilder.DataSources.Any(d => d.Endpoints.Any(e => e.DisplayName == "/{**catch-all}"));
        if (catchAllConfigured)
        {
            return;
        }

        routeBuilder.Map("/{**catch-all}", httpContext =>
        {
            var verb = httpContext.Request.Method;
            var path = httpContext.Request.Path;
            AnsiConsole.MarkupLine($"[yellow]WARN No route defined for [/][red]{verb} {path}[/]");
            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        });
    }

    private static string GetPrefixedPath(ProxyRoute route)
    {
        return string.Join("/", route.Prefix.TrimEnd('/'), route.RelativePath.TrimStart('/'));
    }

    private static bool TryFindHandler(IEnumerable<ProxyRoute> allHandlers, string path, string verb,
        out ProxyRoute handler)
    {
        var handlers = allHandlers.Where(
            h => (h.Verb?.ToString() == verb || verb == HttpMethod.Options.ToString()) &&
                 path == GetPrefixedPath(h)).ToList();
        if (!handlers.Any())
        {
            AnsiConsole.MarkupLine($"[red]Handler for {path} was not found[/]");
            handler = new ProxyRoute();
            return false;
        }

        if (handlers.Count > 1)
        {
            if (handlers.Any(r => r.Preferred))
            {
                handler = handlers.First(r => r.Preferred);
                AnsiConsole.MarkupLine(
                    $"[cyan1]INFO[/] [yellow]Too many handlers found for {verb} {path} - using preferred server {handler.RemoteServer}[/]");
                return true;
            }

            AnsiConsole.MarkupLine(
                $"[cyan1]INFO[/] Too many handlers exist for [yellow]{verb} {path}[/] - using first one found");
        }

        handler = handlers.First();
        return true;
    }
}