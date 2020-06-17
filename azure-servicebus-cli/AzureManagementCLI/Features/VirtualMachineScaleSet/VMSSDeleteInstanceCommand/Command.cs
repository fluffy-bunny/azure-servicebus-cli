using AutoMapper;
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

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSDeleteInstanceCommand
{
    public static class Commands
    {
        [Command("vmss-instance-delete", Description = "Delete an VMSS VM instance")]
        public class VMSSDeleteInstanceCommand
        {
            [Option("-g|--resource-group", CommandOptionType.SingleValue, Description = "The resource group name")]
            public string ResourceGroup { get; set; }

            [Option("-v|--vmss-name", CommandOptionType.SingleValue, Description = "The VirtualMachineScalseSet name")]
            public string ScaleSet { get; set; }

            [Option("-i|--instance-id", CommandOptionType.SingleValue, Description = "The VirtualMachineScalseSet VM Instance ID, i.e. --instance-id ['193','194']")]
            public string InstanceIds { get; set; }

            private async Task OnExecuteAsync(
                IConsole console,
                IMediator mediator,
                ISerializer serializer,
                IMapper mapper,
                VMSSDeleteInstance.Request request)
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
                        console.WriteLine(@$"VMSS: {response.VirtualMachineScaleSet.Name} 
    Id: {response.VirtualMachineScaleSet.Id} 
    Capacity: {response.VirtualMachineScaleSet.Capacity}");

                        console.WriteLine(response.HttpResponseMessage.PrettyJson(serializer));
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
                if (string.IsNullOrWhiteSpace(InstanceIds))
                {
                    error = true;
                    sb.Append($"--instance-id is missing\n");
                }
                else
                {
                    try
                    {
                        InstanceIds = InstanceIds.Replace('\'', '"');
                        var ids = serializer.Deserialize<List<string>>(InstanceIds);
                        foreach(var id in ids)
                        {
                            Convert.ToInt32(id);
                        }
                    }
                    catch(Exception ex)
                    {
                        error = true;
                        sb.Append($"--instance-id is bad ->{InstanceIds}\n");
                        sb.Append(ex.Message);
                    }

                }
                if (error)
                {
                    throw new Exception(sb.ToString());
                }
            }
        }
    }
}
