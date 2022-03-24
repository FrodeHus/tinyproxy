using Microsoft.AspNetCore.SignalR;
using TinyProxy.Infrastructure;

namespace TinyProxy.Hubs;

public class ProxyHub : Hub
{
    public async Task SendMessage(string path, ProxyRoute handler)
    {
        await Clients.All.SendAsync("GetTrafficSummary", path, handler);
    }
}