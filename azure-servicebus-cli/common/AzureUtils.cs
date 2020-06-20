using IdentityModel.Client;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

                ServicePrincipalLoginInformation loginInfo = new ServicePrincipalLoginInformation()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };
                var credentials = new AzureCredentials(loginInfo, tenantId, AzureEnvironment.AzureGlobalCloud);
                var azureAuth = Microsoft.Azure.Management.Fluent.Azure.Configure()
                             .WithLogLevel(HttpLoggingDelegatingHandler.Level.BodyAndHeaders)
                             .Authenticate(credentials);

                var azure = Microsoft.Azure.Management.Fluent.Azure
                                 .Configure()
                                 .WithLogLevel(HttpLoggingDelegatingHandler.Level.BodyAndHeaders)
                                 .Authenticate(credentials)
                                 .WithSubscription(subscriptionId);
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
