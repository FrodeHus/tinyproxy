using System.Text.Json;
using Spectre.Console;
using TinyProxy.Infrastructure;

namespace TinyProxy.UI;

public class RequestVisualizer
{

    public async void DisplayRequest(HttpContext context, ProxyRoute handler)
    {
        var verb = context.Request.Method;
        var path = context.Request.Path;
        var statusCode = context.Response.StatusCode;
        var color = statusCode switch
        {
            >= 500 => Color.Red1,
            >= 400 => Color.Orange1,
            _ => Color.Green3
        };
        if (path.HasValue && path.Value.Length > 60)
        {
            path = path.Value[..57] + "...";
        }

        var handlerPath = handler.RemoteServerBaseUrl.TrimEnd('/') + handler.RelativePath;
        var labelUpstreamResponse = "Upstream response";
        var labelUpstream = "Upstream";
        var labelUpstreamPath = "Upstream path";
        AnsiConsole.MarkupLine(
            $"[{Color.Cornsilk1}]{labelUpstream,-30}: [/][{Color.CornflowerBlue}]{handler.RemoteServer,-30} [/]");
        AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{labelUpstreamResponse,-30}: [/][{color}]{context.Response.StatusCode,-10}[/]");
        AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{labelUpstreamPath,-30}:[/] [{Color.CornflowerBlue}]{handlerPath,-60}[/]");

    }
}