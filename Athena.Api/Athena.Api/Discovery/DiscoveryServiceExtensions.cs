using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Athena.Api.Discovery
{
    public static class DiscoveryServiceExtensions
    {
        public static void AddDiscoveryRegistrer(this IServiceCollection services)
        {
            services.AddSingleton<IDiscoveryRegisterer, DiscoveryRegisterer>();
        }
    }
}
