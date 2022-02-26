using System.Text.Json;

namespace TinyProxy.Infrastructure;

public class ProxyConfig
{
    public List<UpstreamServer> UpstreamServers { get; set; } = new();

    public static void Initialize(string configFile)
    {
        var emptyConfig = JsonSerializer.Serialize(new ProxyConfig(), new JsonSerializerOptions{WriteIndented = true});
        File.WriteAllText(configFile, emptyConfig);
    }
}