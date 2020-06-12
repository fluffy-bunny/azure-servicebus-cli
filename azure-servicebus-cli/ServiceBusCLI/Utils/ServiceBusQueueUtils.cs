using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Utils
{
    public class ServiceBusQueueUtils : IServiceBusQueueUtils
    {
        public ServiceBusQueueUtils(
            ISessionSettings sessionSettings,
            ISecurityAccessSignatureProvider securityAccessSignatureProvider)
        {
            SessionSettings = sessionSettings;
            SecurityAccessSignatureProvider = securityAccessSignatureProvider;

        }

        public ISessionSettings SessionSettings { get; }
        public ISecurityAccessSignatureProvider SecurityAccessSignatureProvider { get; }

        // https://docs.microsoft.com/en-us/rest/api/servicebus/renew-lock-for-a-message
        // https://scalesets.servicebus.windows.net/w10rs5pr0/messages/f8b1652bd8f6498d858d01c4a58b9429/efbfc7a6-a059-4570-9409-88134d1a08e6


        public async Task<HttpResponseMessage> RenewLockAsync(string messageId, string lockToken)
        {
            try
            {
                var uri = $"https://{SessionSettings.Namespace}.servicebus.windows.net/{SessionSettings.Queue}/messages/{messageId}/{lockToken}";
                var sasToken = SecurityAccessSignatureProvider.GenerateSecurityAccessSignature(3600);
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                client.DefaultRequestHeaders.Add("Authorization", sasToken);
                var data = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, data);
                return response;
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }
    }
}