using System.ComponentModel;
using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class AddRouteSettings : ConfigurationSettings
{
    [Description("Name of the upstream server that handles this route")]
    [CommandArgument(0, "<SERVER_NAME>")]
    public string? ServerName { get; set; }
    [Description("The relative path of the route")]
    [CommandArgument(1, "<ROUTE>")]
    public string? Route { get; set; }

    [Description("Which HTTP methods are supported by this route (GET,PUT,POST,etc)")]
    [CommandOption("-m|--methods <VERBS>")]
    public string[] Methods { get; set; } = {"GET","PUT","POST","DELETE","OPTIONS"};
}