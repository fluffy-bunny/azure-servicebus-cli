using AutoMapper;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListCommand;
using AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListInstancesCommand;
using Common.Extensions;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;
using static AzureManagementCLI.Features.VirtualMachineScaleSet.Commands;
using static AzureManagementCLI.Features.VirtualMachineScaleSet.VMSSListInstancesCommand.Commands;
using static AzureManagementCLI.Features.When.Commands;

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
        typeof(WhenCommand),
        typeof(VMSSListCommand),
        typeof(VMSSListInstancesCommand)


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
                    services.AddTransient<VMSSListInstances.Request>();

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
