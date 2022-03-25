using System.Reflection.Emit;
using System.Text.Json;
using Spectre.Console;

namespace TinyProxy.Infrastructure.OutputFormat;

public class JsonFormatter : IOutputFormatter
{
    public object Output(object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions {WriteIndented = true});
    }
}