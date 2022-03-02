using Spectre.Console;
using Spectre.Console.Cli;
using TinyProxy.Commands;
using TinyProxy.Infrastructure;
using TinyProxy.Server;
using TinyProxy.UI;

var services = new ServiceCollection();
services.AddSingleton<Proxy>();
services.AddSingleton<RequestVisualizer>();

var serviceProvider = new TypeRegistrar(services);
var app = new CommandApp(serviceProvider);
app.Configure(config =>
{
    config.AddCommand<StartProxyCommand>("start");
    config.AddBranch("config", c =>
    {
        c.AddBranch("add", add =>
        {
            add.AddCommand<AddServerCommand>("server");
            add.AddCommand<AddRouteCommand>("route");
        });
        c.AddBranch("remove", remove =>
        {
            remove.AddCommand<RemoveServerCommand>("server");
        });
    });
});
AnsiConsole.Write(
    new FigletText("TinyProxy")
        .LeftAligned()
        .Color(Color.Cyan3));
app.Run(args);