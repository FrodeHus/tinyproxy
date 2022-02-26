namespace TinyProxy.Infrastructure;

public class ProxyConfig
{
    public List<UpstreamServer> UpstreamServers { get; set; } = new();
}