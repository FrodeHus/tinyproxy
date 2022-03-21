using Spectre.Console;

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
                await LogResponse(httpContext.Response, originalBody, memoryStream);
            }
        }
        finally
        {
            httpContext.Response.Body = originalBody;
        }
    }

    private async Task LogResponse(HttpResponse httpResponse, Stream originalBody, MemoryStream memoryStream)
    {
        httpResponse.Body.Seek(0, SeekOrigin.Begin);
        var responseHeader = new Rule("Response").Alignment(Justify.Left);
        AnsiConsole.Write(responseHeader);
        foreach (var header in httpResponse.Headers)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]{header.Key,-30}[/]:[{Color.CornflowerBlue}]{header.Value,-10}[/]");
        }


        StreamReader streamReader = new StreamReader(memoryStream);
        string bodyText = await streamReader.ReadToEndAsync();

        if (bodyText.Length > 0)
        {
            AnsiConsole.MarkupLine($"[{Color.Cornsilk1}]Content: [/]");
            Console.WriteLine(bodyText);
        }

        httpResponse.Body.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
    }
}