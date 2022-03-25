using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using TinyProxy.Hubs;
using TinyProxy.Infrastructure;
using TinyProxy.Models;

namespace TinyProxy.Server;

public class HubMessages
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IHubContext<ProxyHub> _hub;
    private readonly IMemoryCache _cache;

    public HubMessages(RequestDelegate requestDelegate, IHubContext<ProxyHub> hub, IMemoryCache cache)
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
            var request =
                new ProxyData(httpContext.Request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()), ProxyDataType.Request);
            if (httpContext.Request.ContentLength > 0 && httpContext.Items["request"] is string requestCacheId)
            {
                request.Content = (string)_cache.Get(requestCacheId);
            }
            var response = new ProxyData(httpContext.Response.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()), ProxyDataType.Response);
            if (httpContext.Items["response"] is string responseCacheId)
            {
                response.Content = (string)_cache.Get(responseCacheId);
            }
            await _hub.Clients.All.SendAsync("ReceiveProxyData", httpContext.Request.Path, httpContext.Response.StatusCode, handler, request, response);
        }
    }

}