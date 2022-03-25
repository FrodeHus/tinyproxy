using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace TinyProxy.UI.Web;

public class WebUIMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebUIOptions _options;
    private readonly StaticFileMiddleware _staticFileMiddleware;
    private const string EmbeddedFileNamespace = "tinyproxy_dashboard/out";

    public WebUIMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnv,
        ILoggerFactory loggerFactory,
        WebUIOptions options)
    {
        _next = next;
        _options = options;
        _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options);
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var httpMethod = httpContext.Request.Method;
        var path = httpContext.Request.Path.Value;
        if (!Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RelativePath)}/?", RegexOptions.IgnoreCase))
        {
            await _next(httpContext);
            return;
        }
        
        if (httpMethod == "GET" &&
            Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RelativePath)}/?$", RegexOptions.IgnoreCase))
        {
            var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                ? "index.html"
                : $"{path.Split('/').Last()}/index.html";

            RespondWithRedirect(httpContext.Response, relativeIndexUrl);
            return;
        }

        await _staticFileMiddleware.Invoke(httpContext);
    }

    private void RespondWithRedirect(HttpResponse response, string location)
    {
        response.StatusCode = 301;
        response.Headers["Location"] = location;
    }

    private StaticFileMiddleware CreateStaticFileMiddleware(
        RequestDelegate next,
        IWebHostEnvironment hostingEnv,
        ILoggerFactory loggerFactory,
        WebUIOptions options)
    {
        var staticFileOptions = new StaticFileOptions
        {
            RequestPath = string.IsNullOrEmpty(options.RelativePath) ? string.Empty : $"/{options.RelativePath}",
            FileProvider =
                new ManifestEmbeddedFileProvider(typeof(WebUIMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
        };

        return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
    }
}