using CommandLine;

namespace TinyProxy.Infrastructure;

[Verb("configure", HelpText = "Configure proxy")]
public class ProxyConfigure
{
    [Option('i', "init", HelpText = "Creates an empty configuration file")]
    public bool Initialize { get; set; }

    [Option('u', "url", Required = true, HelpText = "Base url of remote server")]
    public string? Url { get; set; }

    [Option('s', "swagger", HelpText = "Relative path to Swagger definition")]
    public string? SwaggerEndpoint { get; set; }

    [Option('p',"prefix", HelpText = "Optional path prefix for the remote endpoints")]
    public string? Prefix { get; set; }

    [Option('p', "preferred", HelpText = "Set this remote as a preferred server (will override matching routes in other servers)")]
    public bool Preferred { get; set; }
    [Option('n',"name", Required = true, HelpText = "Descriptive name of the remote server")]
    public string? Name { get; set; }
    [Option('f', "config-file", Required = true, HelpText = "Path to configuration file")]
    public string? ConfigFile { get; set; }
}