using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Secrets;
using System.Threading.Tasks;

namespace Common
{
    public interface IAzureKeyVaultClients
    {
        KeyClient CreateKeyClient(string keyVaultUrl);
        SecretClient CreateSecretClient(string keyVaultUrl);
        Task<CryptographyClient> CreateCryptographyClientAsync(KeyClient keyClient, string keyName, string version = null);
    }
}
