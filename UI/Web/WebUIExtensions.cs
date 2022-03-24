namespace TinyProxy.UI.Web;

public static class WebUIExtensions
{
    public static IApplicationBuilder UseWebUI(this IApplicationBuilder app)
    {
        var options = new WebUIOptions();
        return app.UseWebUI(options);
    }

    public static IApplicationBuilder UseWebUI(this IApplicationBuilder app, Action<WebUIOptions> setup = null)
    {
        var options = new WebUIOptions();
        using var scope = app.ApplicationServices.CreateScope();
        setup?.Invoke(options);
        return app.UseWebUI(options);
    }

    public static IApplicationBuilder UseWebUI(this IApplicationBuilder app, WebUIOptions options)
    {
        return app.UseMiddleware<WebUIMiddleware>(options);
    }

    public static IEndpointConventionBuilder MapWebUI(this IEndpointRouteBuilder routeBuilder, string pattern = "/ui/{**catch-all}")
    {
        var pipeline = routeBuilder.CreateApplicationBuilder().UseWebUI().Build();
        return routeBuilder.MapGet(pattern,pipeline);
    }
}