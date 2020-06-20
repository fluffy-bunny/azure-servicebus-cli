using AutoMapper;

namespace AzureManagementCLI.Features.KeyVault.ECDsaJwksCommand
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.ECDsaJwksCommand, ECDsaJwks.Request>();
        }
    }
}
