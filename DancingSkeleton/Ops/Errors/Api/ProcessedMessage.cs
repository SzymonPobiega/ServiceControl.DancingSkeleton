namespace DancingSkeleton.Ops.Errors.Api
{
    class FailedMessage
    {
        public FailedMessage(string sendingEndpoint, string processingEndpoint)
        {
            SendingEndpoint = sendingEndpoint;
            ProcessingEndpoint = processingEndpoint;
        }

        public string ProcessingEndpoint { get; }
        public string SendingEndpoint { get; }
    }
}