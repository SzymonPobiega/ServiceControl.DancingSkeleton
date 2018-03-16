using System.Collections.Generic;

namespace DancingSkeleton.Capabilities.Recoverability
{
    class FailedMessageDatabase
    {
        public List<FailedMessageData> FailedMessages { get; } = new List<FailedMessageData>();
    }
}