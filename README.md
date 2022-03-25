# TinyProxy

[![.NET](https://github.com/FrodeHus/tinyproxy/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/FrodeHus/tinyproxy/actions/workflows/build_and_test.yml)
[![CodeQL](https://github.com/FrodeHus/tinyproxy/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/FrodeHus/tinyproxy/actions/workflows/codeql-analysis.yml)


This project was born out of a need for a better local developer experience (IMHO) and thus the limitations of this proxy is **local**.

TinyProxy will only bind to localhost - it is meant as a local development proxy.

## Installation

### Dotnet Tool
Simply run `dotnet tool install --global Reothor.Lab.TinyProxy`

Usage can be found by executing `dotnet tinyproxy --help`.
```
USAGE:
    TinyProxy.dll [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information   
    -v, --version    Prints version information

COMMANDS:
    start      
    config  
```

### Docker
Usage information: 

`docker run frodehus/tinyproxy:latest --help`

Quickstart: 

`docker run -it -v /<path>/proxyconfig.json:/config.json -p 8080:80 -p 8443:443 frodehus/tinyproxy:latest start -f /config.json`

## Run the proxy

Create a config file (see below) and run `dotnet TinyProxy start -f <configfile>`.

If `<configfile>` is omitted, it will default to `{APPDATA}/tinyproxyconfig.json`.


## Configuration

### Quick start

Getting started with your first upstream server:

`dotnet tinyproxy config add server MyServer https://example.com`

This will create an empty config file under `$HOME/.config/tinyproxyconfig.json` (can be overriden by using the `-f <config_file>` option).

### Manual config
Create a config file such as `proxy_dev.json`:

```json
{
  "UpstreamServers": [
    {
      "Name": "MyDevAPI",
      "Url": "http://localhost:5100",
      "SwaggerEndpoint": "swagger/v1/swagger.json"
    },
    {
      "Name": "MyDevAPI2",
      "Url": "http://localhost:5200",
      "Preferred": true,
      "SwaggerEndpoint": "swagger/v1/swagger.json"
    },
    {
      "Name": "RemoteNonSwagger",
      "Url": "https://api.wishthisexisted.dev",
      "Prefix": "/weather"
      "Routes": [
                 {
                    "RelativePath": "/weatherforecast",
                    "HttpMethods": [
                        "GET",
                      ]
                 }
      ]
    }
  ]
}
```
This will proxy 3 servers where:

- MyDevAPI2 is preferred so any routes that are duplicates between services will be overriden by this one
- RemoteNonSwagger has no Swagger definition available, so we define static routes for this. Available at `/weather/weatherforecast` due to the `Prefix` property.

## Metrics

TinyProxy has a Prometheus-enabled endpoint which provides insights into which remote server and endpoint are receiving proxied requests.

This metric is available at `/metrics`.
