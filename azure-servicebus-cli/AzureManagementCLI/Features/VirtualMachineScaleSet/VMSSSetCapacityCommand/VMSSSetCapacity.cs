using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSSetCapacityCommand
{
    public static class VMSSSetCapacity
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public IAzureManagementApi AzureManagementApi { get; }
            public ISerializer Serializer { get; }
            public string ResourceGroup { get; set; }
            public string ScaleSet { get; set; }
            public string Capacity { get; set; }
            public Request(AzureClient azureClient,IAzureManagementApi azureManagementApi, ISerializer serializer)
            {
                AzureClient = azureClient;
                AzureManagementApi = azureManagementApi;
                Serializer = serializer;
            }
        }

        public class Response
        {
            public HttpResponseMessage HttpResponseMessage { get; internal set; }
            public Exception Exception { get; set; }
        }
        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response{};
                try
                {
                    var subscriptionId = request.AzureClient.AzureInstance.SubscriptionId;
                    var capacity = Convert.ToInt32(request.Capacity);
                    var timeSpan = new TimeSpan(0, 0, 5);
                    using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
                    {
                        try
                        {
                            response.HttpResponseMessage = await request.AzureManagementApi.SetVirtualMachineScaleSetCapacity(subscriptionId,
                                request.ResourceGroup, request.ScaleSet, capacity, cancellationTokenSource.Token);
                        }
                        catch (TaskCanceledException tex)
                        {
                            response.Exception = tex;
                            Console.WriteLine("Task was cancelled");
                        }
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
