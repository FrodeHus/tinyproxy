using System.Reflection;
using System.Runtime.Loader;

namespace TinyProxy.Plugins;

public class PluginContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    public PluginContext(string pluginPath)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (string.IsNullOrEmpty(assemblyPath))
        {
            return null;
        }
        return LoadFromAssemblyPath(assemblyPath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (string.IsNullOrEmpty(libraryPath))
        {
            return IntPtr.Zero;
        }
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }
}