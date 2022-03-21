using System.Text;
using Spectre.Console;

public class RequestLogging
{
    private readonly RequestDelegate requestDelegate;

    public RequestLogging(RequestDelegate requestDelegate)
    {
        this.requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering();
        await LogRequest(httpContext.Request);
        await requestDelegate(httpContext);
    }

    private async Task LogRequest(HttpRequest httpRequest)
    {
        var requestHeader = new Rule(httpRequest.Path).Alignment(Justify.Left);
        AnsiConsole.Write(requestHeader);
        foreach (var header in httpRequest.Headers)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{header.Key,-30}[/]:[{Color.CornflowerBlue}]{header.Value,-10}[/]");
        }


        if (httpRequest.ContentLength > 0)
        {
            using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, true, 1024, true);
            var content = await reader.ReadToEndAsync();
            AnsiConsole.Markup($"[{Color.Cornsilk1}]Content: [/]");
            Console.WriteLine(content);
            httpRequest.Body.Position = 0;
        }
    }
}