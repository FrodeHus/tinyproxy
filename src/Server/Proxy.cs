using Microsoft.AspNetCore.Server.Kestrel.Core;
using Prometheus;
using TinyProxy.Hubs;
using TinyProxy.Infrastructure;
using TinyProxy.UI.CommandLine;
using TinyProxy.UI.Web;
using TinyProxy.Server;

namespace TinyProxy.Server;

public class Proxy
{
    private readonly RequestVisualizer _visualizer;

    public Proxy(RequestVisualizer visualizer)
    {
        _visualizer = visualizer;
    }

    private WebApplication? _app;

    public void Configure(List<ProxyRoute> routes, LogLevel logLevel = LogLevel.Error, bool useWebUI = false,
        int port = 5000, bool verbose = false)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel((_, options) =>
            options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http1AndHttp2));
        builder.Logging.SetMinimumLevel(logLevel);
        builder.Logging.AddFilter((_, _, level) => level == logLevel);
        builder.Services.AddHttpForwarder();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<RouteConfigurator>();
        builder.Services.AddSignalR();
        builder.Services.AddCors();
        builder.Services.AddMemoryCache();
        _app = builder.Build();

        if (_app.Environment.IsDevelopment())
        {
            _app.UseDeveloperExceptionPage();
        }

        if (useWebUI)
        {
            _app.UseWebUI();
        }

        _app.UseMiddleware<HubMessages>();

        if (verbose)
        {
            _app.UseMiddleware<RequestLogging>();
            _app.UseMiddleware<ResponseLogging>();
        }
        _app.UseMiddleware<ResponseRewriter>();
        _app.UseMiddleware<RequestIntercept>();

        _app.UseCors(c =>
        {
            c.WithOrigins($"http://localhost:{port}", "http://localhost:3000").AllowAnyHeader().AllowAnyMethod()
                .AllowCredentials();
        });

        _app.UseRouting();
        _app.UseMetricServer();
        _app.UseEndpoints(endpoints =>
        {
            var routeConfigurator = endpoints.ServiceProvider.GetService<RouteConfigurator>();
            if (routeConfigurator == null)
            {
                throw new ArgumentNullException(nameof(routeConfigurator));
            }

            endpoints.MapHub<ProxyHub>("/tinyproxy/hub");
            routeConfigurator.MapEndpoints(endpoints, routes, _visualizer.DisplayRequest);
            endpoints.MapMetrics();
        });
    }

    public void Start()
    {
        if (_app == null)
        {
            throw new ArgumentNullException(nameof(_app), "proxy is not configured - call Configure()");
        }

        _app.Run();
    }
}