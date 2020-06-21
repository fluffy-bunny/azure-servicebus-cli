using Azure.Security.KeyVault.Keys;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Common.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommon(this IServiceCollection services)
        {
            services.AddSerializer();
            services.AddBase64Encoder();
            services.AddTransient(typeof(AppSettings<>), typeof(AppSettings<>));
            services.AddSingleton<IAzureManagementTokenProvider, AzureManagementTokenProvider>();
            services.AddSingleton<IAzureManagementApi, AzureManagementApi>();
            return services;
        }
        public static IServiceCollection AddSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer, Serializer>();
            return services;
        }
        public static IServiceCollection AddBase64Encoder(this IServiceCollection services)
        {
            services.AddSingleton<IBase64Encoder, Base64Encoder>();
            return services;
        }
        public static IServiceCollection AddAzureClients(this IServiceCollection services)
        {
            services.AddSingleton<AzureClient>(sp =>
            {
                using (new DisposableStopwatch(t => Utilities.Log($"AzureUtils.FetchAzureClient() - {t} elapsed")))
                {
                    return AzureUtils.FetchAzureClient();
                }
            });
            services.AddSingleton<KeyVaultClient>(sp =>
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var authCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
                return new KeyVaultClient(authCallback);
            });
            services.AddSingleton<AzureKeyVaultTokenCredential>();
            services.AddSingleton<IAzureKeyVaultClients, AzureKeyVaultClients>();
            services.AddSingleton<IAzureKeyVaultServices, AzureKeyVaultServices>();
      

            return services;
        }
    }
}
