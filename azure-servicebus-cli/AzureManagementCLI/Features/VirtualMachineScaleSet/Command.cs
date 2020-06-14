using AutoMapper;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.Text;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet
{
    public static class Commands
    {
        [Command("vmss-list", Description = "List all vmss in the subscription")]
        public class VMSSListCommand
        {
            [Option("-g|--resource-group", CommandOptionType.SingleValue, Description = "The resource group name")]
            public string ResourceGroup { get; set; }
            private async Task OnExecuteAsync(
                IConsole console,
                IMediator mediator,
                IMapper mapper,
                VMSSList.Request request)
            {
                var command = mapper.Map(this, request);
                var response = await mediator.Send(command);
                if (response.Exception != null)
                {
                    console.WriteLine($"{response.Exception.Message}");
                }
                else
                {
                    foreach (var item in response.Result)
                    {
                        console.WriteLine($"VMSS: {item.Name}\n Id: {item.Id}\n Capacity: {item.Capacity}");
                    }
                }
            }
        }
    }
}
