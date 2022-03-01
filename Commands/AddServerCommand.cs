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
        
        var existingConfig = File.ReadAllText(settings.ConfigFile);
        var proxyConfig = JsonSerializer.Deserialize<ProxyConfig>(existingConfig);
        if (proxyConfig == null)
        {
            throw new Exception($"failed loading config from {settings.ConfigFile}");
        }

        var newUpstream = new UpstreamServer
        {
            Name = settings.Name,
            Url = new Uri(settings.BaseUrl),
            SwaggerEndpoint = settings.SwaggerEndpoint,
            Prefix = settings.Prefix
        };

        proxyConfig.UpstreamServers.Add(newUpstream);
        var newConfig = JsonSerializer.Serialize(proxyConfig, new JsonSerializerOptions{WriteIndented = true});
        File.WriteAllText(settings.ConfigFile, newConfig);
        return 0;
    }
}