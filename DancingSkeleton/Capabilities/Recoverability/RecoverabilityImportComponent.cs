using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Web.Api;
using DancingSkeleton.Ops.Errors.Api;

namespace DancingSkeleton.Capabilities.Recoverability
{
    class RecoverabilityImportComponent : IProcessFailedMessages
    {
        public void Handle(FailedMessage message, IEnvironment environment)
        {
            var db = environment.GetSharedService<FailedMessageDatabase>();
            db.FailedMessages.Add(new FailedMessageData(message.SendingEndpoint, message.ProcessingEndpoint));
        }
    }
}