using Spectre.Console;
using TinyProxy.Infrastructure;

namespace TinyProxy.UI;

public class RequestVisualizer
{
    public void DisplayRequest(HttpContext context, ProxyRoute handler)
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
        AnsiConsole.MarkupLine(
            $"[cyan1]{verb}[/] [deepskyblue1]{path}[/] -> [cyan2]{handler.RemoteServer}:{handler.RemoteServerBaseUrl.TrimEnd('/')}{handler.RelativePath}[/] [{color}]{context.Response.StatusCode}[/]");

    }
}