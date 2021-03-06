﻿using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaviniaApi.Extensions
{
    public static class ServicesExtensions
    {

        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.TryAddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.TryAddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.TryAddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
