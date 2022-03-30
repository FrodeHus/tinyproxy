using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using TinyProxy.Hubs;
using TinyProxy.Infrastructure;

namespace TinyProxy.Server;

public class HubMessages
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IHubContext<ProxyHub, IProxyClient> _hub;
    private readonly IMemoryCache _cache;
    private int _requestIndex = 0;

    public HubMessages(RequestDelegate requestDelegate, IHubContext<ProxyHub, IProxyClient> hub, IMemoryCache cache)
    {
        _requestDelegate = requestDelegate;
        _hub = hub;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        await _requestDelegate(httpContext);
        if (httpContext.Items["handler"] is ProxyRoute handler)
        {
            var request = new Request{
                Id = _requestIndex++,
                TraceIdentifier = httpContext.TraceIdentifier,
                RequestHeaders = httpContext.Request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                ResponseHeaders = httpContext.Response.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                RequestContentId = httpContext.Items["request"] is string requestCacheId ? requestCacheId : null,
                RequestContentLength = httpContext.Request.ContentLength,
                ResponseContentId = httpContext.Items["response"] is string responseCacheId ? responseCacheId : null,
                ResponseContentLength = httpContext.Response.ContentLength,
                Handler = handler,
                StatusCode = httpContext.Response.StatusCode
            };
            await _hub.Clients.All.ReceiveRequest(request);
        }
    }
}