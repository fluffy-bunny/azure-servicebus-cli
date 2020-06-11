using AutoMapper;

namespace ServiceBusCLI.Features.MessageReceiver
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.PeekCommand, Peek.Request>();
            CreateMap<Commands.ReceiveCommand, Receive.Request>();
        }
    }
}
