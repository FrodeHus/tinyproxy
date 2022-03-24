using Microsoft.AspNetCore.SignalR;
using TinyProxy.Hubs;
using TinyProxy.Infrastructure;
using TinyProxy.Models;

namespace TinyProxy.Server;

public class HubMessages
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IHubContext<ProxyHub> _hub;

    public HubMessages(RequestDelegate requestDelegate, IHubContext<ProxyHub> hub)
    {
        _requestDelegate = requestDelegate;
        _hub = hub;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        await _requestDelegate(httpContext);
        if (httpContext.Items["handler"] is ProxyRoute handler)
        {
            var request =
                new Request(httpContext.Request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()));
            await _hub.Clients.All.SendAsync("GetTrafficSummary", httpContext.Request.Path, httpContext.Response.StatusCode, handler, request);
        }
    }

}