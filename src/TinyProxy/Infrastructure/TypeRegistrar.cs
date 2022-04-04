using Spectre.Console.Cli;

namespace TinyProxy.Models;

public class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _serviceCollection;

    public TypeRegistrar(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
    public void Register(Type service, Type implementation)
    {
        _serviceCollection.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _serviceCollection.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        _serviceCollection.AddSingleton(service, (provider) => factory);
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_serviceCollection.BuildServiceProvider());
    }
}