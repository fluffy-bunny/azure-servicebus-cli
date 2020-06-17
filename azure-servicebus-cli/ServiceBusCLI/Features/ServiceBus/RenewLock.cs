using AutoMapper;
using Common;
using MediatR;
using ServiceBusCLI.Utils;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.ServiceBus
{
    public static class RenewLock
    {
        public class Request : IRequest<Response>
        {
            public IServiceBusQueueUtils ServiceBusQueueUtils { get; }
            public IMapper Mapper { get; }
            public ISerializer Serializer { get; }

            public Request(
                IServiceBusQueueUtils serviceBusQueueUtils,
                IMapper mapper,
                ISerializer serializer)
            {
                ServiceBusQueueUtils = serviceBusQueueUtils;
                Mapper = mapper;
                Serializer = serializer;
            }
            public string MessageId { get; set; }
            public string LockToken { get; set; }
        }
        public class Response
        {
            public HttpResponseMessage Result { get; set; }
            public Exception Exception { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var response = await request.ServiceBusQueueUtils.RenewLockAsync(request.MessageId, request.LockToken);
                    return await RespondWith(response, null);
                }
                catch (Exception ex)
                {
                    return await RespondWith(null, ex);
                }
            }

 

            private Task<Response> RespondWith(HttpResponseMessage response, Exception ex)
            {
                return Task.FromResult(new Response { Result = response, Exception = ex});
            }
        }
    }
}
