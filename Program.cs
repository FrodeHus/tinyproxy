using CommandLine;
using SwaggerProxy;

var options = new ProxyOptions();

Parser.Default.ParseArguments<ProxyOptions>(args)
    .WithParsed<ProxyOptions>(o =>
    {
        options = o;
        if (File.Exists(options.ConfigFile)) return;
        Console.WriteLine($"ERROR: {options.ConfigFile} not found");
        Environment.Exit(1);
    });

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

    routeConfigurator.MapEndpoints(endpoints, options.ConfigFile);
});
app.Run();