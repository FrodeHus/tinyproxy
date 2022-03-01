using CommandLine;
using Spectre.Console.Cli;
using TinyProxy.Commands;
using TinyProxy.Infrastructure;
using TinyProxy.Server;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<StartProxyCommand>("start");
    config.AddBranch("config", config =>
    {
        config.AddBranch("add", add =>
        {
            add.AddCommand<AddServerCommand>("server");
        });
        config.AddBranch("remove", remove =>
        {
            remove.AddCommand<RemoveServerCommand>("server");
        });
    });
});
app.Run(args);