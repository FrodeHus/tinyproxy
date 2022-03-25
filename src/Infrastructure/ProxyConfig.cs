using System.Text.Json;
using Yarp.ReverseProxy.Configuration;

namespace TinyProxy.Infrastructure;

public class ProxyConfig
{
    public List<UpstreamServer> UpstreamServers { get; set; } = new();
    public static void Initialize(ProxyConfigure config)
    {
        if (string.IsNullOrEmpty(config.ConfigFile))
        {
            throw new ArgumentNullException(nameof(config.ConfigFile));
        }

        if (string.IsNullOrEmpty(config.Url))
        {
            throw new ArgumentNullException(nameof(config.Url));
        }

        if (string.IsNullOrEmpty(config.Name))
        {
            throw new ArgumentNullException(nameof(config.Name));
        }

        var proxyConfig = new ProxyConfig
        {
            UpstreamServers = new List<UpstreamServer>
            {
                new ()
                {
                    Url = new Uri(config.Url),
                    SwaggerEndpoint = config.SwaggerEndpoint ?? "",
                    Prefix = config.Prefix ?? "",
                    Name = config.Name
                }
            }
        };
        
        var newConfig = JsonSerializer.Serialize(proxyConfig, new JsonSerializerOptions{WriteIndented = true});
        File.WriteAllText(config.ConfigFile, newConfig);
    }
}