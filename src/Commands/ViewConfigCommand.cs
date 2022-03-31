using Spectre.Console.Cli;
using TinyProxy.Models;
using TinyProxy.Models.OutputFormat;

namespace TinyProxy.Commands;

public class ViewConfigCommand : Command<ViewConfigSettings>
{
    public override int Execute(CommandContext context, ViewConfigSettings settings)
    {
        var config = ConfigUtils.ReadOrCreateConfig(settings.ConfigFile);
        var formatter = GetFormatter(settings.Format);
        var formattedOutput = formatter.Output(config);
        Console.WriteLine(formattedOutput);
        return 0;
    }

    private IOutputFormatter GetFormatter(string format)
    {
        return format.ToLowerInvariant() switch
        {
            "json" => new JsonFormatter(),
            _ => throw new ArgumentException(nameof(format))
        };
    }
}