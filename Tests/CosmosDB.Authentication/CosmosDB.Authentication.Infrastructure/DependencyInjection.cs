using CosmosDB.Authentication.Domain.Common.Interfaces;
using CosmosDB.Authentication.Domain.Repositories;
using CosmosDB.Authentication.Infrastructure.Configuration;
using CosmosDB.Authentication.Infrastructure.Persistence;
using CosmosDB.Authentication.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CosmosDB.Authentication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureCosmosRepository(configuration);
            services.AddScoped<IProductRepository, ProductCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            return services;
        }
    }
}