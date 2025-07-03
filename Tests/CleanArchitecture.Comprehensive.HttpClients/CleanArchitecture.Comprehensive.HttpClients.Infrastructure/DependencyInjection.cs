using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Caching;
using CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDistributedMemoryCache();
            services.AddSingleton<IDistributedCacheWithUnitOfWork, DistributedCacheWithUnitOfWork>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}