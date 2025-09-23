using System;
using System.Linq;
using System.Reflection;
using AzureFunctions.MongoDb.Domain.Entities;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Associations;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Collections;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Collections.FolderCollection;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.IdTypes;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Indexes;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Mappings;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.NestedAssociations;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.ToManyIds;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoConfigurationExtensions", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Configuration
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
            services.AddMongoCollection(new A_RequiredCompositeMapping());
            services.AddMongoCollection(new AggregateAMapping());
            services.AddMongoCollection(new AggregateBMapping());
            services.AddMongoCollection(new B_OptionalAggregateMapping());
            services.AddMongoCollection(new B_OptionalDependentMapping());
            services.AddMongoCollection(new BaseTypeMapping());
            RegisterBaseTypeOfTMappings(assembly);
            services.AddMongoCollection(new C_RequireCompositeMapping());
            services.AddMongoCollection(new CompoundIndexEntityMapping());
            services.AddMongoCollection(new CompoundIndexEntityMultiParentMapping());
            services.AddMongoCollection(new CompoundIndexEntitySingleParentMapping());
            services.AddMongoCollection(new CustomCollectionEntityAMapping());
            services.AddMongoCollection(new CustomCollectionEntityBMapping());
            services.AddMongoCollection(new D_MultipleDependentMapping());
            services.AddMongoCollection(new D_OptionalAggregateMapping());
            services.AddMongoCollection(new DerivedMapping());
            services.AddMongoCollection(new DerivedOfTMapping());
            services.AddMongoCollection(new E_RequiredCompositeNavMapping());
            services.AddMongoCollection(new F_OptionalAggregateNavMapping());
            services.AddMongoCollection(new F_OptionalDependentMapping());
            services.AddMongoCollection(new FolderCollectionEntityAMapping());
            services.AddMongoCollection(new FolderCollectionEntityBMapping());
            services.AddMongoCollection(new G_RequiredCompositeNavMapping());
            services.AddMongoCollection(new H_MultipleDependentMapping());
            services.AddMongoCollection(new H_OptionalAggregateNavMapping());
            services.AddMongoCollection(new I_MultipleAggregateMapping());
            services.AddMongoCollection(new I_RequiredDependentMapping());
            services.AddMongoCollection(new IdTypeGuidMapping());
            services.AddMongoCollection(new IdTypeOjectIdStrMapping());
            services.AddMongoCollection(new J_MultipleAggregateMapping());
            services.AddMongoCollection(new J_MultipleDependentMapping());
            services.AddMongoCollection(new K_MultipleAggregateNavMapping());
            services.AddMongoCollection(new K_MultipleDependentMapping());
            services.AddMongoCollection(new MapAggChildMapping());
            services.AddMongoCollection(new MapAggPeerMapping());
            services.AddMongoCollection(new MapAggPeerAggMapping());
            services.AddMongoCollection(new MapAggPeerAggMoreMapping());
            services.AddMongoCollection(new MapCompChildAggMapping());
            services.AddMongoCollection(new MapImplyOptionalMapping());
            services.AddMongoCollection(new MapMapMeMapping());
            services.AddMongoCollection(new MapPeerCompChildAggMapping());
            services.AddMongoCollection(new MapperM2MMapping());
            services.AddMongoCollection(new MapperRootMapping());
            services.AddMongoCollection(new MultikeyIndexEntityMapping());
            services.AddMongoCollection(new MultikeyIndexEntityMultiParentMapping());
            services.AddMongoCollection(new MultikeyIndexEntitySingleParentMapping());
            services.AddMongoCollection(new SingleIndexEntityMapping());
            services.AddMongoCollection(new SingleIndexEntityMultiParentMapping());
            services.AddMongoCollection(new SingleIndexEntitySingleParentMapping());
            services.AddMongoCollection(new TextIndexEntityMapping());
            services.AddMongoCollection(new TextIndexEntityMultiParentMapping());
            services.AddMongoCollection(new TextIndexEntitySingleParentMapping());
            services.AddMongoCollection(new ToManySourceMapping());
            return services;
        }

        public static void RegisterBaseTypeOfTMappings(Assembly assembly)
        {
            var baseType = typeof(BaseTypeOfT<>);

            var derivedTypes = assembly.GetTypes()
                .Where(t => t.BaseType != null &&
                            t.BaseType.IsGenericType &&
                            t.BaseType.GetGenericTypeDefinition() == baseType);

            foreach (var derivedType in derivedTypes)
            {
                var genericArg = derivedType.BaseType!.GetGenericArguments()[0];
                var closedMappingType = typeof(BaseTypeOfTMapping<>).MakeGenericType(genericArg);
                var mappingInstance = Activator.CreateInstance(closedMappingType);
                var registerMethod = closedMappingType.GetMethod(nameof(BaseTypeOfTMapping<object>.RegisterCollectionMap));

                registerMethod?.Invoke(mappingInstance, null);
            }
        }
    }
}