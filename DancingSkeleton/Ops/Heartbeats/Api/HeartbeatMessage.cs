using System;

namespace DancingSkeleton.Ops.Heartbeats.Api
{
    class HeartbeatMessage
    {
        public HeartbeatMessage(DateTime timestamp)
        {
            Timestamp = timestamp;
        }

        public DateTime Timestamp { get; }
    }
}