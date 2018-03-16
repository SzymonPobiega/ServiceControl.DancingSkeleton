using System;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Web.Api;
using DancingSkeleton.Ops.Heartbeats.Api;
using DancingSkeleton.Ops.Metrics.Api;

namespace DancingSkeleton.Capabilities.EndpointLifecycle
{
    class UptimeMonitorComponent : 
        IController,
        IProcessMetrics,
        IProcessHeartbeats
    {
        DateTime lastHearbeat = DateTime.MinValue;
        
        public string UrlSuffix => "heartbeats";

        public Task<object> Process(object request, IHttpClient client, IEnvironment environment)
        {
            var typedRequest = (GetLastHeartbeat) request;
            var response = new GetLastHeartbeatResponse(lastHearbeat);
            return Task.FromResult<object>(response);
        }

        public void Handle(MetricsMessage message)
        {
            lastHearbeat = message.Timestamp;
        }

        public void Handle(HeartbeatMessage message)
        {
            lastHearbeat = message.Timestamp;
        }
    }
}