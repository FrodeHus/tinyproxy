# TinyProxy

[![.NET](https://github.com/FrodeHus/tinyproxy/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/FrodeHus/tinyproxy/actions/workflows/build_and_test.yml)


Simple HTTP forwarder that reads swagger definitions from configured upstream servers and proxies requests to them.

## Installation

Simply run `dotnet tool install --global Reothor.Lab.TinyProxy`

Usage can be found by executing `dotnet tinyproxy help`.

## Run the proxy

Create a config file (see below) and run `dotnet TinyProxy start -f <configfile>`

## Configuration

### Quick start

To get a config file started, run `dotnet tinyproxy configure --init -f <configfile> --url https://<server> --name MyRemote1`.

You can also provide optional parameters such as `--prefix` and `--swagger`.

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
        "/weatherforecast"
      ]
    }
  ]
}
```
This will proxy 3 servers where:

- MyDevAPI2 is preferred so any routes that are duplicates between services will be overriden by this one
- RemoteNonSwagger has no Swagger definition available, so we define static routes for this. Available at `/weather/weatherforecast` due to the `Prefix` property.
