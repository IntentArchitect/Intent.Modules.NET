using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.MultiTenant;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence.Interceptors;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Repositories;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Services;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SoftDeleteInterceptor>();
            services.AddDbContext<AlternateConnStrDefaultDbDbContext>((sp, options) =>
            {
                var tenantInfo = sp.GetRequiredService<IMultiTenantContextAccessor<TenantExtendedInfo>>().MultiTenantContext?.TenantInfo;
                var connectionString = tenantInfo?.ConnectionString ?? throw new MultiTenantException(sp.GetRequiredService<IHostEnvironment>().IsDevelopment()
                    ? "Failed to resolve tenant connection information. If you are running EF Core CLI commands (e.g. 'dotnet ef migrations'), install the Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory module."
                    : "Failed to resolve tenant connection information.");
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(AlternateConnStrDefaultDbDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
                options.AddInterceptors(sp.GetService<SoftDeleteInterceptor>()!);
            });
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(EntityFrameworkCoreMultiDbContextWithDefaultDbContextConstants.DefaultConnection),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomAlternateRepository, CustomAlternateRepository>();
            services.AddTransient<ICustomAppDefaultRepository, CustomAppDefaultRepository>();
            services.AddTransient<ICustomDefaultRepository, CustomDefaultRepository>();
            services.AddTransient<IAlternateConnStrDefaultDbDomainPackageAuditLogRepository, AlternateConnStrDefaultDbDomainPackageAuditLogRepository>();
            services.AddTransient<IAppDbContextDomainPackageAuditLogRepository, AppDbContextDomainPackageAuditLogRepository>();
            services.AddTransient<IDefaultDomainPackageAuditLogRepository, DefaultDomainPackageAuditLogRepository>();
            services.AddTransient<IEntityAlternateRepository, EntityAlternateRepository>();
            services.AddTransient<IEntityAppDefaultRepository, EntityAppDefaultRepository>();
            services.AddTransient<IEntityDefaultRepository, EntityDefaultRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}
