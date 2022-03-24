using System.Reflection;

namespace TinyProxy.UI.Web;

public class WebUIOptions
{
    public string RelativePath { get; set; } = "ui";
    public Func<Stream> IndexStream { get; set; } = () => typeof(WebUIOptions).GetTypeInfo().Assembly
        .GetManifestResourceStream("TinyProxy.UI.Web.www.index.html");
}