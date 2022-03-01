using System.Text.Json;
using Spectre.Console.Cli;
using TinyProxy.Infrastructure;

namespace TinyProxy.Commands;

public class AddServerCommand : Command<AddServerSettings>
{
    public override int Execute(CommandContext context, AddServerSettings settings)
    {
        if (string.IsNullOrEmpty(settings.Name)) throw new ArgumentNullException(nameof(settings.Name));
        if (string.IsNullOrEmpty(settings.BaseUrl)) throw new ArgumentNullException(nameof(settings.BaseUrl));

        var proxyConfig = ConfigUtils.ReadOrCreateConfig(settings.ConfigFile);

        var newUpstream = new UpstreamServer
        {
            Name = settings.Name,
            Url = new Uri(settings.BaseUrl),
            SwaggerEndpoint = settings.SwaggerEndpoint,
            Prefix = settings.Prefix
        };

        proxyConfig.UpstreamServers.Add(newUpstream);
        ConfigUtils.WriteConfig(proxyConfig, settings.ConfigFile);
        return 0;
    }
}