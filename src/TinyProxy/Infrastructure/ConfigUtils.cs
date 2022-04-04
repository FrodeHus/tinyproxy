using System.Text.Json;
using TinyProxy.Server;

namespace TinyProxy.Models;

public static class ConfigUtils
{
    public static ProxyConfig ReadOrCreateConfig(string configFile)
    {
        if (!File.Exists(configFile))
        {
            return new ProxyConfig();
        }
        var existingConfig = File.ReadAllText(configFile);
        var proxyConfig = JsonSerializer.Deserialize<ProxyConfig>(existingConfig);
        if (proxyConfig == null)
        {
            throw new Exception($"failed loading config from {configFile}");
        }

        return proxyConfig;
    }

    public static void WriteConfig(ProxyConfig config, string configFile)
    {
        var newConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions{WriteIndented = true});
        File.WriteAllText(configFile, newConfig);
    }
}