namespace TinyProxy.Infrastructure;

public class ProxyRoute
{
    public string RelativePath { get; set; } = "";
    public string Prefix { get; set; } = "";
    public string RemoteServerBaseUrl { get; set; } = "";
}