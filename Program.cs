using CommandLine;
using TinyProxy.Infrastructure;
using TinyProxy.Server;

var options = new ProxyOptions();

Parser.Default.ParseArguments<ProxyOptions>(args)
    .WithParsed<ProxyOptions>(o =>
    {
        options = o;
    });

var proxy = new Proxy();
proxy.Configure(args, options);
proxy.Start();
