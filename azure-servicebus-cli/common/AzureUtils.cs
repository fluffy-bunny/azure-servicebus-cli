using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using System;

namespace Common
{
    public static class AzureUtils
    {
        
        public static AzureClient FetchAzureClient()
        {
            try
            {
                //=================================================================
                // Authenticate
                var clientId = Environment.GetEnvironmentVariable("ARM_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET");
                var subscriptionId = Environment.GetEnvironmentVariable("ARM_SUBSCRIPTION_ID");
                var tenantId = Environment.GetEnvironmentVariable("ARM_TENANT_ID");

                AzureCredentialsFactory f = new AzureCredentialsFactory();
                var msi = new MSILoginInformation(MSIResourceType.AppService);
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var credentials = f.FromMSI(msi, AzureEnvironment.AzureGlobalCloud);
                credentials = f.FromUserAssigedManagedServiceIdentity(clientId, MSIResourceType.AppService, AzureEnvironment.AzureGlobalCloud);
                var azureAuth = Azure.Configure()
                                .WithLogLevel(HttpLoggingDelegatingHandler.Level.BodyAndHeaders)
                                .Authenticate(credentials);

           //     log.LogInformation("Authenticating with Azure using MSI");
                var azure = azureAuth.WithSubscription(subscriptionId);


              /*

                ServicePrincipalLoginInformation loginInfo = new ServicePrincipalLoginInformation()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };
                credentials = new AzureCredentials(loginInfo, tenantId, AzureEnvironment.AzureGlobalCloud);
                azureAuth = Azure.Configure()
                             .WithLogLevel(HttpLoggingDelegatingHandler.Level.BodyAndHeaders)
                             .Authenticate(credentials);

                azure = Azure
                                 .Configure()
                                 .WithLogLevel(HttpLoggingDelegatingHandler.Level.BodyAndHeaders)
                                 .Authenticate(credentials)
                                 .WithSubscription(subscriptionId);
              */
                return new AzureClient()
                {
                    AzureInstance = azure,
                    AzureCredentials = credentials
                };
            }
            catch (Exception ex)
            {
                Utilities.Log(ex);
            }
            return null;
        }
    }
}
