namespace DancingSkeleton.Capabilities.Debugging
{
    class MessageData
    {
        public MessageData(string sendingEndpoint, string processingEndpoint)
        {
            SendingEndpoint = sendingEndpoint;
            ProcessingEndpoint = processingEndpoint;
        }

        public string ProcessingEndpoint { get; }
        public string SendingEndpoint { get; }


    }
}