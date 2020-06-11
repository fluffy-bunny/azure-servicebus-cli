using AutoMapper;

namespace ServiceBusCLI.Features.ServiceBus
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.ServiceBusRenewLockCommand, RenewLock.Request>();
            CreateMap<Commands.ServiceBusSettingsCommand, ServiceBusSettings.Request>();
            CreateMap<ServiceBusSettings.Request, ServiceBusSettings.Settings>();

        }
    }
}
