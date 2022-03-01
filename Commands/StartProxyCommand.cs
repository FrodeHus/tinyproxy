using Spectre.Console;
using Spectre.Console.Cli;
using TinyProxy.Server;

namespace TinyProxy.Commands;

public class StartProxyCommand : Command<ProxySettings>
{
    public override int Execute(CommandContext context, ProxySettings settings)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        var proxy = new Proxy();
        proxy.Configure(settings);
        proxy.Start();
        return 0;
    }
}