using System.Net.Http;
using System.Threading.Tasks;

namespace Common
{
    public interface IAzureManagementApi
    {
        Task<HttpResponseMessage> DeleteVirtualMachineScaleSetVM(string subscriptionId, string resourceGroupName, 
            string vmScaleSetName, string instanceId, System.Threading.CancellationToken cancellationToken = default);
    }
}
