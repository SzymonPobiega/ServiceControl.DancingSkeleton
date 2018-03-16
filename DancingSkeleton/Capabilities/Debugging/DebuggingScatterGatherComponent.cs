using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Web.Api;

namespace DancingSkeleton.Capabilities.Debugging
{
    class DebuggingScatterGatherComponent : IController
    {
        public string UrlSuffix => "messages";

        public async Task<object> Process(object request, IHttpClient client, IEnvironment environment)
        {
            var typedRequest = (GetMessages)request;

            var hosts = environment.GetHostsFor(typeof(DebuggingComponent));

            var allMessageData = new List<MessageData>();
            foreach (var h in hosts)
            {
                var response = (GetMessagesResponse) await client.Send(new InternalGetMessages(), h, "messages-internal");
                allMessageData.AddRange(response.Messages);
            }
            
            return new GetMessagesResponse(allMessageData);
        }
    }
}