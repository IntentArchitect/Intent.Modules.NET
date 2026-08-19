using Azure.Data.Tables;
using AzureIdentityManagement.Application.Common.Storage;
using AzureIdentityManagement.Domain.Common.Interfaces;
using AzureIdentityManagement.Infrastructure.BlobStorage;
using AzureIdentityManagement.Infrastructure.Configuration;
using AzureIdentityManagement.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureIdentityManagement.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<TableServiceClient>(provider => new TableServiceClient(configuration["TableStorageConnectionString"]));
            services.ConfigureCosmosRepository(configuration);
            services.AddTransient<IBlobStorage, AzureBlobStorage>();
            services.AddScoped<TableStorageUnitOfWork>();
            services.AddScoped<ITableStorageUnitOfWork>(provider => provider.GetRequiredService<TableStorageUnitOfWork>());
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddAzureServiceBusConfiguration(configuration);
            return services;
        }
    }
}