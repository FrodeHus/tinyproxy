using Spectre.Console;
using TinyProxy.UI;

public class ResponseLogging
{
    private readonly RequestDelegate requestDelegate;

    public ResponseLogging(RequestDelegate requestDelegate)
    {
        this.requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        HttpResponse httpResponse = httpContext.Response;
        Stream originalBody = httpResponse.Body;

        try
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                httpResponse.Body = memoryStream;

                await requestDelegate(httpContext);
                await LogResponse(httpContext.Response, httpContext.Request.Path, originalBody, memoryStream);
            }
        }
        finally
        {
            httpContext.Response.Body = originalBody;
        }
    }

    private async Task LogResponse(HttpResponse httpResponse, string requestPath, Stream originalBody, MemoryStream memoryStream)
    {
        httpResponse.Body.Seek(0, SeekOrigin.Begin);
        var responseHeader = new Rule($"[#00ffff]<==[/] [#87d7ff]{requestPath}[/]").Alignment(Justify.Left);
        AnsiConsole.Write(responseHeader);
        foreach (var header in httpResponse.Headers)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{header.Key,-30}[/]:[{Color.CornflowerBlue}]{header.Value,-10}[/]");
        }


        var content = await PayloadVisualizer.Visualize(httpResponse.ContentType, memoryStream);
        if (content.Length > 0)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]Content: [/]");
            Console.WriteLine(content);
        }

        httpResponse.Body.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
    }
}