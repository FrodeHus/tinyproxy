using TinyProxy.Infrastructure;

public class Request{
    public int Id{get;set;}
    public string? TraceIdentifier { get; set; }
    public string Path { get; set; } = null!;
    public string? Method { get; set; } = null!;
    public Dictionary<string,string>? RequestHeaders { get; set; } 
    public Dictionary<string,string>? ResponseHeaders { get; set; } 
    public string? RequestContentId { get; set; } = null!;
    public long? RequestContentLength{get;set;}
    public string? ResponseContentId { get; set; } = null!;
    public long? ResponseContentLength{get;set;}
    public ProxyRoute? Handler { get; set; }
    public int StatusCode {get;set;}
}