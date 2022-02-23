using System.Net;
using SwaggerProxy;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpForwarder();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<RouteConfigurator>();
builder.Services.Configure<ProxyConfig>(builder.Configuration.GetSection("ProxyConfig"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    var routeConfigurator = endpoints.ServiceProvider.GetService<RouteConfigurator>();
    if (routeConfigurator == null)
    {
        throw new ArgumentNullException(nameof(routeConfigurator));
    }
    routeConfigurator.MapEndpoints(endpoints);
});
app.Run();
