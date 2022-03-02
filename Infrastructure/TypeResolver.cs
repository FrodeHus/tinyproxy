using Spectre.Console.Cli;

namespace TinyProxy.Infrastructure;

public class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider;
    }
    
    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            return null;
        }
        
        return _provider.GetService(type);
    }
}