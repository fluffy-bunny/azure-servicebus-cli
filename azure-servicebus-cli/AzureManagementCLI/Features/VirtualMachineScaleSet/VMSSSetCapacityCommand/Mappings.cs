using AutoMapper;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSSetCapacityCommand
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.VMSSSetCapacityCommand, VMSSSetCapacity.Request>();
        }
    }
}
