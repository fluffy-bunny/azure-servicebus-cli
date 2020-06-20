namespace Common
{
    public class AzureKeyVaultTokenCredential : AzureServiceTokenCredential
    {
        public AzureKeyVaultTokenCredential() : base("https://vault.azure.net")
        {
        }
    }
}
