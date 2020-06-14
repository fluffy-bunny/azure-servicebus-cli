using AutoMapper;
using AzureManagementCLI.Features.VirtualMachineScaleSet;
using Common.Extensions;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureManagementCLI
{
    [Command(
        Name = "AzureManagementCLI",
        Description = @"AzureManagementCLI

Please set the following Azure environment variables:
ARM_CLIENT_ID
ARM_CLIENT_SECRET
ARM_SUBSCRIPTION_ID
ARM_TENANT_ID

")]
    [HelpOption]
    [VersionOptionFromMember(MemberName = "GetVersion")]
    [Subcommand(
        typeof(Features.When.Commands.WhenCommand),
        typeof(Features.VirtualMachineScaleSet.Commands.VMSSListCommand)

        )
       ]
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {

                await CreateHostBuilder()
                    .RunCommandLineApplicationAsync<Program>(args);
            }
            catch (Exception ex)
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
                    services.AddAzureClient();
                    services.AddCommon();

                    services.AddTransient<VMSSList.Request>();

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
