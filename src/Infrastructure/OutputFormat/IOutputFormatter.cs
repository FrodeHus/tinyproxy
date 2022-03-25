namespace TinyProxy.Infrastructure.OutputFormat;

public interface IOutputFormatter
{
    object Output(object obj);
}