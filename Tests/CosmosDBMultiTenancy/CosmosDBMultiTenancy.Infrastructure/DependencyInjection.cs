using CosmosDBMultiTenancy.Application.Common.Interfaces;
using CosmosDBMultiTenancy.Domain.Common.Interfaces;
using CosmosDBMultiTenancy.Domain.Repositories;
using CosmosDBMultiTenancy.Infrastructure.Persistence;
using CosmosDBMultiTenancy.Infrastructure.Persistence.Documents;
using CosmosDBMultiTenancy.Infrastructure.Repositories;
using CosmosDBMultiTenancy.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CosmosDBMultiTenancy.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository(options =>
            {
                options.ContainerPerItemType = true;

                options.ContainerBuilder
                    .Configure<InvoiceDocument>(c => c
                        .WithContainer("CosmosDBMultiTenancy")
                        .WithPartitionKey("/tenantId"));
            });
            services.AddScoped<IInvoiceRepository, InvoiceCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}