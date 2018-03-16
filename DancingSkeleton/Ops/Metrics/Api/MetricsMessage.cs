using System;

namespace DancingSkeleton.Ops.Metrics.Api
{
    class MetricsMessage
    {
        public MetricsMessage(DateTime timestamp, params decimal[] measurements)
        {
            Measurements = measurements;
            Timestamp = timestamp;
        }

        public decimal[] Measurements { get; }
        public DateTime Timestamp { get; }
    }
}