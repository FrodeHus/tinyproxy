using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class ConfigurationSettings : CommandSettings
{
    [CommandOption("-f|--config-file <CONFIG_FILE>")]
    public string ConfigFile { get; set; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tinyproxyconfig.json");
}