using System.Reflection;
using AutoMapper;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.SeparateDatabase.TestApplication.Application;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Common.Interfaces;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Repositories;
using Finbuckle.SeparateDatabase.TestApplication.Infrastructure.MultiTenant;
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
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                // Design-time safe: at runtime the tenant is always resolved and its connection string is used; at design time (EF tooling) no tenant is resolved, so fall back to DefaultConnection so FindContextTypes()/migrations do not throw.
                var tenantInfo = sp.GetRequiredService<IMultiTenantContextAccessor<TenantExtendedInfo>>().MultiTenantContext?.TenantInfo;
                var connectionString = tenantInfo?.ConnectionString ?? configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IUserRepository, UserRepository>();
            return services;
        }
    }
}
