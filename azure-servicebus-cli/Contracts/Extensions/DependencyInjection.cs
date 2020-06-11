using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Extensions
{
    public static class DependencyInjection
    {
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

    }
}
