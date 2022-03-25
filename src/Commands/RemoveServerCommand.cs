using Spectre.Console;
using Spectre.Console.Cli;
using TinyProxy.Infrastructure;

namespace TinyProxy.Commands;

public class RemoveServerCommand : Command<RemoveServerSettings>
{
    public override int Execute(CommandContext context, RemoveServerSettings settings)
    {
        var config = ConfigUtils.ReadOrCreateConfig(settings.ConfigFile);
        var existingServer = config.UpstreamServers.Find(u => u.Name == settings.Name);
        if (existingServer == null)
        {
            AnsiConsole.MarkupLine($"[red]Upstream server {settings.Name} was not found in config file {settings.ConfigFile}[/]");
            return 1;
        }
        config.UpstreamServers.Remove(existingServer);
        ConfigUtils.WriteConfig(config, settings.ConfigFile);

        return 0;
    }
}