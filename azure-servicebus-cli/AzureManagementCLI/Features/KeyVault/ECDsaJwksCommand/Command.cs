using AutoMapper;
using Common;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.KeyVault.ECDsaJwksCommand
{
    public static class Commands
    {
        [Command("ecdsa-jwks", Description = "create a jwks discovery document")]
        public class ECDsaJwksCommand
        {
            [Option("-v|--vault", CommandOptionType.SingleValue, Description = "The name of the keyvault")]
            public string KeyVaultName { get; set; }

            [Option("-k|--key", CommandOptionType.SingleValue, Description = "The key name")]
            public string KeyName { get; set; }

            private async Task OnExecuteAsync(
                IConsole console,
                IMediator mediator,
                IMapper mapper,
                ISerializer serializer,
                ECDsaJwks.Request request)
            {
                Validate();
                using (new DisposableStopwatch(t => Utilities.Log($"ECDsaJwksCommand - {t} elapsed")))
                {
                    var command = mapper.Map(this, request);
                    var response = await mediator.Send(command);
                    if (response.Exception != null)
                    {
                        console.WriteLine($"{response.Exception.Message}");
                    }
                    else
                    {
                        var json = serializer.Serialize(response.Result, indent: true);
                        console.WriteLine(json);
                    }
                }
            }
            private void Validate()
            {
                StringBuilder sb = new StringBuilder();
                bool error = false;
                if (string.IsNullOrWhiteSpace(KeyVaultName))
                {
                    error = true;
                    sb.Append($"--vault is missing\n");
                }
                if (string.IsNullOrWhiteSpace(KeyName))
                {
                    error = true;
                    sb.Append($"--key is missing\n");
                }

                if (error)
                {
                    throw new Exception(sb.ToString());
                }
            }
        }
    }
}
