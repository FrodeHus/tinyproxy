namespace TinyProxy.Models;

public class StaticRoute
{
    public string RelativePath { get; set; } = "";
    public string[] HttpMethods { get; set; } = {"*"};
}