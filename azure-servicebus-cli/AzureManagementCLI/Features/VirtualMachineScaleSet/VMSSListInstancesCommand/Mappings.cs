using AutoMapper;

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
