using Spectre.Console;
using Spectre.Console.Cli;
using TinyProxy.Services;
using TinyProxy.Server;

namespace TinyProxy.Commands;

public class StartProxyCommand : AsyncCommand<ProxySettings>
{
    private readonly Proxy _proxy;

    public StartProxyCommand(Proxy proxy)
    {
        _proxy = proxy;
    }
    public override async Task<int> ExecuteAsync(CommandContext context, ProxySettings settings)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrEmpty(settings.ConfigFile)) throw new ArgumentNullException(nameof(settings.ConfigFile));
        var openApiParser = new RouteService();
        await openApiParser.ParseConfigFile(settings.ConfigFile);
        AnsiConsole.MarkupLine($"Loading routes...");
        var proxyRoutes = openApiParser.GetAggregatedProxyRoutes();
        AnsiConsole.MarkupLine($"Proxying [yellow]{proxyRoutes.Count}[/] routes...");
        var logLevel = LogLevel.Error;
        if (settings.Verbose.HasValue && settings.Verbose.Value)
        {
            logLevel = LogLevel.Trace;
        }
        _proxy.Configure(proxyRoutes, logLevel, settings.Port);
        _proxy.Start();
        return 0;
    }
}