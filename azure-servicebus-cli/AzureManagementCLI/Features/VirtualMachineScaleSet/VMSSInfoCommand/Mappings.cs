using AutoMapper;

namespace AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSInfoCommand
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.VMSSInfoCommand,VMSSInfo.Request>();
        }
    }
}
