using AutoMapper;
using Common;
using MediatR;
using ServiceBusCLI.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.ServiceBus
{
    public static class ServiceBusSettings
    {
        public static string SettingsFileName = "service-bus-queue-settings.json";
        public class Request : IRequest<Response>
        {
            public AppSettings<Settings> AppSettings { get; }

            public IMapper Mapper { get; }
            public ISerializer Serializer { get; }

            public Request(AppSettings<Settings> appsettings, IMapper mapper,ISerializer serializer)
            {
                AppSettings = appsettings;
                Mapper = mapper;
                Serializer = serializer;
            }
            public string Namespace { get; set; }
            public string Queue { get; set; }
        }
        public class Settings
        {
            public string Namespace { get; set; }
            public string Queue { get; set; }

        }
        public class Response
        {
            public string Result { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var settings = request.Mapper.Map<Settings>(request);
                request.AppSettings.Save(settings, ServiceBusSettings.SettingsFileName);
                settings = request.AppSettings.Load(ServiceBusSettings.SettingsFileName);
                return RespondWith(request.Serializer.Serialize(settings));
            }

            private Task<Response> RespondWith(string result)
            {
                return Task.FromResult(new Response { Result = result });
            }
        }
    }
}
