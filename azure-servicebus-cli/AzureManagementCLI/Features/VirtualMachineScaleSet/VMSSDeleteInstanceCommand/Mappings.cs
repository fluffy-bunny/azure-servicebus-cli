using AutoMapper;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSDeleteInstanceCommand
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.VMSSDeleteInstanceCommand, VMSSDeleteInstance.Request>();
        }
    }
}
