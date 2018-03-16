using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure.Queue
{
    class RawEndpoint
    {
        readonly QueueInfrastructure infrastructure;
        readonly Action<Message> onMessage;
        readonly string queueName;
        CancellationTokenSource stopTokenSource;
        Task receiveTask;

        public RawEndpoint(QueueInfrastructure infrastructure, Action<Message> onMessage, string queueName)
        {
            this.infrastructure = infrastructure;
            this.onMessage = onMessage;
            this.queueName = queueName;
        }

        public Task Start()
        {
            stopTokenSource = new CancellationTokenSource();

            var receiver = infrastructure.ReceiveFrom(queueName, stopTokenSource.Token);

            receiveTask = Task.Run(() => ProcessMessages(receiver));

            return Task.CompletedTask;
        }

        void ProcessMessages(IEnumerable<Message> receiver)
        {
            foreach (var message in receiver)
            {
                onMessage(message);
            }
        }

        public Task Stop()
        {
            stopTokenSource.Cancel();
            stopTokenSource.Dispose();
            return receiveTask;
        }
    }
}