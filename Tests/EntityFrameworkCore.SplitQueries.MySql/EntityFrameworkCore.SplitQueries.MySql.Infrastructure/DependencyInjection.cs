using EntityFrameworkCore.SplitQueries.MySql.Domain.Common.Interfaces;
using EntityFrameworkCore.SplitQueries.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.MySql.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("8.0"), b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "mysql");
                });
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            return services;
        }
    }
}