using AutoMapper;
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
        Description = @"AzureManagementCLI")]
    [HelpOption]
    [VersionOptionFromMember(MemberName = "GetVersion")]
    [Subcommand(
        typeof(Features.When.Commands.WhenCommand)
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
