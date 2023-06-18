using System.Reflection;
using AutoMapper;
using Finbuckle.MultiTenant;
using Finbuckle.SeparateDatabase.TestApplication.Application;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Common.Interfaces;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Repositories;
using Finbuckle.SeparateDatabase.TestApplication.Infrastructure.Persistence;
using Finbuckle.SeparateDatabase.TestApplication.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var tenantInfo = sp.GetService<ITenantInfo>() ?? throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant info.");
                options.UseSqlServer(
                    tenantInfo.ConnectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(Application.DependencyInjection).Assembly);
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IUserRepository, UserRepository>();
            return services;
        }
    }
}