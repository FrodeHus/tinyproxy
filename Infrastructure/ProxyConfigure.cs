using CommandLine;

namespace TinyProxy.Infrastructure;

[Verb("configure", HelpText = "Configure proxy")]
public class ProxyConfigure
{
    [Option('i', "init", HelpText = "Creates an empty configuration file")]
    public bool Initialize { get; set; }

    [Option('f', "config-file", Required = true, HelpText = "Path to configuration file")]
    public string? ConfigFile { get; set; }
}