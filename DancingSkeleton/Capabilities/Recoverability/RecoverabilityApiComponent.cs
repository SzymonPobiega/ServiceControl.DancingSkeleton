using System.Linq;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Web.Api;

namespace DancingSkeleton.Capabilities.Recoverability
{
    class RecoverabilityApiComponent : IController
    {
        public string UrlSuffix => "errors";

        public Task<object> Process(object request, IHttpClient client, IEnvironment environment)
        {
            var typedRequest = (GetFailedMessages)request;

            var db = environment.GetSharedService<FailedMessageDatabase>();

            var response = new GetFailedMessagesResponse(db.FailedMessages.Take(10).ToList());
            return Task.FromResult<object>(response);
        }
    }
}