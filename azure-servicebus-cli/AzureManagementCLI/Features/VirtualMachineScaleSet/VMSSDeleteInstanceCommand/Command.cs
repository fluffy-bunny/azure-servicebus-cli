using AutoMapper;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListCommand;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListInstancesCommand;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using System;
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

            [Option("-i|--instance-id", CommandOptionType.SingleValue, Description = "The VirtualMachineScalseSet VM Instance ID")]
            public string InstanceId { get; set; }

            private async Task OnExecuteAsync(
                IConsole console,
                IMediator mediator,
                IMapper mapper,
                VMSSDeleteInstance.Request request)
            {
                Validate();
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

                    console.WriteLine($"Deleted: VMSS: {response.FullVMInstanceId} ");
                }
            }

            private void Validate()
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
                if (string.IsNullOrWhiteSpace(InstanceId))
                {
                    error = true;
                    sb.Append($"--instance-id is missing\n");
                }
                else
                {
                    try
                    {
                        Convert.ToInt32(InstanceId);
                    }
                    catch(Exception ex)
                    {
                        sb = new StringBuilder();
                        sb.Append("--instance-id is bad\n");
                        sb.Append(ex.Message);
                        throw new Exception(sb.ToString(), ex);
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
