using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Spectre.Console;
using TinyProxy.UI;
using TinyProxy.UI.CommandLine;

public class RequestLogging
{
    private readonly RequestDelegate requestDelegate;
    private readonly IMemoryCache cache;

    public RequestLogging(RequestDelegate requestDelegate, IMemoryCache cache)
    {
        this.requestDelegate = requestDelegate;
        this.cache = cache;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        await LogRequest(httpContext);
        await requestDelegate(httpContext);
    }

    private async Task LogRequest(HttpContext httpContext)
    {
        var httpRequest = httpContext.Request;
        var requestHeader = new Rule($"[#00ffff]==>[/] [#87d7ff]{httpRequest.Path}[/]").Alignment(Justify.Left);
        AnsiConsole.Write(requestHeader);
        foreach (var header in httpRequest.Headers)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{header.Key,-30}[/]:[{Color.CornflowerBlue}]{header.Value,-10}[/]");
        }


        if (httpRequest.ContentLength > 0)
        {
            AnsiConsole.Markup($"[{Color.Cornsilk1}]Content: [/]");
            var cacheId = httpContext.Items["request"] as string;
            if (string.IsNullOrEmpty(cacheId))
            {
                Console.WriteLine("could not find id for current request");
                return;
            }

            var encodedContent = cache.Get<string>(cacheId);
            var decodedBytes = Convert.FromBase64String(encodedContent);
            var content = Encoding.UTF8.GetString(decodedBytes);
            var result = PayloadVisualizer.Visualize(httpRequest.ContentType ?? "*/*", content);
            Console.WriteLine(result);
            httpRequest.Body.Position = 0;
        }
    }
}