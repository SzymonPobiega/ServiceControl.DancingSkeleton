namespace DancingSkeleton.Ops.Metrics.Api
{
    interface IProcessMetrics
    {
        void Handle(MetricsMessage message);
    }
}