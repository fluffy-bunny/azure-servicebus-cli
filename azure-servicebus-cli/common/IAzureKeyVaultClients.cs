using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace Common
{
    public interface IAzureKeyVaultClients
    {
        KeyClient CreateKeyClient(string keyVaultUrl);
        SecretClient CreateSecretClient(string keyVaultUrl);
    }
}
