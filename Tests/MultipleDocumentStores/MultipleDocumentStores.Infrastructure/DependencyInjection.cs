using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MultipleDocumentStores.Application.Common.Interfaces;
using MultipleDocumentStores.Domain.Common.Interfaces;
using MultipleDocumentStores.Domain.Repositories;
using MultipleDocumentStores.Infrastructure.Persistence;
using MultipleDocumentStores.Infrastructure.Persistence.Documents;
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
            services.AddSingleton<IMongoClient>(sp =>
                    {
                        var connectionString = configuration.GetConnectionString("MongoDbConnection");
                        return new MongoClient(connectionString);
                    });
            services.AddSingleton(sp =>
                    {
                        var connectionString = configuration.GetConnectionString("MongoDbConnection");

                        // Parse connection string to get the database name
                        var mongoUrl = new MongoUrl(connectionString);
                        var client = sp.GetRequiredService<IMongoClient>();

                        return client.GetDatabase(mongoUrl.DatabaseName);
                    });
            services.AddSingleton<IMongoCollection<CustomerMongoDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CustomerMongoDocument>("CustomerMongo");
                            });
            services.AddScoped<ICustomerCosmosRepository, CustomerCosmosCosmosDBRepository>();
            services.AddScoped<ICustomerDaprRepository, CustomerDaprDaprStateStoreRepository>();
            services.AddScoped<ICustomerMongoRepository, CustomerMongoMongoRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<DaprStateStoreUnitOfWork>();
            services.AddScoped<IDaprStateStoreUnitOfWork>(provider => provider.GetRequiredService<DaprStateStoreUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<MongoDbUnitOfWork>();
            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            services.AddScoped<IDaprStateStoreGenericRepository, DaprStateStoreGenericRepository>();
            return services;
        }
    }
}