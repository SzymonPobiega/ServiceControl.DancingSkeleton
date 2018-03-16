using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DancingSkeleton.Infrastructure.Queue
{
    class QueueInfrastructure
    {
        readonly ConcurrentDictionary<string, BlockingCollection<Message>> queues = new ConcurrentDictionary<string, BlockingCollection<Message>>();

        public IEnumerable<Message> ReceiveFrom(string queueName, CancellationToken token)
        {
            var queue = queues.GetOrAdd(queueName, new BlockingCollection<Message>());
            return queue.GetConsumingEnumerable(token);
        }

        public void SendTo(Message message, string queueName)
        {
            var queue = queues.GetOrAdd(queueName, new BlockingCollection<Message>());
            queue.Add(message);
        }
    }
}