using Prometheus;
using TinyProxy.Infrastructure;

namespace TinyProxy.Server;

public class Proxy
{
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

            routeConfigurator.MapEndpoints(endpoints, routes);
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