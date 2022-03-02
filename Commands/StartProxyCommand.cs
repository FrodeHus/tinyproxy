using Spectre.Console;
using Spectre.Console.Cli;
using TinyProxy.OpenAPI;
using TinyProxy.Server;

namespace TinyProxy.Commands;

public class StartProxyCommand : AsyncCommand<ProxySettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ProxySettings settings)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrEmpty(settings.ConfigFile)) throw new ArgumentNullException(nameof(settings.ConfigFile));
        var openApiParser = new OpenApiParser();
        await openApiParser.ParseConfigFile(settings.ConfigFile);
        
        var proxyRoutes = openApiParser.GetAggregatedProxyRoutes();
        AnsiConsole.MarkupLine($"Proxying [yellow]{proxyRoutes.Count}[/] routes...");
        var proxy = new Proxy();
        var logLevel = LogLevel.Error;
        if (settings.Verbose.HasValue && settings.Verbose.Value)
        {
            logLevel = LogLevel.Trace;
        }
        proxy.Configure(proxyRoutes, logLevel);
        proxy.Start();
        return 0;
    }
}