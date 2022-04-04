namespace TinyProxy.Models.OutputFormat;

public interface IOutputFormatter
{
    object Output(object obj);
}