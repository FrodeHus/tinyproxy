namespace TinyProxy.UI.Web;

public class WebUIMiddleware
{
    private readonly RequestDelegate _next;

    public WebUIMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        await _next(httpContext);
    }
}