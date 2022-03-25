namespace TinyProxy.Infrastructure;

public class StaticRoute
{
    public string RelativePath { get; set; } = "";
    public string[] HttpMethods { get; set; } = {"*"};
}