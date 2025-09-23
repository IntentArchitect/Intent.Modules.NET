using System;
using System.Linq;
using System.Reflection;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Mappings;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoConfigurationExtensions", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Configuration
{
    internal static class MongoConfigurationExtensions
    {
        public static IServiceCollection AddMongoCollection<T>(
            this IServiceCollection services,
            IMongoMappingConfiguration<T> mongoConfiguration)
        {
            mongoConfiguration.RegisterCollectionMap();
            services.AddSingleton(sp =>
                                    {
                                        var database = sp.GetRequiredService<IMongoDatabase>();
                                        return database.GetCollection<T>(mongoConfiguration.CollectionName);
                                    });
            return services;
        }

        public static IServiceCollection RegisterMongoCollections(this IServiceCollection services, Assembly assembly)
        {
            services.AddMongoCollection(new CustomerMapping());
            services.AddMongoCollection(new ExternalDocMapping());
            services.AddMongoCollection(new OrderMapping());
            services.AddMongoCollection(new ProductMapping());
            return services;
        }
    }
}