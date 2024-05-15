using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Persistence;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Repositories;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Db1DbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("Db1ConnectionString"),
                    b => b.MigrationsAssembly(typeof(Db1DbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddDbContext<Db2DbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("Db2ConnectionString"),
                    b => b.MigrationsAssembly(typeof(Db2DbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddDbContext<Db3DbContext>((sp, options) =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("Db3ConnectionString"),
                    b => b.MigrationsAssembly(typeof(Db3DbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddTransient<ICustomDb1Repository, CustomDb1Repository>();
            services.AddTransient<ICustomDb2Repository, CustomDb2Repository>();
            services.AddTransient<ICustomDb3Repository, CustomDb3Repository>();
            services.AddTransient<IDb1EntityRepository, Db1EntityRepository>();
            services.AddTransient<IDb2EntityRepository, Db2EntityRepository>();
            services.AddTransient<IDb3EntityRepository, Db3EntityRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}