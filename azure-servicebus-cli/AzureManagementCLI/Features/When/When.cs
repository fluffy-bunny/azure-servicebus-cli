using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementCLI.Features.When
{
    public static class When
    {
        public class Query : IRequest<Response>
        {
        }

        public class Response
        {
            public DateTime CurrentTime { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            public Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Response { CurrentTime = DateTime.UtcNow });
            }
        }
    }
}
