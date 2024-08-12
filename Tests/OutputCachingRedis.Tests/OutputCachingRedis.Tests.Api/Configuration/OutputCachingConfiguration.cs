using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.OutputCaching.Redis.OutputCachingConfiguration", Version = "1.0")]

namespace OutputCachingRedis.Tests.Api.Configuration
{
    public static class OutputCachingConfiguration
    {
        public static IServiceCollection ConfigureOutputCaching(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("OutputCaching").Get<OutputCachingConfig>();

            if (redisConfig == null)
            {
                throw new Exception("Missing 'OutputCaching' configuration in appsettings.json ");
            }
            services.AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = redisConfig.Configuration;
                options.InstanceName = redisConfig.InstanceName;
            });
            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(b => b.NoCache());
                options.AddPolicy("All Default", builder => builder
                    .Expire(TimeSpan.FromSeconds(configuration.GetValue<int?>("OutputCaching:Policies:AllDefault:Duration") ?? options.DefaultExpirationTimeSpan.Seconds)));
                options.AddPolicy("Long Term", builder => builder
                    .Expire(TimeSpan.FromSeconds(configuration.GetValue<int?>("OutputCaching:Policies:LongTerm:Duration") ?? 900)));
                options.AddPolicy("Short Term", builder => builder
                    .Expire(TimeSpan.FromSeconds(configuration.GetValue<int?>("OutputCaching:Policies:ShortTerm:Duration") ?? 60)));
            });
            return services;
        }

        private class OutputCachingConfig
        {
            public string? Configuration { get; set; }
            public string? InstanceName { get; set; }
        }
    }
}