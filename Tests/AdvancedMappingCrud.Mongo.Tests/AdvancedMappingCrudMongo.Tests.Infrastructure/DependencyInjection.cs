using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Documents;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Repositories;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
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
            services.AddSingleton<IMongoCollection<CustomerDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CustomerDocument>("Customer");
                            });
            services.AddSingleton<IMongoCollection<ExternalDocDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<ExternalDocDocument>("ExternalDoc");
                            });
            services.AddSingleton<IMongoCollection<OrderDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<OrderDocument>("Order");
                            });
            services.AddSingleton<IMongoCollection<OrderItemDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<OrderItemDocument>("OrderItem");
                            });
            services.AddSingleton<IMongoCollection<ProductDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<ProductDocument>("Product");
                            });

            services.AddScoped<ICustomerRepository, CustomerMongoRepository>();

            services.AddScoped<IExternalDocRepository, ExternalDocMongoRepository>();
            services.AddScoped<IOrderRepository, OrderMongoRepository>();
            services.AddScoped<IProductRepository, ProductMongoRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<MongoDbUnitOfWork>();
            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            return services;
        }
    }
}