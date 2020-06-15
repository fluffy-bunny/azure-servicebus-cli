using AutoMapper;
using Common;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListInstancesCommand
{
    public static class Commands
    {
        [Command("vmss-instances", Description = "List all instances in a VitualMachineScaleSet")]
        public class VMSSListInstancesCommand
        {
            [Option("-g|--resource-group", CommandOptionType.SingleValue, Description = "The resource group name")]
            public string ResourceGroup { get; set; }

            [Option("-v|--vmss-name", CommandOptionType.SingleValue, Description = "The VirtualMachineScalseSet name")]
            public string ScaleSet { get; set; }
            private async Task OnExecuteAsync(
                IConsole console,
                IMediator mediator,
                IMapper mapper,
                VMSSListInstances.Request request)
            {
                using (new DisposableStopwatch(t => Utilities.Log($"VMSSListInstancesCommand - {t} elapsed")))
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

                        var vms = response.VirtualMachineScaleSetVMs;
                        foreach (var item in vms)
                        {
                            console.WriteLine($"====================\nVMSS: {item.Name}\n  Id: {item.Id}\n  InstanceId: {item.InstanceId}\n  ComputerName:{item.ComputerName}");

                            foreach(var network in item.ListNetworkInterfaces())
                            {
                                console.WriteLine($"  Network:.......\n     PrimaryPrivateIP: {network.PrimaryPrivateIP} ");
                            }
                            var networkId = item.ListNetworkInterfaces();
                        }
                    }
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
                if (error)
                {
                    throw new Exception(sb.ToString());
                }
            }
        }
    }
}
