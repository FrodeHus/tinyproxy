using Prometheus;
using TinyProxy.Infrastructure;
using TinyProxy.UI;

namespace TinyProxy.Server;

public class Proxy
{
    private readonly RequestVisualizer _visualizer;

    public Proxy(RequestVisualizer visualizer)
    {
        _visualizer = visualizer;
    }
    private WebApplication? _app;
    
    public void Configure(List<ProxyRoute> routes, LogLevel logLevel = LogLevel.Error)
    {
        
        var builder = WebApplication.CreateBuilder();
        builder.Logging.SetMinimumLevel(logLevel);
        builder.Logging.AddFilter((provider, category, level) => level == logLevel);
        builder.Services.AddHttpForwarder();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<RouteConfigurator>();

        _app = builder.Build();

        if (_app.Environment.IsDevelopment())
        {
            _app.UseDeveloperExceptionPage();
        }

        _app.UseRouting();
        _app.UseMetricServer();
        _app.UseEndpoints(endpoints =>
        {
            var routeConfigurator = endpoints.ServiceProvider.GetService<RouteConfigurator>();
            if (routeConfigurator == null)
            {
                throw new ArgumentNullException(nameof(routeConfigurator));
            }

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