using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using System;

namespace Common
{
    public class AzureKeyVaultClients : IAzureKeyVaultClients
    {
        private AzureKeyVaultTokenCredential _azureKeyVaultTokenCredential;

        public AzureKeyVaultClients(AzureKeyVaultTokenCredential azureKeyVaultTokenCredential)
        {
            _azureKeyVaultTokenCredential = azureKeyVaultTokenCredential;
        }

        public SecretClient CreateSecretClient(string keyVaultUrl)
        {
            return new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: _azureKeyVaultTokenCredential);

        }

        public KeyClient CreateKeyClient(string keyVaultUrl)
        {
            return new KeyClient(vaultUri: new Uri(keyVaultUrl), credential: _azureKeyVaultTokenCredential);
        }
    }
}
