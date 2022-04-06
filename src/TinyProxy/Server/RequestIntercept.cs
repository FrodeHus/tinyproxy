using Microsoft.Extensions.Caching.Memory;
using TinyProxy.Models;

namespace TinyProxy.Server;

public class RequestIntercept
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IMemoryCache _cache;
    public RequestIntercept(RequestDelegate requestDelegate, IMemoryCache cache)
    {
        _requestDelegate = requestDelegate;
        _cache = cache;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering();
        var httpResponse = httpContext.Response;
        var originalBody = httpResponse.Body;

        try
        {
            await StoreRequest(httpContext);

            await using var memoryStream = new MemoryStream();
            httpResponse.Body = memoryStream;

            await _requestDelegate(httpContext);
            await StoreResponse(httpContext);
            httpResponse.Body.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBody);
        }
        finally
        {
            httpContext.Response.Body = originalBody;
        }
    }

    private async Task StoreRequest(HttpContext httpContext)
    {
        if (httpContext.Request.ContentLength > 0)
        {
            await using var reader = new MemoryStream();
            await httpContext.Request.Body.CopyToAsync(reader);
            var encodedContent = Convert.ToBase64String(reader.ToArray());
            var cacheId = Guid.NewGuid().ToString();
            httpContext.Items.Add("request", cacheId);
            httpContext.Items.Add("requestLength", encodedContent.Length);
            _cache.Set(cacheId, encodedContent);
            httpContext.Request.Body.Position = 0;
        }
    }
    private async Task StoreResponse(HttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        await using var reader = new MemoryStream();
        await httpContext.Response.Body.CopyToAsync(reader);
        var encodedContent = Convert.ToBase64String(reader.ToArray());
        var cacheId = Guid.NewGuid().ToString();
        httpContext.Items.Add("response", cacheId);
        httpContext.Items.Add("responseLength", encodedContent.Length);
        _cache.Set(cacheId, encodedContent);
    }
}