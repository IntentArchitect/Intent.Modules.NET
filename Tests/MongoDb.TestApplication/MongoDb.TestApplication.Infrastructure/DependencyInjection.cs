using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Domain.Common.Interfaces;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Associations;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;
using MongoDb.TestApplication.Domain.Repositories.NestedAssociations;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Repositories;
using MongoDb.TestApplication.Infrastructure.Repositories.Associations;
using MongoDb.TestApplication.Infrastructure.Repositories.IdTypes;
using MongoDb.TestApplication.Infrastructure.Repositories.Indexes;
using MongoDb.TestApplication.Infrastructure.Repositories.NestedAssociations;
using MongoFramework;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<ApplicationMongoDbContext>());
            services.AddTransient<IA_RequiredCompositeRepository, A_RequiredCompositeMongoRepository>();
            services.AddTransient<IAggregateARepository, AggregateAMongoRepository>();
            services.AddTransient<IAggregateBRepository, AggregateBMongoRepository>();
            services.AddTransient<IB_OptionalAggregateRepository, B_OptionalAggregateMongoRepository>();
            services.AddTransient<IB_OptionalDependentRepository, B_OptionalDependentMongoRepository>();
            services.AddTransient<IC_RequireCompositeRepository, C_RequireCompositeMongoRepository>();
            services.AddTransient<ICompoundIndexEntityRepository, CompoundIndexEntityMongoRepository>();
            services.AddTransient<ICompoundIndexEntityMultiParentRepository, CompoundIndexEntityMultiParentMongoRepository>();
            services.AddTransient<ICompoundIndexEntitySingleParentRepository, CompoundIndexEntitySingleParentMongoRepository>();
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
            services.AddTransient<IIdTypeGuidRepository, IdTypeGuidMongoRepository>();
            services.AddTransient<IIdTypeOjectIdStrRepository, IdTypeOjectIdStrMongoRepository>();
            services.AddTransient<IJ_MultipleAggregateRepository, J_MultipleAggregateMongoRepository>();
            services.AddTransient<IJ_MultipleDependentRepository, J_MultipleDependentMongoRepository>();
            services.AddTransient<IK_MultipleAggregateNavRepository, K_MultipleAggregateNavMongoRepository>();
            services.AddTransient<IK_MultipleDependentRepository, K_MultipleDependentMongoRepository>();
            services.AddTransient<IMultikeyIndexEntityRepository, MultikeyIndexEntityMongoRepository>();
            services.AddTransient<IMultikeyIndexEntityMultiParentRepository, MultikeyIndexEntityMultiParentMongoRepository>();
            services.AddTransient<IMultikeyIndexEntitySingleParentRepository, MultikeyIndexEntitySingleParentMongoRepository>();
            services.AddTransient<ISingleIndexEntityRepository, SingleIndexEntityMongoRepository>();
            services.AddTransient<ISingleIndexEntityMultiParentRepository, SingleIndexEntityMultiParentMongoRepository>();
            services.AddTransient<ISingleIndexEntitySingleParentRepository, SingleIndexEntitySingleParentMongoRepository>();
            services.AddTransient<ITextIndexEntityRepository, TextIndexEntityMongoRepository>();
            services.AddTransient<ITextIndexEntityMultiParentRepository, TextIndexEntityMultiParentMongoRepository>();
            services.AddTransient<ITextIndexEntitySingleParentRepository, TextIndexEntitySingleParentMongoRepository>();
            return services;
        }
    }
}