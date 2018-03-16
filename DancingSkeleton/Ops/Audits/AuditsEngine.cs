using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Queue;
using DancingSkeleton.Ops.Audits.Api;

namespace DancingSkeleton.Ops.Audits
{
    class AuditsEngine : IEngine
    {
        readonly List<IProcessAuditMessages> processors = new List<IProcessAuditMessages>();
        readonly RawEndpoint endpoint;

        public AuditsEngine(QueueInfrastructure infrastructure)
        {
            endpoint = new RawEndpoint(infrastructure, OnMessage, "audit");
        }

        void OnMessage(Message message)
        {
            var deserializedMessage = (ProcessedMessage) message.Payload;
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
            processors.Add((IProcessAuditMessages)component);
        }

        public bool CanHandle(Type componentType)
        {
            return componentType == typeof(IProcessAuditMessages);
        }
    }
}