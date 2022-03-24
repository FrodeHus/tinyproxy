using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class ProxySettings : ConfigurationSettings
{
    [CommandOption("-v|--verbose")]
    public bool? Verbose { get; set; }

    [CommandOption("-p|--port <PORT>")]
    public int Port { get; set; } = 5000;

    [CommandOption("-u|--ui")]
    public bool? UseWebUI { get; set; }

}