using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using TinyProxy.Hubs;
using TinyProxy.Models;

namespace TinyProxy.Server;

public class HubMessages
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IHubContext<ProxyHub, IProxyClient> _hub;
    private int _requestIndex = 0;

    public HubMessages(RequestDelegate requestDelegate, IHubContext<ProxyHub, IProxyClient> hub)
    {
        _requestDelegate = requestDelegate;
        _hub = hub;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        await _requestDelegate(httpContext);

        var request = new Request
        {
            Id = _requestIndex++,
            Path = httpContext.Request.Path,
            Method = httpContext.Request.Method.ToUpperInvariant(),
            TraceIdentifier = httpContext.TraceIdentifier,
            RequestHeaders = httpContext.Request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
            ResponseHeaders = httpContext.Response.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
            RequestContentId = httpContext.Items["request"] is string requestCacheId ? requestCacheId : null,
            RequestContentLength = httpContext.Request.ContentLength,
            ResponseContentId = httpContext.Items["response"] is string responseCacheId ? responseCacheId : null,
            ResponseContentLength = httpContext.Response.ContentLength,
            StatusCode = httpContext.Response.StatusCode
        };
        if (httpContext.Items["handler"] is UpstreamHandler handler)
        {
            request.Handler = handler;
        }
        await _hub.Clients.All.ReceiveRequest(request);
    }
}
