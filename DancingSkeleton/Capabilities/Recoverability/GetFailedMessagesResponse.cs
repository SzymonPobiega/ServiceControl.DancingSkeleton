using System.Collections.Generic;

namespace DancingSkeleton.Capabilities.Recoverability
{
    class GetFailedMessagesResponse
    {
        public GetFailedMessagesResponse(List<FailedMessageData> messages)
        {
            Messages = messages;
        }

        public List<FailedMessageData> Messages { get; }
    }
}