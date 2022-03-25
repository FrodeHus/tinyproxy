using System.Text;

namespace TinyProxy.UI.CommandLine;

public static class PayloadVisualizer
{
    public static string Visualize(string contentType, string content)
    {
        if (string.IsNullOrEmpty(contentType))
        {
            return GenericContent(content);
        }

        if (contentType.Contains("application/json", StringComparison.InvariantCultureIgnoreCase))
        {
            return JsonContent(content);
        }

        if (contentType.Contains("html", StringComparison.InvariantCultureIgnoreCase))
        {
            return HtmlContent(content);
        }
        if (contentType.Contains("text/", StringComparison.InvariantCultureIgnoreCase))
        {
            return TextContent(content);
        }

        return GenericContent(content);
    }

    private static string HtmlContent(string content)
    {
        return TextContent(content);
    }

    private static string GenericContent(string content)
    {
        return content;
    }

    private static string JsonContent(string content)
    {
        return TextContent(content);
    }

    private static string TextContent(string content)
    {
        var payload = Convert.FromBase64String(content);
        return Encoding.UTF8.GetString(payload);
    }
}