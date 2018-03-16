using System.Collections.Generic;

namespace DancingSkeleton.Capabilities.Debugging
{
    class GetMessagesResponse
    {
        public GetMessagesResponse(List<MessageData> messages)
        {
            Messages = messages;
        }

        public List<MessageData> Messages { get; }
    }
}