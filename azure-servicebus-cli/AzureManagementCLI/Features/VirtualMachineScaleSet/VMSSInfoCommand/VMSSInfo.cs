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

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSInfoCommand
{
    public static class VMSSInfo
    {
        public class Request : IRequest<Response>
        {
            public AzureClient AzureClient { get; }
            public IAzureManagementApi AzureManagementApi { get; }
            public ISerializer Serializer { get; }
            public string ResourceGroup { get; set; }
            public string ScaleSet { get; set; }
            public Request(AzureClient azureClient, IAzureManagementApi azureManagementApi, ISerializer serializer)
            {
                AzureClient = azureClient;
                AzureManagementApi = azureManagementApi;
                Serializer = serializer;
            }
        }

        public class Response
        {
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
                    var timeSpan = new TimeSpan(0, 0, 5);
                    using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
                    {
                        
                        var r2 = await request.AzureManagementApi.GetVirtualMachineScaleSetInfo(
                            subscriptionId, rg.Name, request.ScaleSet, cancellationTokenSource.Token);
                        response.HttpResponseMessage = r2.HttpResponseMessage;
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
