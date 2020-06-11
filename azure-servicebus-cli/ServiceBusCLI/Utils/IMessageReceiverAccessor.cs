using Microsoft.Azure.ServiceBus.Core;

namespace ServiceBusCLI.Utils
{
    public interface IMessageReceiverAccessor
    {
        IMessageReceiver MessageReceiver { get; }
    }
}