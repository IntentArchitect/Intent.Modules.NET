using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Domain.Common.Interfaces;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Associations;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.NestedAssociations;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Repositories;
using MongoDb.TestApplication.Infrastructure.Repositories.Associations;
using MongoDb.TestApplication.Infrastructure.Repositories.IdTypes;
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
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetService<ApplicationMongoDbContext>());
            services.AddTransient<IA_RequiredCompositeRepository, A_RequiredCompositeRepository>();
            services.AddTransient<IAggregateARepository, AggregateARepository>();
            services.AddTransient<IAggregateBRepository, AggregateBRepository>();
            services.AddTransient<IB_OptionalAggregateRepository, B_OptionalAggregateRepository>();
            services.AddTransient<IB_OptionalDependentRepository, B_OptionalDependentRepository>();
            services.AddTransient<IC_RequireCompositeRepository, C_RequireCompositeRepository>();
            services.AddTransient<ID_MultipleDependentRepository, D_MultipleDependentRepository>();
            services.AddTransient<ID_OptionalAggregateRepository, D_OptionalAggregateRepository>();
            services.AddTransient<IE_RequiredCompositeNavRepository, E_RequiredCompositeNavRepository>();
            services.AddTransient<IF_OptionalAggregateNavRepository, F_OptionalAggregateNavRepository>();
            services.AddTransient<IF_OptionalDependentRepository, F_OptionalDependentRepository>();
            services.AddTransient<IG_RequiredCompositeNavRepository, G_RequiredCompositeNavRepository>();
            services.AddTransient<IH_MultipleDependentRepository, H_MultipleDependentRepository>();
            services.AddTransient<IH_OptionalAggregateNavRepository, H_OptionalAggregateNavRepository>();
            services.AddTransient<II_MultipleAggregateRepository, I_MultipleAggregateRepository>();
            services.AddTransient<II_RequiredDependentRepository, I_RequiredDependentRepository>();
            services.AddTransient<IIdTypeGuidRepository, IdTypeGuidRepository>();
            services.AddTransient<IIdTypeOjectIdStrRepository, IdTypeOjectIdStrRepository>();
            services.AddTransient<IJ_MultipleAggregateRepository, J_MultipleAggregateRepository>();
            services.AddTransient<IJ_MultipleDependentRepository, J_MultipleDependentRepository>();
            services.AddTransient<IK_MultipleAggregateNavRepository, K_MultipleAggregateNavRepository>();
            services.AddTransient<IK_MultipleDependentRepository, K_MultipleDependentRepository>();
            return services;
        }
    }
}