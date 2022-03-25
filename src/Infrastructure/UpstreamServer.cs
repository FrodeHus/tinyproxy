namespace TinyProxy.Infrastructure;

public class UpstreamServer
{
    /// <summary>
    /// Display name for the server (informational only)
    /// </summary>
    public string Name { get; set; } = "<not set>";
    public Uri Url { get; set; } = new Uri("http://localhost:5000");

    /// <summary>
    /// Adds a prefix for all routes for this upstream server
    /// </summary>
    public string Prefix { get; set; } = "";
    /// <summary>
    /// Relative path to swagger JSON
    /// </summary>
    public string SwaggerEndpoint { get; set; } = "";
    /// <summary>
    /// If true, any routes defined on this upstream server will take precedence over duplicate routes in another server.
    /// </summary>
    public bool Preferred { get; set; }
    /// <summary>
    /// Static routes that are not defined in Swagger
    /// </summary>
    public List<StaticRoute> Routes { get; set; } = new List<StaticRoute>();
}