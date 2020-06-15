﻿using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSDeleteInstanceCommand
{
    public static class VMSSDeleteInstance
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public string ResourceGroup { get; set; }
            public string ScaleSet { get; set; }
            public string InstanceId { get; set; }
            public Request(AzureClient azureClient)
            {
                AzureClient = azureClient;
            }
        }

        public class Response
        {
            public IVirtualMachineScaleSet VirtualMachineScaleSet { get; set; }
            public List<IVirtualMachineScaleSetVM> VirtualMachineScaleSetVMs { get; set; }
            public Exception Exception { get; set; }
            public string FullVMInstanceId { get; internal set; }
        }
        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response { };
                try
                {
                    // GetVirtualMachineScaleSetVMs
                    var rg = await request.AzureClient.AzureInstance.ResourceGroups.GetByNameAsync(request.ResourceGroup);
                    if (rg == null)
                    {
                        throw new Exception($"rg:{request.ResourceGroup} does not exist!");
                    }

                    response.VirtualMachineScaleSet = await request.AzureClient.AzureInstance.GetScaleSetAsync(rg.Name, request.ScaleSet);
                
                    var subscriptionId = request.AzureClient.AzureInstance.SubscriptionId;

                    var fullVMInstanceId = $"/subscriptions/{subscriptionId}/resourceGroups/{request.ResourceGroup}/providers/Microsoft.Compute/virtualMachineScaleSets/{request.ScaleSet}/virtualMachines/{request.InstanceId}";

                    var vmInstance = response.VirtualMachineScaleSet.VirtualMachines.GetInstance(request.InstanceId);
                    if(vmInstance != null)
                    {
                        response.FullVMInstanceId = fullVMInstanceId;
                        await vmInstance.DeleteAsync();
                        await response.VirtualMachineScaleSet.RefreshAsync();
                    }
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