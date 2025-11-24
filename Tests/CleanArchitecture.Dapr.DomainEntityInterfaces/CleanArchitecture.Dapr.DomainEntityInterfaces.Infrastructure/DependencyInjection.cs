using CleanArchitecture.Dapr.DomainEntityInterfaces.Application.Common.Eventing;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Common.Interfaces;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Repositories;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure.Configuration;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure.Eventing;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure.Persistence;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClientRepository, ClientDaprStateStoreRepository>();
            services.AddScoped<DaprStateStoreUnitOfWork>();
            services.AddScoped<IDaprStateStoreUnitOfWork>(provider => provider.GetRequiredService<DaprStateStoreUnitOfWork>());
            services.AddScoped<IDaprStateStoreGenericRepository, DaprStateStoreGenericRepository>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}