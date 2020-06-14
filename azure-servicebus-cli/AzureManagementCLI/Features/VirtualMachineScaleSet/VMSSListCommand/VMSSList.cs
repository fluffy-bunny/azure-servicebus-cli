using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListCommand
{
    public static class VMSSList
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public string ResourceGroup { get; set; }
            public Request(AzureClient azureClient)
            {
                AzureClient = azureClient;
            }
        }

        public class Response
        {
            public List<IVirtualMachineScaleSet> Result { get; set; }
            public Exception Exception { get; set; }
        }
        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response
                {
                };
                try
                {
                    var rg = await request.AzureClient.AzureInstance.ResourceGroups.GetByNameAsync(request.ResourceGroup);

                    if (rg == null)
                    {
                        throw new Exception($"rg:{request.ResourceGroup} does not exist!");
                    }
                    response.Result = await request.AzureClient.AzureInstance.GetScaleSetsForResourceGroupAsync(rg.Name);
                }
                catch (Exception ex)
                {
                    response.Exception = ex;
                    response.Result = null;
                }

                return response;
            }
        }
    }
}
