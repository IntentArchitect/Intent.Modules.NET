using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WolverineEventing.MultiTenancy.Application.Common.Eventing;
using WolverineEventing.MultiTenancy.Domain.Common.Interfaces;
using WolverineEventing.MultiTenancy.Infrastructure.Eventing;
using WolverineEventing.MultiTenancy.Infrastructure.MultiTenant;
using WolverineEventing.MultiTenancy.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace WolverineEventing.MultiTenancy.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var tenantInfo = sp.GetRequiredService<IMultiTenantContextAccessor<TenantExtendedInfo>>().MultiTenantContext?.TenantInfo;
                var connectionString = tenantInfo?.ConnectionString ?? throw new MultiTenantException(sp.GetRequiredService<IHostEnvironment>().IsDevelopment()
                    ? "Failed to resolve tenant connection information. If you are running EF Core CLI commands (e.g. 'dotnet ef migrations'), install the Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory module."
                    : "Failed to resolve tenant connection information.");
                options.UseInMemoryDatabase(connectionString);
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IMessageBus, WolverineMessageBus>();
            return services;
        }
    }
}