using AutoMapper;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListCommand
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.VMSSListCommand, VMSSList.Request>();
        }
    }
}
