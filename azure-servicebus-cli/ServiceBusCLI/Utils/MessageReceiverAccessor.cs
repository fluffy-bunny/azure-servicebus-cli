using Microsoft.Azure.ServiceBus.Core;

namespace ServiceBusCLI.Utils
{
    public class MessageReceiverAccessor : IMessageReceiverAccessor
    {
        public IMessageReceiver MessageReceiver { get; set; }
    }
}
