namespace DancingSkeleton.Capabilities.PerformanceMonitoring
{
    class GetMonitoringDataResponse
    {
        public GetMonitoringDataResponse(decimal[] measurements)
        {
            Measurements = measurements;
        }

        public decimal[] Measurements { get; }
    }
}