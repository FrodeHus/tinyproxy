using Prometheus;

namespace TinyProxy.Models;

public static class ProxyMetrics
{
    private static readonly Gauge TotalRequests = Metrics.CreateGauge("total_requests", $"Total requests for all endpoints proxied", new GaugeConfiguration
    {
        LabelNames = new []{"remote", "route"}
    });
    
    public static void IncomingRequest(UpstreamHandler route)
    {
        if(string.IsNullOrEmpty(route.RemoteServer)) return;
        TotalRequests.WithLabels(route.RemoteServer, route.RelativePath).Inc();
    }
}