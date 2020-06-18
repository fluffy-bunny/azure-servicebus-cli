using Common.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class VirtualMachineScaleSetResponse
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public VirtualMachineScaleSetHandle VirtualMachineScaleSet { get; set; }
    }
    public interface IAzureManagementApi
    {
        Task<HttpResponseMessage> DeleteVirtualMachineScaleSetVM(string subscriptionId, string resourceGroupName,
            string vmScaleSetName, List<string> instanceIds, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> SetVirtualMachineScaleSetCapacity(string subscriptionId, string resourceGroupName,
            string vmScaleSetName, int capacity, CancellationToken cancellationToken = default);

        Task<VirtualMachineScaleSetResponse> GetVirtualMachineScaleSetInfo(string subscriptionId, string resourceGroupName,
            string vmScaleSetName, CancellationToken cancellationToken = default);

    }
}
