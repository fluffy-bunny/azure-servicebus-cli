using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListInstancesCommand
{
    public static class VMSSListInstances
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public IAzureManagementTokenProvider AzureManagementTokenProvider { get; }
            public string ResourceGroup { get; set; }
            public string ScaleSet { get; set; }
            public Request(AzureClient azureClient, IAzureManagementTokenProvider azureManagementTokenProvider)
            {
                AzureClient = azureClient;
                AzureManagementTokenProvider = azureManagementTokenProvider;
            }
        }

        public class Response
        {
            public IVirtualMachineScaleSet VirtualMachineScaleSet { get; set; }
            public List<IVirtualMachineScaleSetVM> VirtualMachineScaleSetVMs { get; set; }
            public Exception Exception { get; set; }
        }
        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response{};
                try
                {
                    var t = await request.AzureManagementTokenProvider.AcquireAccessTokenAsync();
                    var rg = await request.AzureClient.AzureInstance.ResourceGroups.GetByNameAsync(request.ResourceGroup);
                    if (rg == null)
                    {
                        throw new Exception($"rg:{request.ResourceGroup} does not exist!");
                    }

                    response.VirtualMachineScaleSet = await request.AzureClient.AzureInstance.GetScaleSetAsync(rg.Name, request.ScaleSet);
                    response.VirtualMachineScaleSetVMs = await response.VirtualMachineScaleSet.GetVirtualMachineScaleSetVMs();

                }
                catch (Exception ex)
                {
                    response.Exception = ex;
                }

                return response;
            }


        }
    }
}
