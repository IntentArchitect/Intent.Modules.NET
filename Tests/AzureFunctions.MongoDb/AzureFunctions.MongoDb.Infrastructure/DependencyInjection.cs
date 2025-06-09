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
using AzureFunctions.MongoDb.Infrastructure.Persistence;
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
using MongoFramework;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));
            services.AddTransient<IDerivedRepository, DerivedMongoRepository>();
            services.AddTransient<IDerivedOfTRepository, DerivedOfTMongoRepository>();
            services.AddTransient<IMapperM2MRepository, MapperM2MMongoRepository>();
            services.AddTransient<IA_RequiredCompositeRepository, A_RequiredCompositeMongoRepository>();
            services.AddTransient<IB_OptionalAggregateRepository, B_OptionalAggregateMongoRepository>();
            services.AddTransient<IB_OptionalDependentRepository, B_OptionalDependentMongoRepository>();
            services.AddTransient<IC_RequireCompositeRepository, C_RequireCompositeMongoRepository>();
            services.AddTransient<ID_MultipleDependentRepository, D_MultipleDependentMongoRepository>();
            services.AddTransient<ID_OptionalAggregateRepository, D_OptionalAggregateMongoRepository>();
            services.AddTransient<IE_RequiredCompositeNavRepository, E_RequiredCompositeNavMongoRepository>();
            services.AddTransient<IF_OptionalAggregateNavRepository, F_OptionalAggregateNavMongoRepository>();
            services.AddTransient<IF_OptionalDependentRepository, F_OptionalDependentMongoRepository>();
            services.AddTransient<IG_RequiredCompositeNavRepository, G_RequiredCompositeNavMongoRepository>();
            services.AddTransient<IH_MultipleDependentRepository, H_MultipleDependentMongoRepository>();
            services.AddTransient<IH_OptionalAggregateNavRepository, H_OptionalAggregateNavMongoRepository>();
            services.AddTransient<II_MultipleAggregateRepository, I_MultipleAggregateMongoRepository>();
            services.AddTransient<II_RequiredDependentRepository, I_RequiredDependentMongoRepository>();
            services.AddTransient<IJ_MultipleAggregateRepository, J_MultipleAggregateMongoRepository>();
            services.AddTransient<IJ_MultipleDependentRepository, J_MultipleDependentMongoRepository>();
            services.AddTransient<IK_MultipleAggregateNavRepository, K_MultipleAggregateNavMongoRepository>();
            services.AddTransient<IK_MultipleDependentRepository, K_MultipleDependentMongoRepository>();
            services.AddTransient<ICustomCollectionEntityARepository, CustomCollectionEntityAMongoRepository>();
            services.AddTransient<ICustomCollectionEntityBRepository, CustomCollectionEntityBMongoRepository>();
            services.AddTransient<IFolderCollectionEntityARepository, FolderCollectionEntityAMongoRepository>();
            services.AddTransient<IFolderCollectionEntityBRepository, FolderCollectionEntityBMongoRepository>();
            services.AddTransient<IIdTypeGuidRepository, IdTypeGuidMongoRepository>();
            services.AddTransient<IIdTypeOjectIdStrRepository, IdTypeOjectIdStrMongoRepository>();
            services.AddTransient<ICompoundIndexEntityRepository, CompoundIndexEntityMongoRepository>();
            services.AddTransient<ICompoundIndexEntityMultiParentRepository, CompoundIndexEntityMultiParentMongoRepository>();
            services.AddTransient<ICompoundIndexEntitySingleParentRepository, CompoundIndexEntitySingleParentMongoRepository>();
            services.AddTransient<IMultikeyIndexEntityRepository, MultikeyIndexEntityMongoRepository>();
            services.AddTransient<IMultikeyIndexEntityMultiParentRepository, MultikeyIndexEntityMultiParentMongoRepository>();
            services.AddTransient<IMultikeyIndexEntitySingleParentRepository, MultikeyIndexEntitySingleParentMongoRepository>();
            services.AddTransient<ISingleIndexEntityRepository, SingleIndexEntityMongoRepository>();
            services.AddTransient<ISingleIndexEntityMultiParentRepository, SingleIndexEntityMultiParentMongoRepository>();
            services.AddTransient<ISingleIndexEntitySingleParentRepository, SingleIndexEntitySingleParentMongoRepository>();
            services.AddTransient<ITextIndexEntityRepository, TextIndexEntityMongoRepository>();
            services.AddTransient<ITextIndexEntityMultiParentRepository, TextIndexEntityMultiParentMongoRepository>();
            services.AddTransient<ITextIndexEntitySingleParentRepository, TextIndexEntitySingleParentMongoRepository>();
            services.AddTransient<IMapAggChildRepository, MapAggChildMongoRepository>();
            services.AddTransient<IMapAggPeerRepository, MapAggPeerMongoRepository>();
            services.AddTransient<IMapAggPeerAggRepository, MapAggPeerAggMongoRepository>();
            services.AddTransient<IMapAggPeerAggMoreRepository, MapAggPeerAggMoreMongoRepository>();
            services.AddTransient<IMapCompChildAggRepository, MapCompChildAggMongoRepository>();
            services.AddTransient<IMapImplyOptionalRepository, MapImplyOptionalMongoRepository>();
            services.AddTransient<IMapMapMeRepository, MapMapMeMongoRepository>();
            services.AddTransient<IMapPeerCompChildAggRepository, MapPeerCompChildAggMongoRepository>();
            services.AddTransient<IMapperRootRepository, MapperRootMongoRepository>();
            services.AddTransient<IAggregateARepository, AggregateAMongoRepository>();
            services.AddTransient<IAggregateBRepository, AggregateBMongoRepository>();
            services.AddTransient<IToManySourceRepository, ToManySourceMongoRepository>();
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<ApplicationMongoDbContext>());
            return services;
        }
    }
}