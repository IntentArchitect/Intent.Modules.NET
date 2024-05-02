using System.Transactions;
using EntityFrameworkCore.MultiConnectionStrings.Domain.Common.Interfaces;
using EntityFrameworkCore.MultiConnectionStrings.Domain.Repositories;
using EntityFrameworkCore.MultiConnectionStrings.Infrastructure.Persistence;
using EntityFrameworkCore.MultiConnectionStrings.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.MultiConnectionStrings.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AlternateDbDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("AlternateDbConnectionString"),
                    b => b.MigrationsAssembly(typeof(AlternateDbDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IClassARepository, ClassARepository>();
            services.AddTransient<IClassBRepository, ClassBRepository>();
            return services;
        }
    }
}