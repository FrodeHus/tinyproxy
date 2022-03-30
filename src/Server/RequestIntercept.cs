using Microsoft.Extensions.Caching.Memory;
using TinyProxy.Models;

namespace TinyProxy.Server;

public class RequestIntercept
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IMemoryCache _cache;
    public RequestIntercept(RequestDelegate reguestDelegate, IMemoryCache cache)
    {
        _requestDelegate = reguestDelegate;
        _cache = cache;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering();
        HttpResponse httpResponse = httpContext.Response;
        Stream originalBody = httpResponse.Body;

        try
        {
            await StoreRequest(httpContext);

            using var memoryStream = new MemoryStream();
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
        _cache.Set(cacheId, encodedContent, TimeSpan.FromSeconds(30));
    }
}