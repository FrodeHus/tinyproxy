using Spectre.Console.Cli;
using TinyProxy.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<StartProxyCommand>("start");
    config.AddBranch("config", c =>
    {
        c.AddBranch("add", add =>
        {
            add.AddCommand<AddServerCommand>("server");
        });
        c.AddBranch("remove", remove =>
        {
            remove.AddCommand<RemoveServerCommand>("server");
        });
    });
});
app.Run(args);