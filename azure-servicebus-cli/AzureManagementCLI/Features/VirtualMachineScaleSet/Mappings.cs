using AutoMapper;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.VMSSListCommand, VMSSList.Request>();
        }
    }
}
