using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public class JwksDiscoveryDocument
    {
        public List<IdentityServer4.Models.JsonWebKey> keys { get; set; }
    }
    public interface IAzureKeyVaultServices
    {
        Task<JwksDiscoveryDocument> GetECDsaJwksDiscoveryDocumentAsync(string keyVaultName, string key);
    }
}
