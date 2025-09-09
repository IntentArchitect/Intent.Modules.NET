using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Collections;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Collections.FolderCollection;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.IdTypes;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Mappings;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.NestedAssociations;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.ToManyIds;
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
            services.AddSingleton<IMongoCollection<A_OptionalDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<A_OptionalDependentDocument>("A_OptionalDependent");
                            });
            services.AddSingleton<IMongoCollection<A_RequiredCompositeDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<A_RequiredCompositeDocument>("A_RequiredComposite");
                            });
            services.AddSingleton<IMongoCollection<AggregateADocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<AggregateADocument>("AggregateA");
                            });
            services.AddSingleton<IMongoCollection<AggregateBDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<AggregateBDocument>("AggregateB");
                            });
            services.AddSingleton<IMongoCollection<B_OptionalAggregateDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<B_OptionalAggregateDocument>("B_OptionalAggregate");
                            });
            services.AddSingleton<IMongoCollection<B_OptionalDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<B_OptionalDependentDocument>("B_OptionalDependent");
                            });
            services.AddSingleton<IMongoCollection<C_MultipleDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<C_MultipleDependentDocument>("C_MultipleDependent");
                            });
            services.AddSingleton<IMongoCollection<C_RequireCompositeDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<C_RequireCompositeDocument>("C_RequireComposite");
                            });
            services.AddSingleton<IMongoCollection<CompoundIndexEntityDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CompoundIndexEntityDocument>("CompoundIndexEntity");
                            });
            services.AddSingleton<IMongoCollection<CompoundIndexEntityMultiChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CompoundIndexEntityMultiChildDocument>("CompoundIndexEntityMultiChild");
                            });
            services.AddSingleton<IMongoCollection<CompoundIndexEntityMultiParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CompoundIndexEntityMultiParentDocument>("CompoundIndexEntityMultiParent");
                            });
            services.AddSingleton<IMongoCollection<CompoundIndexEntitySingleChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CompoundIndexEntitySingleChildDocument>("CompoundIndexEntitySingleChild");
                            });
            services.AddSingleton<IMongoCollection<CompoundIndexEntitySingleParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CompoundIndexEntitySingleParentDocument>("CompoundIndexEntitySingleParent");
                            });
            services.AddSingleton<IMongoCollection<CustomCollectionEntityADocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CustomCollectionEntityADocument>("CustomCollectionEntityA");
                            });
            services.AddSingleton<IMongoCollection<CustomCollectionEntityBDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CustomCollectionEntityBDocument>("CustomCollectionEntityB");
                            });
            services.AddSingleton<IMongoCollection<D_MultipleDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<D_MultipleDependentDocument>("D_MultipleDependent");
                            });
            services.AddSingleton<IMongoCollection<D_OptionalAggregateDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<D_OptionalAggregateDocument>("D_OptionalAggregate");
                            });
            services.AddSingleton<IMongoCollection<DerivedDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<DerivedDocument>("Derived");
                            });
            services.AddSingleton<IMongoCollection<DerivedOfTDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<DerivedOfTDocument>("DerivedOfT");
                            });
            services.AddSingleton<IMongoCollection<E_RequiredCompositeNavDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<E_RequiredCompositeNavDocument>("E_RequiredCompositeNav");
                            });
            services.AddSingleton<IMongoCollection<E_RequiredDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<E_RequiredDependentDocument>("E_RequiredDependent");
                            });
            services.AddSingleton<IMongoCollection<F_OptionalAggregateNavDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<F_OptionalAggregateNavDocument>("F_OptionalAggregateNav");
                            });
            services.AddSingleton<IMongoCollection<F_OptionalDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<F_OptionalDependentDocument>("F_OptionalDependent");
                            });
            services.AddSingleton<IMongoCollection<FolderCollectionEntityADocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<FolderCollectionEntityADocument>("FolderCollectionEntityA");
                            });
            services.AddSingleton<IMongoCollection<FolderCollectionEntityBDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<FolderCollectionEntityBDocument>("FolderCollectionEntityB");
                            });
            services.AddSingleton<IMongoCollection<G_MultipleDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<G_MultipleDependentDocument>("G_MultipleDependent");
                            });
            services.AddSingleton<IMongoCollection<G_RequiredCompositeNavDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<G_RequiredCompositeNavDocument>("G_RequiredCompositeNav");
                            });
            services.AddSingleton<IMongoCollection<H_MultipleDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<H_MultipleDependentDocument>("H_MultipleDependent");
                            });
            services.AddSingleton<IMongoCollection<H_OptionalAggregateNavDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<H_OptionalAggregateNavDocument>("H_OptionalAggregateNav");
                            });
            services.AddSingleton<IMongoCollection<I_MultipleAggregateDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<I_MultipleAggregateDocument>("I_MultipleAggregate");
                            });
            services.AddSingleton<IMongoCollection<I_RequiredDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<I_RequiredDependentDocument>("I_RequiredDependent");
                            });
            services.AddSingleton<IMongoCollection<IdTypeGuidDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<IdTypeGuidDocument>("IdTypeGuid");
                            });
            services.AddSingleton<IMongoCollection<IdTypeOjectIdStrDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<IdTypeOjectIdStrDocument>("IdTypeOjectIdStr");
                            });
            services.AddSingleton<IMongoCollection<J_MultipleAggregateDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<J_MultipleAggregateDocument>("J_MultipleAggregate");
                            });
            services.AddSingleton<IMongoCollection<J_MultipleDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<J_MultipleDependentDocument>("J_MultipleDependent");
                            });
            services.AddSingleton<IMongoCollection<K_MultipleAggregateNavDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<K_MultipleAggregateNavDocument>("K_MultipleAggregateNav");
                            });
            services.AddSingleton<IMongoCollection<K_MultipleDependentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<K_MultipleDependentDocument>("K_MultipleDependent");
                            });
            services.AddSingleton<IMongoCollection<MapAggChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapAggChildDocument>("MapAggChild");
                            });
            services.AddSingleton<IMongoCollection<MapAggPeerDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapAggPeerDocument>("MapAggPeer");
                            });
            services.AddSingleton<IMongoCollection<MapAggPeerAggDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapAggPeerAggDocument>("MapAggPeerAgg");
                            });
            services.AddSingleton<IMongoCollection<MapAggPeerAggMoreDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapAggPeerAggMoreDocument>("MapAggPeerAggMore");
                            });
            services.AddSingleton<IMongoCollection<MapCompChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapCompChildDocument>("MapCompChild");
                            });
            services.AddSingleton<IMongoCollection<MapCompChildAggDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapCompChildAggDocument>("MapCompChildAgg");
                            });
            services.AddSingleton<IMongoCollection<MapCompOptionalDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapCompOptionalDocument>("MapCompOptional");
                            });
            services.AddSingleton<IMongoCollection<MapImplyOptionalDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapImplyOptionalDocument>("MapImplyOptional");
                            });
            services.AddSingleton<IMongoCollection<MapMapMeDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapMapMeDocument>("MapMapMe");
                            });
            services.AddSingleton<IMongoCollection<MapPeerCompChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapPeerCompChildDocument>("MapPeerCompChild");
                            });
            services.AddSingleton<IMongoCollection<MapPeerCompChildAggDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapPeerCompChildAggDocument>("MapPeerCompChildAgg");
                            });
            services.AddSingleton<IMongoCollection<MapperM2MDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapperM2MDocument>("MapperM2M");
                            });
            services.AddSingleton<IMongoCollection<MapperRootDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MapperRootDocument>("MapperRoot");
                            });
            services.AddSingleton<IMongoCollection<MultikeyIndexEntityDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MultikeyIndexEntityDocument>("MultikeyIndexEntity");
                            });
            services.AddSingleton<IMongoCollection<MultikeyIndexEntityMultiChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MultikeyIndexEntityMultiChildDocument>("MultikeyIndexEntityMultiChild");
                            });
            services.AddSingleton<IMongoCollection<MultikeyIndexEntityMultiParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MultikeyIndexEntityMultiParentDocument>("MultikeyIndexEntityMultiParent");
                            });
            services.AddSingleton<IMongoCollection<MultikeyIndexEntitySingleChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MultikeyIndexEntitySingleChildDocument>("MultikeyIndexEntitySingleChild");
                            });
            services.AddSingleton<IMongoCollection<MultikeyIndexEntitySingleParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MultikeyIndexEntitySingleParentDocument>("MultikeyIndexEntitySingleParent");
                            });
            services.AddSingleton<IMongoCollection<NestedCompositionADocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<NestedCompositionADocument>("NestedCompositionA");
                            });
            services.AddSingleton<IMongoCollection<SingleIndexEntityDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<SingleIndexEntityDocument>("SingleIndexEntity");
                            });
            services.AddSingleton<IMongoCollection<SingleIndexEntityMultiChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<SingleIndexEntityMultiChildDocument>("SingleIndexEntityMultiChild");
                            });
            services.AddSingleton<IMongoCollection<SingleIndexEntityMultiParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<SingleIndexEntityMultiParentDocument>("SingleIndexEntityMultiParent");
                            });
            services.AddSingleton<IMongoCollection<SingleIndexEntitySingleChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<SingleIndexEntitySingleChildDocument>("SingleIndexEntitySingleChild");
                            });
            services.AddSingleton<IMongoCollection<SingleIndexEntitySingleParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<SingleIndexEntitySingleParentDocument>("SingleIndexEntitySingleParent");
                            });
            services.AddSingleton<IMongoCollection<TextIndexEntityDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<TextIndexEntityDocument>("TextIndexEntity");
                            });
            services.AddSingleton<IMongoCollection<TextIndexEntityMultiChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<TextIndexEntityMultiChildDocument>("TextIndexEntityMultiChild");
                            });
            services.AddSingleton<IMongoCollection<TextIndexEntityMultiParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<TextIndexEntityMultiParentDocument>("TextIndexEntityMultiParent");
                            });
            services.AddSingleton<IMongoCollection<TextIndexEntitySingleChildDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<TextIndexEntitySingleChildDocument>("TextIndexEntitySingleChild");
                            });
            services.AddSingleton<IMongoCollection<TextIndexEntitySingleParentDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<TextIndexEntitySingleParentDocument>("TextIndexEntitySingleParent");
                            });
            services.AddSingleton<IMongoCollection<ToManyGuidDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<ToManyGuidDocument>("ToManyGuid");
                            });
            services.AddSingleton<IMongoCollection<ToManyIntDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<ToManyIntDocument>("ToManyInt");
                            });
            services.AddSingleton<IMongoCollection<ToManyLongDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<ToManyLongDocument>("ToManyLong");
                            });
            services.AddSingleton<IMongoCollection<ToManySourceDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<ToManySourceDocument>("ToManySource");
                            });
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