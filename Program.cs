using CommandLine;
using TinyProxy.Infrastructure;
using TinyProxy.Server;


Parser.Default.ParseArguments<ProxyOptions, ProxyConfigure>(args)
    .MapResult(
        (ProxyOptions o) =>
        {
            var proxy = new Proxy();
            proxy.Configure(args, o);
            proxy.Start();
            return 0;
        },
        (ProxyConfigure c) =>
        {
            if (string.IsNullOrEmpty(c.ConfigFile))
            {
                Console.WriteLine("Config file required");
                return 1;
            }
            if (c.Initialize)
            {
                ProxyConfig.Initialize(c);
            }
            else
            {
                ProxyConfig.UpdateConfig(c);
            }
            
            return 0;
        },
        errs => 1
    );