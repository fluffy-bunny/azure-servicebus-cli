using Common;
using MediatR;
using Microsoft.Azure.Management.Compute.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.KeyVault.ECDsaJwksCommand
{
    public static class ECDsaJwks
    {
        public class Request : IRequest<Response>
        {

            public string KeyVaultName { get; set; }

            public string KeyName { get; set; }
            public IAzureKeyVaultServices AzureKeyVaultServices { get; }

            public Request(IAzureKeyVaultServices azureKeyVaultServices)
            {
                AzureKeyVaultServices = azureKeyVaultServices;
            }
        }

        public class Response
        {
            public object Result { get; set; }
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
                    var res = await request.AzureKeyVaultServices.GetECDsaJwksDiscoveryDocumentAsync(request.KeyVaultName, request.KeyName);
                    response.Result = res;
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
