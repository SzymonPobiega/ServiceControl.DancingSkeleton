using System.Linq;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure.Web.Api;
using DancingSkeleton.Ops.Metrics.Api;

namespace DancingSkeleton.Capabilities.PerformanceMonitoring
{
    class MonitoringComponent : 
        IController,
        IProcessMetrics
    {
        readonly MonitoringDatabase database = new MonitoringDatabase();
        
        public string UrlSuffix => "monitoring";

        public Task<object> Process(object request, IHttpClient client, IEnvironment environment)
        {
            var typedRequest = (GetMonitoringData) request;
            var response = new GetMonitoringDataResponse(database.Measurements.Take(10).ToArray());
            return Task.FromResult<object>(response);
        }

        public void Handle(MetricsMessage message)
        {
            database.Measurements.AddRange(message.Measurements);
        }
    }
}