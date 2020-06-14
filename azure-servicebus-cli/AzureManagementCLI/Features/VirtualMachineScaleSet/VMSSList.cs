using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet
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
                    Result = new List<IVirtualMachineScaleSet>()
                };
                try
                {
                    var rg = await request.AzureClient.AzureInstance.ResourceGroups.GetByNameAsync(request.ResourceGroup);
                    if (rg == null)
                    {
                        throw new Exception($"rg:{request.ResourceGroup} does not exist!");
                    }
                  

                    var virtualMachineScaleSetsList = 
                        await request.AzureClient.AzureInstance.VirtualMachineScaleSets
                        .ListByResourceGroupAsync(rg.Name);
                    if (virtualMachineScaleSetsList != null)
                    {
                        foreach (var item in virtualMachineScaleSetsList)
                        {
                            response.Result.Add(item);
                        }
                        do
                        {
                            virtualMachineScaleSetsList = await virtualMachineScaleSetsList.GetNextPageAsync();
                            if (virtualMachineScaleSetsList == null)
                            {
                                break;
                            }
                            foreach (var item in virtualMachineScaleSetsList)
                            {
                                response.Result.Add(item);
                            }
                        } while (true);
                    }
                    
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
