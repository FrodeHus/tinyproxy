using Spectre.Console;
using Spectre.Console.Cli;
using TinyProxy.Commands;
using TinyProxy.Models;
using TinyProxy.Server;
using TinyProxy.UI;
using TinyProxy.UI.CommandLine;

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
        c.AddCommand<ViewConfigCommand>("view");
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