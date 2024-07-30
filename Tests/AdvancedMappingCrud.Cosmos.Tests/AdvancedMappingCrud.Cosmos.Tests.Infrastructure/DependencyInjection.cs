using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Repositories;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository();
            services.AddScoped<IBasicOrderByRepository, BasicOrderByCosmosDBRepository>();
            services.AddScoped<ICustomerRepository, CustomerCosmosDBRepository>();
            services.AddScoped<IExplicitETagRepository, ExplicitETagCosmosDBRepository>();
            services.AddScoped<IOrderRepository, OrderCosmosDBRepository>();
            services.AddScoped<IProductRepository, ProductCosmosDBRepository>();
            services.AddScoped<ISimpleOdataRepository, SimpleOdataCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}