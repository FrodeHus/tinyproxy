using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using TinyProxy.Models;
namespace TinyProxy.Server;

public class ResponseRewriter
{
    private readonly RequestDelegate requestDelegate;
    private readonly IMemoryCache cache;

    public ResponseRewriter(RequestDelegate requestDelegate, IMemoryCache cache)
    {
        this.requestDelegate = requestDelegate;
        this.cache = cache;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        HttpResponse httpResponse = httpContext.Response;
        Stream originalBody = httpResponse.Body;
        using var memoryStream = new MemoryStream();
        httpResponse.Body = memoryStream;
        await requestDelegate(httpContext);
        var cacheId = httpContext.Items["response"];
        if (cache.Get(cacheId) is not string encodedContent)
        {
            Console.WriteLine("could not find response");
            return;
        }
        var contentBytes = Convert.FromBase64String(encodedContent);
        var content = Encoding.UTF8.GetString(contentBytes);
        if (httpContext.Items["handler"] is UpstreamHandler handler && !string.IsNullOrEmpty(handler.Prefix) && Regex.IsMatch(content, @"(href|src)=[""'](\/)[^\w""]*", RegexOptions.IgnoreCase))
        {
            content = Regex.Replace(content, @"(href|src)=[""'](\/)[^\w""]*", @$"href=""{handler.Prefix}/");
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            contentBytes = Encoding.UTF8.GetBytes(content);
            await httpContext.Response.Body.WriteAsync(contentBytes);
        }
        httpResponse.Body.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
        httpResponse.Body = originalBody;
    }
}