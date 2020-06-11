using AutoMapper;
using Contracts;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.ServiceBus
{
    public static class Commands
    {
        [Command("renew-lock", Description = "renew a lock on a service-bus message")]
        public class ServiceBusRenewLockCommand
        {
            [Option("-m|--message-id", CommandOptionType.SingleValue, Description = "The Message Id")]
            public string MessageId { get; set; }

            [Option("-l|--lock-token", CommandOptionType.SingleValue, Description = "The Lock Token")]
            public string LockToken { get; set; }

            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper,
                IConsole console,
                RenewLock.Request request)
            {
                var command = mapper.Map(this, request);

                var response = await mediator.Send(command);

                var json = response.ToJson(true);

                console.WriteLine(json);

            }

        }

        [Command("service-bus-settings", Description = "set service-bus queue settings for the cli")]
        public class ServiceBusSettingsCommand
        {
            [Option("-n|--namespace", CommandOptionType.SingleValue, Description = "ServiceBus Namespace")]
            public string Namespace { get; set; }

            [Option("-q|--queue", CommandOptionType.SingleValue, Description = "Queue Name")]
            public string Queue { get; set; }

            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper,
                IConsole console,
                ServiceBusSettings.Request request)
            {
                var command = mapper.Map(this, request);

                var response = await mediator.Send(command);

                console.WriteLine($"{response.Result}");
            }
        }
    }
}
