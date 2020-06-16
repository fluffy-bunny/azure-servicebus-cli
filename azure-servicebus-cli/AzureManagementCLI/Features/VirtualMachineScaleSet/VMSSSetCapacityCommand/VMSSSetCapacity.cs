using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSSetCapacityCommand
{
    public static class VMSSSetCapacity
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public string ResourceGroup { get; set; }
            public string ScaleSet { get; set; }
            public string Capacity { get; set; }
            public Request(AzureClient azureClient)
            {
                AzureClient = azureClient;
            }
        }

        public class Response
        {
            public IVirtualMachineScaleSet VirtualMachineScaleSet { get; set; }
            public Exception Exception { get; set; }
        }
        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response{};
                try
                {
                    // GetVirtualMachineScaleSetVMs
                    var rg = await request.AzureClient.AzureInstance.ResourceGroups.GetByNameAsync(request.ResourceGroup);
                    if (rg == null)
                    {
                        throw new Exception($"rg:{request.ResourceGroup} does not exist!");
                    }
                    response.VirtualMachineScaleSet = await request.AzureClient.AzureInstance.GetScaleSetAsync(rg.Name, request.ScaleSet);
                    var capacity = Convert.ToInt32(request.Capacity);
                    var timeSpan = new TimeSpan(0, 0, 5);
                    using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
                    {
                        try
                        {
                            response.VirtualMachineScaleSet = await response.VirtualMachineScaleSet
                                                            .Update()
                                                            .WithCapacity(capacity)
                                                            .ApplyAsync(cancellationTokenSource.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            Console.WriteLine("Task was cancelled");
                        }
                    }
                    int loops = 5;
                    do
                    {
                        await response.VirtualMachineScaleSet.RefreshAsync();
                        if(response.VirtualMachineScaleSet.Capacity == capacity)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                        --loops;
                    } while (loops>0);


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
