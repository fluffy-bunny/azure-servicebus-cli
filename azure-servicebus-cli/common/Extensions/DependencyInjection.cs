using Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommon(this IServiceCollection services)
        {
            services.AddSerializer();
            services.AddBase64Encoder();
            services.AddTransient(typeof(AppSettings<>), typeof(AppSettings<>));
            return services;
        }
        public static IServiceCollection AddSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer, Serializer>();
            return services;
        }
        public static IServiceCollection AddBase64Encoder(this IServiceCollection services)
        {
            services.AddSingleton<IBase64Encoder, Base64Encoder>();
            return services;
        }
        public static IServiceCollection AddAzureClient(this IServiceCollection services)
        {
            services.AddSingleton<AzureClient>(sp =>
            {
                using (new DisposableStopwatch(t => Utilities.Log($"AzureUtils.FetchAzureClient() - {t} elapsed")))
                {
                    return AzureUtils.FetchAzureClient();
                }
            });
            return services;
        }
    }
}
