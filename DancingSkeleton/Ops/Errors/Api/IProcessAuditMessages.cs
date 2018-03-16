using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Web.Api;

namespace DancingSkeleton.Ops.Errors.Api
{
    interface IProcessFailedMessages
    {
        void Handle(FailedMessage message, IEnvironment environment);
    }
}