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
        if (path.HasValue && path.Value.Length > 60)
        {
            path = path.Value[..57] + "...";
        }

        var handlerPath = handler.RemoteServerBaseUrl.TrimEnd('/') + handler.RelativePath;
        
        AnsiConsole.MarkupLine(
            $"[[{handler.RemoteServer,-20}]][cyan1]{verb,5}[/] [deepskyblue1]{path,-60}[/] -> [cyan2]{handlerPath,-60}[/] [{color}]{context.Response.StatusCode,5}[/]");

    }
}