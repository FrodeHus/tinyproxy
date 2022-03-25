using System.ComponentModel;
using Spectre.Console.Cli;

namespace TinyProxy.Commands;

public class AddServerSettings : ConfigurationSettings
{
    [CommandArgument(0, "<SERVER_NAME>")]
    public string? Name { get; set; }

    [CommandArgument(1, "<BASE_URL>")]
    public string? BaseUrl { get; set; }

    [Description("Relative path to Swagger Definition (ie. swagger/v1/swagger.json)")]
    [CommandOption("-s|--swagger <PATH_TO_SWAGGER>")]
    public string SwaggerEndpoint { get; set; } = "";

    [Description("Prefix all routes belonging to this server with <PREFIX>")]
    [CommandOption("-p|--prefix <PREFIX>")]
    public string Prefix { get; set; } = "";

    [Description("Any paths defined on this server will be preferred over any duplicates found in non-preferred servers")]
    [CommandOption("-x|--preferred")]
    public bool? IsPreferred { get; set; }
    
}