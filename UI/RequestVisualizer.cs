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

        AnsiConsole.MarkupLine(
            $"[[{handler.RemoteServer,-20}]][{color}]{context.Response.StatusCode,5}[/][cyan1]{verb,5}[/] [deepskyblue1]{path,-60}[/] -> [cyan2]{handlerPath,-60}[/] ");
        await DumpRequest(context);
    }

    private static async Task DumpRequest(HttpContext context)
    {
        var requestHeader = new Rule("Request").Alignment(Justify.Left);
        AnsiConsole.Write(requestHeader);
        foreach (var header in context.Request.Headers)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{header.Key,-30}[/]:[{Color.CornflowerBlue}]{header.Value,-10}[/]");
        }

        if (context.Request.HasJsonContentType())
        {
            var payload = await context.Request.ReadFromJsonAsync<object>();
            var formattedJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            AnsiConsole.Markup($"[{Color.Cornsilk1}]Content: [/][{Color.CornflowerBlue}]{formattedJson}[/]");
        }
    }
}