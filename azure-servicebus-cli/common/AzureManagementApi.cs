using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class AzureManagementApi: IAzureManagementApi
    {
        private ISerializer _serializer;
        private IAzureManagementTokenProvider _azureManagementTokenProvider;

        public AzureManagementApi(ISerializer serializer,IAzureManagementTokenProvider azureManagementTokenProvider)
        {
            _serializer = serializer;
            _azureManagementTokenProvider = azureManagementTokenProvider;
        }
        class Body
        {
            [JsonPropertyName("instanceIds")]
            public List<string> InstanceId { get; set; }
        }
        public async Task<HttpResponseMessage> DeleteVirtualMachineScaleSetVM(string subscriptionId, string resourceGroupName, 
            string vmScaleSetName, List<string> instanceIds, CancellationToken cancellationToken = default)
        {
            // https://docs.microsoft.com/en-us/rest/api/compute/virtualmachinescalesets/deleteinstances
            try
            {
                var token = await _azureManagementTokenProvider.AcquireAccessTokenAsync();
                var uri = $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Compute/virtualMachineScaleSets/{vmScaleSetName}/delete?api-version=2019-12-01";
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var body = new Body()
                {
                    InstanceId = instanceIds
                };
                var jsonBody = _serializer.Serialize(body);
                var data = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, data, cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

      
    }
}
