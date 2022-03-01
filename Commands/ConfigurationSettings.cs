using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class ConfigurationSettings : CommandSettings
{
    [CommandArgument(0, "<CONFIG FILE>")]
    public string ConfigFile { get; set; }
}