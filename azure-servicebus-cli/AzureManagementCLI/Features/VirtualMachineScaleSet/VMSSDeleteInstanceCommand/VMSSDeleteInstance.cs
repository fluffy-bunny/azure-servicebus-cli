using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Management.Compute.Fluent.Models;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSDeleteInstanceCommand
{
    public static class VMSSDeleteInstance
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public IAzureManagementApi AzureManagementApi { get; }
            public string ResourceGroup { get; set; }
            public string ScaleSet { get; set; }
            public string InstanceId { get; set; }
            public Request(AzureClient azureClient, IAzureManagementApi azureManagementApi)
            {
                AzureClient = azureClient;
                AzureManagementApi = azureManagementApi;
            }
        }

        public class Response
        {
            public IVirtualMachineScaleSet VirtualMachineScaleSet { get; set; }
            public List<IVirtualMachineScaleSetVM> VirtualMachineScaleSetVMs { get; set; }
            public Exception Exception { get; set; }
            public string FullVMInstanceId { get; internal set; }
            public VirtualMachineInstanceView InstanceView { get; internal set; }
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
                    var view = vmInstance.InstanceView;
                    if (vmInstance != null)
                    {
                        response.FullVMInstanceId = fullVMInstanceId;
                        response.InstanceView = vmInstance.InstanceView;
                        var timeSpan = new TimeSpan(0, 0, 5);
                        using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
                        {
                            var response2 = await request.AzureManagementApi.DeleteVirtualMachineScaleSetVM(subscriptionId, rg.Name, response.VirtualMachineScaleSet.Name, request.InstanceId, cancellationTokenSource.Token);
                            /*
                            try
                            {
                                await vmInstance.DeleteAsync(cancellationTokenSource.Token);
                            }
                            catch (TaskCanceledException)
                            {
                                Console.WriteLine("Task was cancelled");
                            }
                            */
                        }
                        int loops = 5;
                        do
                        {
                            await vmInstance.RefreshInstanceViewAsync();
                            response.InstanceView = vmInstance.InstanceView;
                            var query = from item in vmInstance.InstanceView.Statuses
                                        where item.Code == "ProvisioningState/deleting"
                                        select item;
                            if (query.Any())
                            {
                                break;
                            }
                            Thread.Sleep(1000);
                            --loops;
                        } while (loops > 0);

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
