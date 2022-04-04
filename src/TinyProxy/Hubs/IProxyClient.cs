public interface IProxyClient{
    Task ReceiveRequest(Request request);
    Task ReceiveContent(string contentId, string content);

}