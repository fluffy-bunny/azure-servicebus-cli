using AutoMapper;
using Common;
using Contracts;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceBusCLI.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.MessageReceiver
{
    class JobMessage
    {
        public Message Message { get; set; }
        public Job Job { get; set; }
    }

    public static class Commands
    {

        [Command("receive", Description = "Receive Message")]
        public class ReceiveCommand
        {
            [Option("-c|--complete", CommandOptionType.NoValue, Description = "complete the message")]
            public bool Complete { get; } = false;
            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper,
                IConsole console,
                ISerializer serializer,
                Receive.Request request)
            {
               
                var command = mapper.Map(this, request);
                console.WriteLine($"Sending Request....");
                var response = await mediator.Send(command);
                if (response.Exception != null)
                {
                    console.WriteLine($"{response.Exception.Message}");
                }
                else
                {

                    if (response.Result == null)
                    {
                        console.WriteLine($"Success: No Messages Available......");
                    }
                    else
                    {
                        try
                        {
                            var json = Encoding.UTF8.GetString(response.Result.Body);
                            var job = serializer.Deserialize<Job>(json);
                          //  json = job.ToJson();

                            var jobMessage = new JobMessage
                            {
                                Job = job,
                                Message = response.Result
                            };
                            json = jobMessage.ToJson(true);

                            Console.WriteLine(json);
                        }
                        catch (Exception ex)
                        {
                            console.WriteLine($"Success: Got message......but failed to Deserialize\n{ex.Message}");
                        }
                    }
                }
            }
        }

        [Command("peek", Description = "peek")]
        public class PeekCommand
        {
            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper,
                IConsole console,
                ISerializer serializer,
                Peek.Request request)
            {
                var command = mapper.Map(this, request);
                console.WriteLine($"Sending Request....");
                var response = await mediator.Send(command);
                if(response.Exception != null)
                {
                    console.WriteLine($"{response.Exception.Message}");
                }
                else
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(response.Result.Body);
                        var job = serializer.Deserialize<Job>(json);
                      //  json = job.ToJson();

                        var jobMessage = new JobMessage
                        {
                            Job = job,
                            Message = response.Result
                        };
                        json = jobMessage.ToJson(true);

                        Console.WriteLine(json);
                    }
                    catch (Exception ex)
                    {
                        console.WriteLine($"Success: peeked message......but failed to Deserialize\n{ex.Message}");
                    }
                }
            }
        }
    }
}
