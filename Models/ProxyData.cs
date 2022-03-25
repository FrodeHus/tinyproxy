namespace TinyProxy.Models;

public enum ProxyDataType{
    Unknown = 0,
    Request = 1,
    Response = 2
}
public class ProxyData
{

    public Dictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    public ProxyDataType Type { get; set; }
    public string Content { get; set; }
    public ProxyData()
    {
    }

    public ProxyData(Dictionary<string,string> headers, ProxyDataType type)
    {
        Headers = headers;
        Type = type;
    }
}