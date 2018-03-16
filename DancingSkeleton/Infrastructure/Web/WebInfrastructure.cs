using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure.Web
{
    class WebInfrastructure
    {
        readonly ConcurrentDictionary<string, BlockingCollection<HttpEnvelope>> queues = new ConcurrentDictionary<string, BlockingCollection<HttpEnvelope>>();
        readonly Random r = new Random();

        public Task<object> Send(object request, string urlSuffix)
        {
            var matchingQueues = queues.Where(q => q.Key.EndsWith("-" + urlSuffix)).ToArray();

            if (matchingQueues.Length == 0)
            {
                throw new Exception("Unknown URL: " + urlSuffix);
            }
            var randomQueue = matchingQueues[r.Next(matchingQueues.Length)];

            Console.WriteLine($"Sending request {request.GetType().Name} to {randomQueue.Key}");

            var source = new TaskCompletionSource<object>();
            var envelope = new HttpEnvelope(request, urlSuffix, source);
            randomQueue.Value.Add(envelope);

            return source.Task;
        }

        public Task<object> Send(object request, string urlPrefix, string urlSuffix)
        {
            if (queues.TryGetValue(urlPrefix + "-" + urlSuffix, out var queue))
            {
                var source = new TaskCompletionSource<object>();
                var envelope = new HttpEnvelope(request, urlSuffix, source);
                queue.Add(envelope);

                return source.Task;
            }
            throw new Exception("Unknown URL: " + urlPrefix + "-" + urlSuffix);
        }

        public IEnumerable<HttpEnvelope> Listen(string urlPrefix, string urlSuffix, CancellationToken token)
        {
            var queue = queues.GetOrAdd(urlPrefix + "-" + urlSuffix, new BlockingCollection<HttpEnvelope>());
            return queue.GetConsumingEnumerable(token);
        }
    }
}