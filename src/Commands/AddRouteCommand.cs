using Spectre.Console;
using Spectre.Console.Cli;
using TinyProxy.Infrastructure;

namespace TinyProxy.Commands;

public class AddRouteCommand : Command<AddRouteSettings>
{
    public override int Execute(CommandContext context, AddRouteSettings settings)
    {
        if (string.IsNullOrEmpty(settings.ServerName)) throw new ArgumentNullException(nameof(settings.ServerName));
        if (string.IsNullOrEmpty(settings.Route)) throw new ArgumentNullException(nameof(settings.Route));

        var proxyConfig = ConfigUtils.ReadOrCreateConfig(settings.ConfigFile);
        var server = proxyConfig.UpstreamServers.Find(s => s.Name == settings.ServerName);
        if (server == null)
        {
            AnsiConsole.MarkupLine($"[red]Could not find server {settings.ServerName} in config - please add and retry[/]");
            return 1;
        }
        
        server.Routes.Add(new StaticRoute
        {
            HttpMethods = settings.Methods,
            RelativePath = settings.Route
        });
        
        ConfigUtils.WriteConfig(proxyConfig, settings.ConfigFile);
        return 0;
    }
}