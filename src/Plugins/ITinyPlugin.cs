namespace TinyProxy.Plugins;

public interface ITinyPlugin
{
    string Name { get; }
    string Description { get; }
    Task HandleAsync(HttpContext context);
}