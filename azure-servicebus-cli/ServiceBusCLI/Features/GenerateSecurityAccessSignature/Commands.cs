using AutoMapper;
using Common;
using Contracts;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.GenerateSecurityAccessSignature
{
    class SecurityAccessSignatureHandle
    {
        public string SecurityAccessSignature { get; set; }
    }
    /*
     *       "commandLineArgs": "generate-sas -k 8u4ZWemBetr**REDACTED**b/7IEOP3/c= -p RootManageSharedAccessKey -e 3600"
     */

    public static class Commands
    {
        [Command("generate-sas", Description = "generate a SecurityAccessSignature")]
        public class GenerateSecurityAccessSignatureCommand
        {
            [Option("-k|--key", CommandOptionType.SingleValue, Description = "ServiceBus Secret Key, i.e. 8u4ZWemBetr9WdRDsnGpBjbC79Vk0MTRLb/7IEOP3/c=")]
            public string Key { get; set; }

            [Option("-p|--policy", CommandOptionType.SingleValue, Description = "i.e. RootManageSharedAccessKey")]
            public string Policy { get; set; }

            [Option("-e|--expiry", CommandOptionType.SingleValue, Description = "in seconds")]
            public int ExpirySeconds { get; set; }

            [Option("-s|--session", CommandOptionType.NoValue, Description = "save SAS in local session storage")]
            public bool Session { get; } = false;

            private async Task OnExecuteAsync(
                IMediator mediator, 
                IMapper mapper, 
                IConsole console,
                ISerializer serializer,
                GenerateSecurityAccessSignature.Request request)
            {
                var command = mapper.Map(this, request);

                var response = await mediator.Send(command);
                var handle = new SecurityAccessSignatureHandle
                {
                    SecurityAccessSignature = response.Result
                };
                var json = handle.ToJson(true);

                Console.WriteLine(json);

            }
        }
    }
}
