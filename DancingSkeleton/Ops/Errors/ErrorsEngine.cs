using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Queue;
using DancingSkeleton.Infrastructure.Web.Api;
using DancingSkeleton.Ops.Errors.Api;

namespace DancingSkeleton.Ops.Errors
{
    class ErrorsEngine : IEngine
    {
        readonly IEnvironment environment;
        readonly List<IProcessFailedMessages> processors = new List<IProcessFailedMessages>();
        readonly RawEndpoint endpoint;

        public ErrorsEngine(QueueInfrastructure infrastructure, IEnvironment environment)
        {
            this.environment = environment;
            endpoint = new RawEndpoint(infrastructure, OnMessage, "error");
        }

        void OnMessage(Message message)
        {
            var deserializedMessage = (FailedMessage) message.Payload;
            foreach (var processor in processors)
            {
                processor.Handle(deserializedMessage, environment);
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
            processors.Add((IProcessFailedMessages)component);
        }

        public bool CanHandle(Type componentType)
        {
            return componentType == typeof(IProcessFailedMessages);
        }
    }
}