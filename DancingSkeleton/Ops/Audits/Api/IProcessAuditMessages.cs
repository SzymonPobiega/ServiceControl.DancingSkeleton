namespace DancingSkeleton.Ops.Audits.Api
{
    interface IProcessAuditMessages
    {
        void Handle(ProcessedMessage message);
    }
}