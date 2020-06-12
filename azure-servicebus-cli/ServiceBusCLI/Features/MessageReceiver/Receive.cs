using MediatR;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusCLI.Utils;
using Microsoft.Azure.ServiceBus.Core;

namespace ServiceBusCLI.Features.MessageReceiver
{
    public static class Receive
    {
        public class Request : IRequest<Response>
        {
            public bool Complete { get; set; }
            public IMessageReceiver MessageReceiver { get; }

            public Request(IMessageReceiver messageReceiver)
            {
                MessageReceiver = messageReceiver;
            }
        }

        public class Response
        {
            public Message Result { get; set; }
            public Exception Exception { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var message = await request.MessageReceiver.ReceiveAsync(new TimeSpan(0,0,5)); // 5 seconds.
                    if (request.Complete)
                    {
                        await request.MessageReceiver.CompleteAsync(message.SystemProperties.LockToken);
                    }
                    return await RespondWith(message, null);
                }
                catch (Exception ex)
                {
                    return await RespondWith(null, ex);
                }

            }

            private Task<Response> RespondWith(Message result, Exception ex)
            {
                return Task.FromResult(new Response { Result = result, Exception = ex });
            }
        }
    }
}
