namespace SwaggerProxy;

public class UpstreamServer
{
    public Uri Url { get; set; } = new Uri("http://localhost:5000");
    public string SwaggerEndpoint { get; set; } = "/swagger/v1/swagger.json";
}