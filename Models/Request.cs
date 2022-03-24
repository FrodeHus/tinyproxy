namespace TinyProxy.Models;

public class Request
{
    public Dictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

    public Request()
    {
        
    }

    public Request(Dictionary<string,string> headers)
    {
        Headers = headers;
    }
}