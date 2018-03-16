using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Queue;
using DancingSkeleton.Ops.Heartbeats.Api;

namespace DancingSkeleton.Ops.Heartbeats
{
    class HeartbeatsEngine : IEngine
    {
        readonly List<IProcessHeartbeats> processors = new List<IProcessHeartbeats>();
        readonly RawEndpoint endpoint;

        public HeartbeatsEngine(QueueInfrastructure infrastructure)
        {
            endpoint = new RawEndpoint(infrastructure, OnMessage, "heartbeat");
        }

        void OnMessage(Message message)
        {
            var deserializedMessage = (HeartbeatMessage) message.Payload;
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
            processors.Add((IProcessHeartbeats)component);
        }

        public bool CanHandle(Type componentType)
        {
            return componentType == typeof(IProcessHeartbeats);
        }
    }
}