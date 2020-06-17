using Contracts;
using MediatR;
using Microsoft.Azure.ServiceBus;
using ServiceBusCLI.Features.GenerateSecurityAccessSignature;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using ServiceBusCLI.Utils;
using Common;

namespace ServiceBusCLI.Features.SendMessage
{

    public class SendMessage<T> where T : class
    {
        public class Response
        {
            public Request Result { get; set; }
            public Exception Exception { get; set; }
        }
        public class Request : IRequest<Response>
        {
            public QueueClient QueueClient { get; internal set; }
            public ISerializer Serializer { get; private set; }
            public T Message { get; internal set; }

            public Request(QueueClient queueClient, ISerializer serializer)
            {
                QueueClient = queueClient;
                Serializer = serializer;
            }
        }
        public class Handler : IRequestHandler<Request, Response>  
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var json = request.Message.ToJson();
                    byte[] byteMessage = Encoding.UTF8.GetBytes(json);
                    await request.QueueClient.SendAsync(new Message(byteMessage));
                    return await RespondWith(request, null);
                }
                catch (Exception ex)
                {
                    return await RespondWith(null, ex);
                }

            }

            private Task<Response> RespondWith(Request result, Exception ex)
            {
                return Task.FromResult(new Response { Result = result, Exception = ex });
            }
        }
    }
}
