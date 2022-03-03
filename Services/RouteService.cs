using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Spectre.Console;
using TinyProxy.Infrastructure;

namespace TinyProxy.Services;

public class RouteService
{
    private readonly Dictionary<UpstreamServer, OpenApiDocument?> _apis = new();

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
        if (string.IsNullOrEmpty(server.SwaggerEndpoint))
        {
            _apis.Add(server, default);
            return;
        }

        var client = new HttpClient
        {
            BaseAddress = server.Url
        };
        var stream = await client.GetStreamAsync(server.SwaggerEndpoint);
        var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);
        _apis.Add(server, openApiDoc);
    }

    private IEnumerable<ProxyRoute> GetStaticRoutes(UpstreamServer server)
    {
        return server.Routes.SelectMany(r =>
        {
            return r.HttpMethods.Select(method => new ProxyRoute
            {
                RelativePath = r.RelativePath, 
                Prefix = server.Prefix, 
                RemoteServer = server.Name,
                RemoteServerBaseUrl = server.Url.ToString(), 
                Verb = ConvertToHttpMethod(method)
            });
        }).ToList();
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
            }).ToList();
            endpoints.AddRange(routes);
            var staticRoutes = GetStaticRoutes(server).ToList();
            endpoints.AddRange(staticRoutes);
            AnsiConsole.MarkupLine($"{server.Name} loaded");
            AnsiConsole.MarkupLine($"\tOpenAPI: [{Color.Orange1}]{routes.Count,5}[/]");
            AnsiConsole.MarkupLine($"\tStatic : [{Color.Orange1}]{staticRoutes.Count,5}[/]");
        }

        return endpoints;
    }

    private HttpMethod ConvertToHttpMethod(string verb)
    {
        return verb.ToUpper() switch
        {
            "GET" => HttpMethod.Get,
            "PUT" => HttpMethod.Put,
            "POST" => HttpMethod.Post,
            "DELETE" => HttpMethod.Delete,
            "OPTIONS" => HttpMethod.Options,
            "PATCH" => HttpMethod.Patch,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Dictionary<HttpMethod, List<string>> GetNormalizedPathsForServer(UpstreamServer server)
    {
        var paths = new Dictionary<HttpMethod, List<string>>();
        var apiDoc = _apis[server];
        if (apiDoc == null)
        {
            return new Dictionary<HttpMethod, List<string>>();
        }

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