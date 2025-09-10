using System;
using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDb.TestApplication.Application;
using MongoDb.TestApplication.Domain.Common.Interfaces;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Associations;
using MongoDb.TestApplication.Domain.Repositories.Collections;
using MongoDb.TestApplication.Domain.Repositories.Collections.FolderCollection;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Mappings;
using MongoDb.TestApplication.Domain.Repositories.NestedAssociations;
using MongoDb.TestApplication.Domain.Repositories.ToManyIds;
using MongoDb.TestApplication.Infrastructure.Configuration;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Repositories;
using MongoDb.TestApplication.Infrastructure.Repositories.Associations;
using MongoDb.TestApplication.Infrastructure.Repositories.Collections;
using MongoDb.TestApplication.Infrastructure.Repositories.Collections.FolderCollection;
using MongoDb.TestApplication.Infrastructure.Repositories.IdTypes;
using MongoDb.TestApplication.Infrastructure.Repositories.Indexes;
using MongoDb.TestApplication.Infrastructure.Repositories.Mappings;
using MongoDb.TestApplication.Infrastructure.Repositories.NestedAssociations;
using MongoDb.TestApplication.Infrastructure.Repositories.ToManyIds;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure
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