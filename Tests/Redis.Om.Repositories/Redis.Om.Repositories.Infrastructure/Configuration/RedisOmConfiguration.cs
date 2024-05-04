using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.OM;
using Redis.Om.Repositories.Infrastructure.Services;
using StackExchange.Redis;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmConfiguration", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Configuration
{
    public static class RedisOmConfiguration
    {
        public static IServiceCollection ConfigureRedisOm(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp => new RedisConnectionProvider(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisOmConnectionString")!)));
            services.AddHostedService<IndexCreationService>();
            return services;
        }
    }
}