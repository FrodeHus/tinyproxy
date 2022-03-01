using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using TinyProxy.Infrastructure;

namespace TinyProxy.OpenAPI;

public class OpenApiParser
{
    private readonly Dictionary<UpstreamServer, OpenApiDocument> _apis = new ();

    public async Task ParseConfigFile(string configFile)
    {
        var config = ConfigUtils.ReadOrCreateConfig(configFile);
        await ParseConfig(config);
    }

    private async Task ParseConfig(ProxyConfig config)
    {
        foreach (var server in config.UpstreamServers)
        {
            await Parse(server);
        }
    }
    private async Task Parse(UpstreamServer server)
    {
        var client = new HttpClient
        {
            BaseAddress = server.Url
        };
        var stream = await client.GetStreamAsync(server.SwaggerEndpoint);
        var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);
        _apis.Add(server, openApiDoc);
    }
    
    public List<ProxyRoute> GetAggregatedProxyRoutes()
    {
        var endpoints = new List<ProxyRoute>();
        foreach (var server in _apis.Keys)
        {
            var normalizedPaths = GetNormalizedPathsForServer(server);
            var routes = normalizedPaths.Select(path => new ProxyRoute
            {
                Prefix = server.Prefix,
                RemoteServer = server.Name,
                RemoteServerBaseUrl = server.Url.ToString(),
                RelativePath = path
            });
            
            endpoints.AddRange(routes.Where(r => endpoints.All(e => e.RelativePath != r.RelativePath)));
        }
        return endpoints;
    }

    private List<string> GetNormalizedPathsForServer(UpstreamServer server)
    {
        var api = _apis[server];
        var endpoints = new List<string>();
        foreach (var (path, pathDefinition) in _apis.Values.SelectMany(p => p.Paths))
        {
            var normalizedPath = NormalizePath(path, pathDefinition);
            
            if (endpoints.Contains(normalizedPath)) continue;
            endpoints.Add(normalizedPath);
        }

        return endpoints;
        
    }

    /// <summary>
    /// Makes sure a path has the same parameter names.
    /// Used to detect duplicate paths with differently named parameters across APIs.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pathItem"></param>
    /// <returns></returns>
    private static string NormalizePath(string path, OpenApiPathItem pathItem)
    {
        var paramIndex = 0;
        var normalizedPath = path;
        foreach (var param in pathItem.Operations.Values.SelectMany(o => o.Parameters.Where(p => p.In == ParameterLocation.Path)))
        {
            normalizedPath = normalizedPath.Replace(param.Name, $"param{paramIndex}");
            paramIndex++;
        }

        return normalizedPath;
    }
}