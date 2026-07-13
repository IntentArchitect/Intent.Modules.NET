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
            // NOTE: Registered as Scoped (not the module's default Singleton) because IMongoDatabase
            // is resolved per-tenant (Scoped) in this multi-tenant SeperateDb app - see
            // MongoDb.MultiTenancy.SeperateDb.Infrastructure.DependencyInjection.AddInfrastructure.
            // A Singleton collection would capture only the first tenant's database and/or throw
            // "Cannot resolve scoped service 'IMongoDatabase' from root provider" at runtime.
            services.AddScoped(sp =>
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