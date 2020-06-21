using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Common
{
    public class JwksDiscoveryDocument
    {
        public List<IdentityServer4.Models.JsonWebKey> keys { get; set; }
    }
    public class PreferredCryptographyClient
    {
        public CryptographyClient CryptographyClient { get; set; }
        public KeyProperties KeyProperties { get; set; }
    }
    public interface IAzureKeyVaultServices
    {
        Task<JwksDiscoveryDocument> GetECDsaJwksDiscoveryDocumentAsync(string keyVaultName, string keyName);
        Task<string> CreateECDsaSignedJWTAsync(string keyVaultName, string keyName, Token token);
        Task<PreferredCryptographyClient> CreatePreferredCryptographyClientAsync(
            KeyClient keyClient,
            string keyName);
    }
}
