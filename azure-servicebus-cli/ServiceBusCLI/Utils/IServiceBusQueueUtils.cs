using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceBusCLI.Utils
{
    public interface IServiceBusQueueUtils
    {
        Task<HttpResponseMessage> RenewLockAsync(string messageId, string lockToken);
    }
}