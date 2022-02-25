# SwaggerProxy

Simple HTTP forwarder that reads swagger definitions from configured upstream servers and proxies requests to them.

## Running the proxy

Simply run `dotnet run -- <configfile>`

## Configuration

Create a config file (`proxyconfig.json` for example):

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
