using Azure.Data.Tables;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TableStorage.Tests.Application.Common.Interfaces;
using TableStorage.Tests.Domain.Common.Interfaces;
using TableStorage.Tests.Domain.Repositories;
using TableStorage.Tests.Infrastructure.Persistence;
using TableStorage.Tests.Infrastructure.Repositories;
using TableStorage.Tests.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TableStorage.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<TableServiceClient>(provider => new TableServiceClient(configuration["TableStorageConnectionString"]));
            services.AddScoped<IInvoiceRepository, InvoiceTableStorageRepository>();
            services.AddScoped<IOrderRepository, OrderTableStorageRepository>();
            services.AddScoped<TableStorageUnitOfWork>();
            services.AddScoped<ITableStorageUnitOfWork>(provider => provider.GetRequiredService<TableStorageUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}