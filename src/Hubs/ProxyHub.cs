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

    public Task<string> GetContent(string contentId){
        if(string.IsNullOrEmpty(contentId)){
            return Task.FromResult("contentId was null");
        }
        var content = _cache.Get<string>(contentId);
        return Task.FromResult(content);
    }
}