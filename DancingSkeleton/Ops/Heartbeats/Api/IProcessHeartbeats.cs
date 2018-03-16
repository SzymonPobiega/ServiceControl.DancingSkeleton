namespace DancingSkeleton.Ops.Heartbeats.Api
{
    interface IProcessHeartbeats
    {
        void Handle(HeartbeatMessage message);
    }
}