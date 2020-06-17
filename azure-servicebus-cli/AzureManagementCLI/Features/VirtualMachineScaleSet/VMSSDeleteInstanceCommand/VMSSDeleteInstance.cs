using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using System.Net.Http;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSDeleteInstanceCommand
{
    public static class VMSSDeleteInstance
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public IAzureManagementApi AzureManagementApi { get; }
            public ISerializer Serializer { get; }
            public string ResourceGroup { get; set; }
            public string ScaleSet { get; set; }
            public string InstanceIds { get; set; }
            public Request(AzureClient azureClient, IAzureManagementApi azureManagementApi, ISerializer serializer)
            {
                AzureClient = azureClient;
                AzureManagementApi = azureManagementApi;
                Serializer = serializer;
            }
        }

        public class Response
        {
            public IVirtualMachineScaleSet VirtualMachineScaleSet { get; set; }
            public Exception Exception { get; set; }
            public HttpResponseMessage HttpResponseMessage { get; internal set; }
        }
        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response { };
                try
                {
                    var subscriptionId = request.AzureClient.AzureInstance.SubscriptionId;
                    // GetVirtualMachineScaleSetVMs
                    var rg = await request.AzureClient.AzureInstance.ResourceGroups.GetByNameAsync(request.ResourceGroup);
                    if (rg == null)
                    {
                        throw new Exception($"rg:{request.ResourceGroup} does not exist!");
                    }
                    response.VirtualMachineScaleSet = await request.AzureClient.AzureInstance.GetScaleSetAsync(rg.Name, request.ScaleSet);
                    if (response.VirtualMachineScaleSet == null)
                    {
                        throw new Exception($"rg:{request.ResourceGroup} ScaleSet:{request.ScaleSet} does not exist!");
                    }

                    var ids = request.Serializer.Deserialize<List<string>>(request.InstanceIds);

                    var timeSpan = new TimeSpan(0, 0, 5);
                    int loops = 5;
                    do
                    {
                        using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
                        {
                            response.HttpResponseMessage = await request.AzureManagementApi.DeleteVirtualMachineScaleSetVM(subscriptionId, rg.Name, response.VirtualMachineScaleSet.Name, ids, cancellationTokenSource.Token);
                            if (response.HttpResponseMessage.IsSuccessStatusCode)
                            {
                                break;
                            }
                        }

                        Thread.Sleep(1000);
                        --loops;
                    } while (loops > 0);
                    /*
                    using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
                    {
                        var response2 = await request.AzureManagementApi.DeleteVirtualMachineScaleSetVM(subscriptionId, rg.Name, response.VirtualMachineScaleSet.Name, request.InstanceId, cancellationTokenSource.Token);
                        try
                        {
                            await vmInstance.DeleteAsync(cancellationTokenSource.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            Console.WriteLine("Task was cancelled");
                        }
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
*/
                    await response.VirtualMachineScaleSet.RefreshAsync();
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
