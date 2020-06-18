using AutoMapper;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSDeleteInstanceCommand;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListCommand;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListInstancesCommand;
using Common;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSInfoCommand
{
    public static class Commands
    {
        [Command("vmss-info", Description = "Get VMSS Information")]
        public class VMSSInfoCommand
        {
            [Option("-g|--resource-group", CommandOptionType.SingleValue, Description = "The resource group name")]
            public string ResourceGroup { get; set; }

            [Option("-v|--vmss-name", CommandOptionType.SingleValue, Description = "The VirtualMachineScalseSet name")]
            public string ScaleSet { get; set; }

            private async Task OnExecuteAsync(
                IConsole console,
                IMediator mediator,
                ISerializer serializer,
                IMapper mapper,
                VMSSInfo.Request request)
            {
                using (new DisposableStopwatch(t => Utilities.Log($"VMSSDeleteInstanceCommand - {t} elapsed")))
                {
                    Validate(serializer);
                    var command = mapper.Map(this, request);
                    var response = await mediator.Send(command);
                    if (response.Exception != null)
                    {
                        console.WriteLine($"{response.Exception.Message}");
                    }
                    else
                    {
                        console.WriteLine(await response.HttpResponseMessage.PrettyJsonAsync(serializer));
                    }
                }
            }

            private void Validate(ISerializer serializer)
            {
                StringBuilder sb = new StringBuilder();
                bool error = false;
                if (string.IsNullOrWhiteSpace(ResourceGroup))
                {
                    error = true;
                    sb.Append($"--resource-group is missing\n");
                }
                if (string.IsNullOrWhiteSpace(ScaleSet))
                {
                    error = true;
                    sb.Append($"--vmss-name is missing\n");
                }
            
                if (error)
                {
                    throw new Exception(sb.ToString());
                }
            }
        }
    }
}
