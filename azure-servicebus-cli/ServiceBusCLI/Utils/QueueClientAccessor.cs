using Microsoft.Azure.ServiceBus;

namespace ServiceBusCLI.Utils
{
    public class QueueClientAccessor : IQueueClientAccessor
    {
        public QueueClient QueueClient { get; set; }
    }

    

}
