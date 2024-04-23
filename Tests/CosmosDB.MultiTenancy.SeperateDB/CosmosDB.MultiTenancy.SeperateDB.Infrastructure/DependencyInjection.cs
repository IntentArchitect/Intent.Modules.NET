using CosmosDB.MultiTenancy.SeperateDB.Application.Common.Interfaces;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Common.Interfaces;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Configuration;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence.Documents;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Repositories;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository(options =>
            {
                options.ContainerPerItemType = true;

                options.ContainerBuilder
                    .Configure<CustomerDocument>(c => c
                        .WithContainer("Customer"));
            });
            services.ConfigureCosmosSeperateDBMultiTenancy(configuration);
            services.AddScoped<ICustomerRepository, CustomerCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}