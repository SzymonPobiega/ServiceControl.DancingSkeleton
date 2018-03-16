using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Queue;
using DancingSkeleton.Ops.Metrics.Api;

namespace DancingSkeleton.Ops.Metrics
{
    class MetricsEngine : IEngine
    {
        readonly List<IProcessMetrics> processors = new List<IProcessMetrics>();
        readonly RawEndpoint endpoint;

        public MetricsEngine(QueueInfrastructure infrastructure)
        {
            endpoint = new RawEndpoint(infrastructure, OnMessage, "metrics");
        }

        void OnMessage(Message message)
        {
            var deserializedMessage = (MetricsMessage) message.Payload;
            foreach (var processor in processors)
            {
                processor.Handle(deserializedMessage);
            }
        }

        public Task Start()
        {
            return endpoint.Start();
        }

        public Task Stop()
        {
            return endpoint.Stop();
        }

        public void Add(object component)
        {
            processors.Add((IProcessMetrics)component);
        }

        public bool CanHandle(Type componentType)
        {
            return componentType == typeof(IProcessMetrics);
        }
    }
}