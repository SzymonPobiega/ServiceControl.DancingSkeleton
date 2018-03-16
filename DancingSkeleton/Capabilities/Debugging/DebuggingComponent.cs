using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DancingSkeleton.Capabilities.PerformanceMonitoring;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Web.Api;
using DancingSkeleton.Ops.Audits.Api;

namespace DancingSkeleton.Capabilities.Debugging
{
    class DebuggingComponent :
        IController,
        IProcessAuditMessages
    {
        readonly List<MessageData> database = new List<MessageData>();

        public string UrlSuffix => "messages-internal";

        public Task<object> Process(object request, IHttpClient client, IEnvironment environment)
        {
            var typedRequest = (InternalGetMessages)request;

            var response = new GetMessagesResponse(database.Take(10).ToList());
            return Task.FromResult<object>(response);
        }

        public void Handle(ProcessedMessage message)
        {
            database.Add(new MessageData(message.SendingEndpoint, message.ProcessingEndpoint));
        }
    }
}