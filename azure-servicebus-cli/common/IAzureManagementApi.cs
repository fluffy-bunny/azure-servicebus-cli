using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public interface IAzureManagementApi
    {
        Task<HttpResponseMessage> DeleteVirtualMachineScaleSetVM(string subscriptionId, string name1, string name2, List<string> instanceIds, CancellationToken token = default);
    }
}
