using AutoMapper;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSSetCapacityCommand;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListInstancesCommand
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.VMSSListInstancesCommand, VMSSListInstances.Request>();
        }
    }
}
