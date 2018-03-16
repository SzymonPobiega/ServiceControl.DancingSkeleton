using System;

namespace DancingSkeleton.Capabilities.EndpointLifecycle
{
    class GetLastHeartbeatResponse
    {
        public GetLastHeartbeatResponse(DateTime lastHeartbeat)
        {
            LastHeartbeat = lastHeartbeat;
        }

        public DateTime LastHeartbeat { get; }
    }
}