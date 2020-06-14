
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MediatR;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Contracts;
using ServiceBusCLI.Features.SendJob;
using ServiceBusCLI.Features.GenerateSecurityAccessSignature;
using ServiceBusCLI.Features.ServiceBus;
using ServiceBusCLI.Utils;
using Microsoft.Azure.ServiceBus.Core;
using ServiceBusCLI.Features.MessageReceiver;
using ServiceBusCLI.Features.SendMessage;
using Common;
using Common.Extensions;

namespace ServiceBusCLI
{
    [Command(Name = "ServiceBusCLI",
           Description = @"   
   ____                 _            ___               _____ __    ____
  / __/___  ____ _  __ (_)____ ___  / _ ) __ __ ___   / ___// /   /  _/
 _\ \ / -_)/ __/| |/ // // __// -_)/ _  |/ // /(_-<  / /__ / /__ _/ /  
/___/ \__//_/   |___//_/ \__/ \__//____/ \_,_//___/  \___//____//___/  

This is an Azure Managed Identity Application, so you must first login to azure;

          az login


Set you queue settings;

          ServiceBusCLI service-bus-settings -n <namespace> -q <queue>")]
    [HelpOption]
    [VersionOptionFromMember(MemberName = "GetVersion")]
    [Subcommand(
        typeof(Features.When.Commands.WhenCommand),
        typeof(Features.GenerateSecurityAccessSignature.Commands.GenerateSecurityAccessSignatureCommand),
        typeof(Features.MessageReceiver.Commands.PeekCommand),
        typeof(Features.MessageReceiver.Commands.ReceiveCommand),
        typeof(Features.SendJob.Commands.SendJobCommand),
        typeof(Features.ServiceBus.Commands.ServiceBusSettingsCommand),
        typeof(Features.ServiceBus.Commands.ServiceBusRenewLockCommand)
        )
       ]
    internal class Program
    {
        //    static public string Namespace = "scalesets";
        //    static public string Queue = "w10rs5pr0";

        private static async Task Main(string[] args)
        {
            try
            {

            await CreateHostBuilder()
                .RunCommandLineApplicationAsync<Program>(args);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddMediatR(typeof(Program).Assembly)
                        .AddAutoMapper(typeof(Program).Assembly);
                    services.AddSingleton<QueueClient>(sp =>
                    {
                        var settings = sp.GetRequiredService(typeof(ISessionSettings)) as ISessionSettings;
                        var queueUri = $"sb://{settings.Namespace}.servicebus.windows.net/";
                        var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
                        var queueClient = new QueueClient(queueUri, settings.Queue, tokenProvider);
                        return queueClient;
                    });

                    services.AddSingleton<IMessageReceiver>(sp =>
                    {
                        var settings = sp.GetRequiredService(typeof(ISessionSettings)) as ISessionSettings;
                        var queueUri = $"sb://{settings.Namespace}.servicebus.windows.net/";
                        var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
                        var messageReciever = new MessageReceiver(queueUri, settings.Queue, tokenProvider);
                        return messageReciever;
                    });
                    services.AddSingleton<IServiceBusQueueUtils, ServiceBusQueueUtils>();
                    services.AddSingleton<ISessionSettings, SessionSettings>();
                    services.AddSingleton<ISecurityAccessSignatureProvider, SecurityAccessSignatureProvider>();


                    //                    services.AddTransient<SendJob.Request>();
                    //                    services.AddTransient(typeof(SendJob.Request<>), typeof(SendJob.Request<>));
                    services.AddTransient(typeof(SendMessage<>), typeof(SendMessage<>));
                    services.AddTransient(typeof(SendMessage<>.Request), typeof(SendMessage<>.Request));
                    services.AddTransient(typeof(SendMessage<>.Response), typeof(SendMessage<>.Response));
                    services.AddTransient<GenerateSecurityAccessSignature.Request>();
                    services.AddTransient<ServiceBusSettings.Request>();
                    services.AddTransient<Peek.Request>();
                    services.AddTransient<Receive.Request>();
                    services.AddTransient<RenewLock.Request>();


                    services.AddCommon();



                });
        }
        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify a command");
            app.ShowHelp();
            return 1;
        }

        private string GetVersion()
        {
            return typeof(Program).Assembly
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
        }
    }
}
