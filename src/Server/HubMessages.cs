using System.Text;
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
        httpContext.Request.EnableBuffering();
        HttpResponse httpResponse = httpContext.Response;
        Stream originalBody = httpResponse.Body;
        using var memoryStream = new MemoryStream();
        httpResponse.Body = memoryStream;
        await _requestDelegate(httpContext);
        if (httpContext.Items["handler"] is ProxyRoute handler)
        {
            var request =
                new ProxyData(httpContext.Request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()), ProxyDataType.Request);
            if (httpContext.Request.ContentLength > 0)
            {
                await using var reader = new MemoryStream();
                await httpContext.Request.Body.CopyToAsync(reader);
                request.Content = Convert.ToBase64String(reader.ToArray());
            }
            var response = new ProxyData(httpContext.Response.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()), ProxyDataType.Response);
            if (httpContext.Response.Body.Length > 0)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                await using var reader = new MemoryStream();
                await httpContext.Response.Body.CopyToAsync(reader);
                response.Content = Convert.ToBase64String(reader.ToArray());
            }
            await _hub.Clients.All.SendAsync("ReceiveProxyData", httpContext.Request.Path, httpContext.Response.StatusCode, handler, request, response);
        }
        httpResponse.Body.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
        httpContext.Response.Body = originalBody;
    }

}