using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;
using MultipleDocumentStores.Application.Common.Interfaces;
using MultipleDocumentStores.Domain.Common.Interfaces;
using MultipleDocumentStores.Domain.Repositories;
using MultipleDocumentStores.Infrastructure.Persistence;
using MultipleDocumentStores.Infrastructure.Repositories;
using MultipleDocumentStores.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository();
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));
            services.AddScoped<ICustomerCosmosRepository, CustomerCosmosCosmosDBRepository>();
            services.AddScoped<ICustomerDaprRepository, CustomerDaprDaprStateStoreRepository>();
            services.AddTransient<ICustomerMongoRepository, CustomerMongoMongoRepository>();
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<ApplicationMongoDbContext>());
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<DaprStateStoreUnitOfWork>();
            services.AddScoped<IDaprStateStoreUnitOfWork>(provider => provider.GetRequiredService<DaprStateStoreUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IDaprStateStoreGenericRepository, DaprStateStoreGenericRepository>();
            return services;
        }
    }
}