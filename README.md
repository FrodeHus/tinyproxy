# SwaggerProxy

Simple HTTP forwarder that reads swagger definitions from configured upstream servers and proxies requests to them.

## Configuration

In `appsettings.json` add a section like this:

```json
"ProxyConfig": {
  "UpstreamServers": [
    {
      "Name": "MyDevAPI",
      "Url": "http://localhost:5100",
      "SwaggerEndpoint": "swagger/v1/swagger.json"
    }
  ]
}
```
