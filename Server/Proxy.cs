using Prometheus;
using TinyProxy.Infrastructure;

namespace TinyProxy.Server;

public class Proxy
{
    private WebApplication? _app;
    
    public void Configure(string[] args, ProxyOptions options)
    {
        if(string.IsNullOrEmpty(options.ConfigFile)) Environment.Exit(0);
        
        var builder = WebApplication.CreateBuilder(args);
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

            routeConfigurator.MapEndpoints(endpoints, options.ConfigFile);
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