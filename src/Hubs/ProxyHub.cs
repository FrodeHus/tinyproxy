using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace TinyProxy.Hubs;

public class ProxyHub : Hub<IProxyClient>
{
    private readonly IMemoryCache _cache;

    public ProxyHub(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task SendContent(string contentId){
        var content = _cache.Get<string>(contentId);
        return Clients.Caller.ReceiveContent(contentId, content);
    }
}