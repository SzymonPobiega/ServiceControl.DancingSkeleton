namespace DancingSkeleton.Infrastructure.Queue
{
    class Message
    {
        public Message(object payload)
        {
            Payload = payload;
        }

        public object Payload { get; }
    }
}