using System;
using System.Linq;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoConfigurationExtensions", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure.Configuration
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
            return services;
        }
    }
}