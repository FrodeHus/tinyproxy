using System.Net;
using System.Text.Json;
using SwaggerProxy;
using Yarp.ReverseProxy.Forwarder;

var configFile = "proxyconfig.json";
if (args.Length == 1)
{
    configFile = args[0];
    if (!File.Exists(configFile))
    {
        Console.WriteLine($"{configFile} does not exist.");
        Environment.Exit(1);
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpForwarder();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<RouteConfigurator>();

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

    routeConfigurator.MapEndpoints(endpoints, configFile);
});
app.Run();