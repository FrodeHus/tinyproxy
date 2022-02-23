namespace SwaggerProxy;

public class UpstreamServer
{
    /// <summary>
    /// Display name for the server (informational only)
    /// </summary>
    public string Name { get; set; } = "<not set>";
    public Uri Url { get; set; } = new Uri("http://localhost:5000");
    /// <summary>
    /// Relative path to swagger JSON
    /// </summary>
    public string SwaggerEndpoint { get; set; } = "/swagger/v1/swagger.json";
    /// <summary>
    /// If true, any routes defined on this upstream server will take precedence over duplicate routes in another server.
    /// </summary>
    public bool Preferred { get; set; }
}