namespace DancingSkeleton.Ops.Audits.Api
{
    class ProcessedMessage
    {
        public ProcessedMessage(string sendingEndpoint, string processingEndpoint)
        {
            SendingEndpoint = sendingEndpoint;
            ProcessingEndpoint = processingEndpoint;
        }

        public string ProcessingEndpoint { get; }
        public string SendingEndpoint { get; }
    }
}