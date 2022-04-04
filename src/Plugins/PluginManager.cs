using System.Reflection;

namespace TinyProxy.Plugins;

public class PluginManager
{
    public PluginManager(string pluginRelativePath)
    {
        var pluginAssembly = LoadPlugin(pluginRelativePath);
        var plugins = ActivatePlugins(pluginAssembly);
    }
    private IEnumerable<ITinyPlugin> ActivatePlugins(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(ITinyPlugin).IsAssignableFrom(type))
            {
                if (Activator.CreateInstance(type) is not ITinyPlugin plugin)
                {
                    continue;
                }
                yield return plugin;
            }
        }
    }
    private Assembly LoadPlugin(string relativePath)
    {
        var root = Path.GetFullPath(typeof(Program).Assembly.Location);
        var pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        var loadContext = new PluginContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }
}