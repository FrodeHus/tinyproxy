using System.Text;
using Spectre.Console;
using TinyProxy.Infrastructure;
using TinyProxy.UI;
using System;
using TinyProxy.UI.CommandLine;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace TinyProxy.Server;
public class ResponseLogging
{
    private readonly RequestDelegate requestDelegate;
    private readonly IMemoryCache cache;

    public ResponseLogging(RequestDelegate requestDelegate, IMemoryCache cache)
    {
        this.requestDelegate = requestDelegate;
        this.cache = cache;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        await requestDelegate(httpContext);
        LogResponse(httpContext);
    }

    private void LogResponse(HttpContext httpContext)
    {
        var requestPath = httpContext.Request.Path;
        var responseHeader = new Rule($"[#00ffff]<==[/] [#87d7ff]{requestPath}[/]").Alignment(Justify.Left);
        AnsiConsole.Write(responseHeader);
        foreach (var header in httpContext.Response.Headers)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{header.Key,-30}[/]:[{Color.CornflowerBlue}]{header.Value,-10}[/]");
        }

        if (cache.Get(httpContext.Items["response"]) is string encodedResponse)
        {

            var content = PayloadVisualizer.Visualize(httpContext.Response.ContentType, encodedResponse);
            if (content.Length > 0)
            {
                AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]Content: [/]");
                Console.WriteLine(content);
            }
        }
    }
}