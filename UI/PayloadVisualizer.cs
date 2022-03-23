using System.Text;

namespace TinyProxy.UI;

public static class PayloadVisualizer
{
    public static async Task<string> Visualize(string contentType, Stream content)
    {
        if (string.IsNullOrEmpty(contentType))
        {
            return await GenericContent(content);
        }
        
        if (contentType.ToLowerInvariant().Contains("application/json"))
        {
            return await JsonContent(content);
        }

        if (contentType.ToLowerInvariant().Contains("html"))
        {
            return await HtmlContent(content);
        }
        if (contentType.ToLowerInvariant().Contains("text/"))
        {
            return await TextContent(content);
        }

        return await GenericContent(content);
    }

    private static async Task<string> HtmlContent(Stream content)
    {
        return await TextContent(content);
    }

    private static async Task<string> GenericContent(Stream content)
    {
        await using var reader = new MemoryStream();
        await content.CopyToAsync(reader);
        return Convert.ToBase64String(reader.ToArray());
    }

    private static async Task<string> JsonContent(Stream content)
    {
        return await TextContent(content);
    }

    private static async Task<string> TextContent(Stream content)
    {
        using var reader = new StreamReader(content, Encoding.UTF8, true, 1024, true);
        var payload = await reader.ReadToEndAsync();
        return payload;
    }
}