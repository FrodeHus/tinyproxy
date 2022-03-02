using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class ProxySettings : ConfigurationSettings
{
    [CommandOption("-v|--verbose")]
    public bool? Verbose { get; set; }
    
}