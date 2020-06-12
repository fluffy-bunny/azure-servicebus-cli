using AutoMapper;
using Contracts;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Azure.ServiceBus;
using ServiceBusCLI.Features.SendMessage;
using ServiceBusCLI.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.SendJob
{
    public static class Commands
    {
        [Command("send-job", Description = "send a job message")]
        public class SendJobCommand
        {
            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper, 
                IConsole console,
                SendMessage<Job>.Request request)
            {
                
                var command = mapper.Map(this, request);
                var job = new Job
                {
                    Id = Guid.NewGuid().ToString(),
                    IssuedTime = DateTime.UtcNow,
                    Name = "My SuperDuper Job"
                };
              //  var json = serializer.Serialize(job);
                command.Message = job;

                var response = await mediator.Send(command);
                
                var json = response.ToJson(true);

                Console.WriteLine(json);
            }
        }
    }
}
