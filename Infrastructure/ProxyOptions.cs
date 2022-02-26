using CommandLine;

namespace TinyProxy.Infrastructure;
[Verb("start", HelpText = "Start proxy")]
public class ProxyOptions
{
        [Option('f', "config-file", Required = true, HelpText = "Configuration file to use")]
        public string? ConfigFile { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('c', "catch-all", HelpText = "Any requests that is not handled by any configured remote servers should be sent to here")]
        public string? UnknownEndpointsPort { get; set; }
}