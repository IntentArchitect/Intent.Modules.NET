using System;
using AzureFunctions.MongoDb.Domain.Common.Interfaces;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.Associations;
using AzureFunctions.MongoDb.Domain.Repositories.Collections;
using AzureFunctions.MongoDb.Domain.Repositories.Collections.FolderCollection;
using AzureFunctions.MongoDb.Domain.Repositories.IdTypes;
using AzureFunctions.MongoDb.Domain.Repositories.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories.NestedAssociations;
using AzureFunctions.MongoDb.Domain.Repositories.ToManyIds;
using AzureFunctions.MongoDb.Infrastructure.Configuration;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings;
using AzureFunctions.MongoDb.Infrastructure.Repositories;
using AzureFunctions.MongoDb.Infrastructure.Repositories.Associations;
using AzureFunctions.MongoDb.Infrastructure.Repositories.Collections;
using AzureFunctions.MongoDb.Infrastructure.Repositories.Collections.FolderCollection;
using AzureFunctions.MongoDb.Infrastructure.Repositories.IdTypes;
using AzureFunctions.MongoDb.Infrastructure.Repositories.Indexes;
using AzureFunctions.MongoDb.Infrastructure.Repositories.Mappings;
using AzureFunctions.MongoDb.Infrastructure.Repositories.NestedAssociations;
using AzureFunctions.MongoDb.Infrastructure.Repositories.ToManyIds;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("MongoDbConnection");
            services.TryAddSingleton<IMongoClient>(_ => new MongoClient(cs));
            services.TryAddSingleton<IMongoDatabase>(sp =>
                    {
                        var dbName = new MongoUrl(cs).DatabaseName
                                     ?? throw new InvalidOperationException(
                                         "MongoDbConnection must include a database name.");
                        return sp.GetRequiredService<IMongoClient>().GetDatabase(dbName);
                    });
            services.RegisterMongoCollections(typeof(DependencyInjection).Assembly);
            services.AddScoped<IDerivedRepository, DerivedMongoRepository>();
            services.AddScoped<IDerivedOfTRepository, DerivedOfTMongoRepository>();
            services.AddScoped<IMapperM2MRepository, MapperM2MMongoRepository>();
            services.AddScoped<IA_RequiredCompositeRepository, A_RequiredCompositeMongoRepository>();
            services.AddScoped<IB_OptionalAggregateRepository, B_OptionalAggregateMongoRepository>();
            services.AddScoped<IB_OptionalDependentRepository, B_OptionalDependentMongoRepository>();
            services.AddScoped<IC_RequireCompositeRepository, C_RequireCompositeMongoRepository>();
            services.AddScoped<ID_MultipleDependentRepository, D_MultipleDependentMongoRepository>();
            services.AddScoped<ID_OptionalAggregateRepository, D_OptionalAggregateMongoRepository>();
            services.AddScoped<IE_RequiredCompositeNavRepository, E_RequiredCompositeNavMongoRepository>();
            services.AddScoped<IF_OptionalAggregateNavRepository, F_OptionalAggregateNavMongoRepository>();
            services.AddScoped<IF_OptionalDependentRepository, F_OptionalDependentMongoRepository>();
            services.AddScoped<IG_RequiredCompositeNavRepository, G_RequiredCompositeNavMongoRepository>();
            services.AddScoped<IH_MultipleDependentRepository, H_MultipleDependentMongoRepository>();
            services.AddScoped<IH_OptionalAggregateNavRepository, H_OptionalAggregateNavMongoRepository>();
            services.AddScoped<II_MultipleAggregateRepository, I_MultipleAggregateMongoRepository>();
            services.AddScoped<II_RequiredDependentRepository, I_RequiredDependentMongoRepository>();
            services.AddScoped<IJ_MultipleAggregateRepository, J_MultipleAggregateMongoRepository>();
            services.AddScoped<IJ_MultipleDependentRepository, J_MultipleDependentMongoRepository>();
            services.AddScoped<IK_MultipleAggregateNavRepository, K_MultipleAggregateNavMongoRepository>();
            services.AddScoped<IK_MultipleDependentRepository, K_MultipleDependentMongoRepository>();
            services.AddScoped<ICustomCollectionEntityARepository, CustomCollectionEntityAMongoRepository>();
            services.AddScoped<ICustomCollectionEntityBRepository, CustomCollectionEntityBMongoRepository>();
            services.AddScoped<IFolderCollectionEntityARepository, FolderCollectionEntityAMongoRepository>();
            services.AddScoped<IFolderCollectionEntityBRepository, FolderCollectionEntityBMongoRepository>();
            services.AddScoped<IIdTypeGuidRepository, IdTypeGuidMongoRepository>();
            services.AddScoped<IIdTypeOjectIdStrRepository, IdTypeOjectIdStrMongoRepository>();
            services.AddScoped<ICompoundIndexEntityRepository, CompoundIndexEntityMongoRepository>();
            services.AddScoped<ICompoundIndexEntityMultiParentRepository, CompoundIndexEntityMultiParentMongoRepository>();
            services.AddScoped<ICompoundIndexEntitySingleParentRepository, CompoundIndexEntitySingleParentMongoRepository>();
            services.AddScoped<IMultikeyIndexEntityRepository, MultikeyIndexEntityMongoRepository>();
            services.AddScoped<IMultikeyIndexEntityMultiParentRepository, MultikeyIndexEntityMultiParentMongoRepository>();
            services.AddScoped<IMultikeyIndexEntitySingleParentRepository, MultikeyIndexEntitySingleParentMongoRepository>();
            services.AddScoped<ISingleIndexEntityRepository, SingleIndexEntityMongoRepository>();
            services.AddScoped<ISingleIndexEntityMultiParentRepository, SingleIndexEntityMultiParentMongoRepository>();
            services.AddScoped<ISingleIndexEntitySingleParentRepository, SingleIndexEntitySingleParentMongoRepository>();
            services.AddScoped<ITextIndexEntityRepository, TextIndexEntityMongoRepository>();
            services.AddScoped<ITextIndexEntityMultiParentRepository, TextIndexEntityMultiParentMongoRepository>();
            services.AddScoped<ITextIndexEntitySingleParentRepository, TextIndexEntitySingleParentMongoRepository>();
            services.AddScoped<IMapAggChildRepository, MapAggChildMongoRepository>();
            services.AddScoped<IMapAggPeerRepository, MapAggPeerMongoRepository>();
            services.AddScoped<IMapAggPeerAggRepository, MapAggPeerAggMongoRepository>();
            services.AddScoped<IMapAggPeerAggMoreRepository, MapAggPeerAggMoreMongoRepository>();
            services.AddScoped<IMapCompChildAggRepository, MapCompChildAggMongoRepository>();
            services.AddScoped<IMapImplyOptionalRepository, MapImplyOptionalMongoRepository>();
            services.AddScoped<IMapMapMeRepository, MapMapMeMongoRepository>();
            services.AddScoped<IMapPeerCompChildAggRepository, MapPeerCompChildAggMongoRepository>();
            services.AddScoped<IMapperRootRepository, MapperRootMongoRepository>();
            services.AddScoped<IAggregateARepository, AggregateAMongoRepository>();
            services.AddScoped<IAggregateBRepository, AggregateBMongoRepository>();
            services.AddScoped<IToManySourceRepository, ToManySourceMongoRepository>();

            services.AddScoped<MongoDbUnitOfWork>();

            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            return services;
        }
    }
}