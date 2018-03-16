namespace DancingSkeleton.Capabilities.Recoverability
{
    class FailedMessageData
    {
        public FailedMessageData(string sendingEndpoint, string processingEndpoint)
        {
            SendingEndpoint = sendingEndpoint;
            ProcessingEndpoint = processingEndpoint;
        }

        public string ProcessingEndpoint { get; }
        public string SendingEndpoint { get; }
    }
}