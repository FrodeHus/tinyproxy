namespace TinyProxy.Plugins;

public interface ITinyPlugin
{
    string Name { get; }
    string Description { get; }
    Task HandleRequestAsync(Stream requestBody);
    Task HandleResponseAsync(Stream responseBody);
}
