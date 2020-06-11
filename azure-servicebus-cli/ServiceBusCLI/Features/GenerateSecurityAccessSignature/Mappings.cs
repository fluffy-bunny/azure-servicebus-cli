using AutoMapper;

namespace ServiceBusCLI.Features.GenerateSecurityAccessSignature
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.GenerateSecurityAccessSignatureCommand, GenerateSecurityAccessSignature.Request>();
        }
    }
}
