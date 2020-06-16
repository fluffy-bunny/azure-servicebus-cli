using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Common.TimedLock;

namespace Common
{
    public class AzureManagementTokenProvider : IAzureManagementTokenProvider
    {
        private readonly object _acquireLock = new object();
        private DateTime _expiration;
        private string _token;
        private TimedLock _lock;
        public AzureManagementTokenProvider()
        {
            _expiration = DateTime.UtcNow.AddDays(-1);
            _lock = new TimedLock();
        }
        public async Task<string> AcquireAccessTokenAsync()
        {
            var utcNow = DateTime.UtcNow;
            int result = DateTime.Compare(_expiration, utcNow);
            if (string.IsNullOrWhiteSpace(_token) || result <= 0)
            {
                LockReleaser releaser = await _lock.Lock(new TimeSpan(0, 0, 30));
                try
                {
                    var clientId = Environment.GetEnvironmentVariable("ARM_CLIENT_ID");
                    var clientSecret = Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET");
                    var subscriptionId = Environment.GetEnvironmentVariable("ARM_SUBSCRIPTION_ID");
                    var tenantId = Environment.GetEnvironmentVariable("ARM_TENANT_ID");
                    var client = new HttpClient();

                    var response = await client.RequestTokenAsync(new TokenRequest
                    {
                        Address = $"https://login.microsoftonline.com/{tenantId}/oauth2/token?api-version=1.0",
                        GrantType = "client_credentials",
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Parameters =
                        {
                            { "resource", "https://management.core.windows.net/"}
                        }
                    });
                    _token = response.AccessToken;
                    _expiration = DateTime.UtcNow.AddSeconds(response.ExpiresIn - 60);  // 1 minute grace.
                    
                }
                finally
                {
                    releaser.Dispose();
                }
            }
            return _token;
        }
    }
}
