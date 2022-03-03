using Spectre.Console;
using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class ViewConfigSettings : ConfigurationSettings
{
    private readonly string[] _validFormats = {"json", "table"};
    [CommandOption("-o|--output <FORMAT>")]
    public string Format { get; set; } = "json";
    
    public override ValidationResult Validate()
    {
        if (_validFormats.Any(f => string.Equals(f, Format, StringComparison.InvariantCultureIgnoreCase)))
        {
            return ValidationResult.Success();
        }
        return ValidationResult.Error($"{Format} is not supported - Supported values are: {string.Join(',', _validFormats)}");
    }
}