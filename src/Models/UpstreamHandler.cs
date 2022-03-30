namespace TinyProxy.Models;

public class UpstreamHandler
{
    public string RelativePath { get; set; } = "";
    public string Prefix { get; set; } = "";
    public string RemoteServerBaseUrl { get; set; } = "";
    public string? RemoteServer { get; set; }
    public bool Preferred { get; set; }
    public string? Verb { get; set; } 
}