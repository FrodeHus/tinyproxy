using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class RemoveServerSettings : ConfigurationSettings
{
    [CommandArgument(0, "<SERVER_NAME>")]
    public string? Name { get; set; }
}