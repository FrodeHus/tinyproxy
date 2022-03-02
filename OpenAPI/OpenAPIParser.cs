using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using TinyProxy.Infrastructure;

namespace TinyProxy.OpenAPI;

public class OpenApiParser
{
    private readonly Dictionary<UpstreamServer, OpenApiDocument> _apis = new ();
    private readonly List<ProxyRoute> _allRoutes = new();
    public async Task ParseConfigFile(string configFile)
    {
        var config = ConfigUtils.ReadOrCreateConfig(configFile);
        await ParseConfig(config);
        _allRoutes.AddRange(GetAggregatedProxyRoutes());
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
            var routes = normalizedPaths.SelectMany(kvp =>
            {
                return kvp.Value.Select(path =>
                    new ProxyRoute
                    {
                        Verb = kvp.Key,
                        Prefix = server.Prefix,
                        RemoteServerBaseUrl = server.Url.ToString(),
                        RelativePath = path,
                        Preferred = server.Preferred,
                        RemoteServer = server.Name
                    });
            });
            endpoints.AddRange(routes);
        }
        
        return endpoints;
    }

    public ProxyRoute FindRoute(string path, HttpMethod method, string prefix = "")
    {
        var route = _allRoutes.Where(r => r.RelativePath == path && r.Verb == method && r.Prefix == prefix).ToList();
        if (route.Count > 1)
        {
            throw new ArgumentException(
                $"failed to find route for {method} {path} - too many matches: {route.Count}");
        }

        return route.FirstOrDefault() ?? new ProxyRoute();
    }

    private Dictionary<HttpMethod, List<string>> GetNormalizedPathsForServer(UpstreamServer server)
    {
        var paths = new Dictionary<HttpMethod, List<string>>();
        var apiDoc = _apis[server];
        foreach (var (path, pathDefinition) in apiDoc.Paths)
        {
            foreach (var operation in pathDefinition.Operations)
            {
                var verb = operation.Key switch
                {
                    OperationType.Get => HttpMethod.Get,
                    OperationType.Put => HttpMethod.Put,
                    OperationType.Post => HttpMethod.Post,
                    OperationType.Delete => HttpMethod.Delete,
                    OperationType.Options => HttpMethod.Options,
                    OperationType.Head => HttpMethod.Head,
                    OperationType.Patch => HttpMethod.Patch,
                    OperationType.Trace => HttpMethod.Trace,
                    _ => throw new ArgumentOutOfRangeException(operation.Key.ToString())
                };
                var normalizedPath = NormalizePath(path, operation.Value);
                if (!paths.ContainsKey(verb))
                {
                    paths.Add(verb, new List<string>());
                }
                
                paths[verb].Add(normalizedPath);
            }
        }

        return paths;
        
    }

    /// <summary>
    /// Makes sure a path has the same parameter names.
    /// Used to detect duplicate paths with differently named parameters across APIs.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pathItem"></param>
    /// <returns></returns>
    private static string NormalizePath(string path, OpenApiOperation operation)
    {
        var paramIndex = 0;
        var normalizedPath = path;
        foreach (var param in operation.Parameters.Where(p => p.In == ParameterLocation.Path))
        {
            normalizedPath = normalizedPath.Replace(param.Name, $"param{paramIndex}");
            paramIndex++;
        }

        return normalizedPath;
    }
}