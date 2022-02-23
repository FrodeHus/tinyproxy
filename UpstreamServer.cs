namespace SwaggerProxy;

public class UpstreamServer
{
    public string Name { get; set; }
    public Uri Url { get; set; } = new Uri("http://localhost:5000");
    public string SwaggerEndpoint { get; set; } = "/swagger/v1/swagger.json";
}