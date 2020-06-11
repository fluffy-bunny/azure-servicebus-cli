using Contracts;
using MediatR;
using Microsoft.Azure.ServiceBus;
using ServiceBusCLI.Features.GenerateSecurityAccessSignature;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using ServiceBusCLI.Utils;
using Microsoft.Azure.ServiceBus.Core;

namespace ServiceBusCLI.Features.MessageReceiver
{
    public static class Peek
    {
        public class Request : IRequest<Response>
        {
            public IMessageReceiver MessageReceiver { get; }

            public Request(IMessageReceiverAccessor messageReceiverAccessor)
            {
                MessageReceiver = messageReceiverAccessor.MessageReceiver;
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
                    var message = await request.MessageReceiver.PeekAsync();
                    return await RespondWith(message,null);
                }
                catch (Exception ex)
                {
                    return await RespondWith(null, ex);
                }

            }

            private Task<Response> RespondWith(Message result,Exception ex)
            {
                return Task.FromResult(new Response { Result = result , Exception = ex});
            }
        }
    }
}
