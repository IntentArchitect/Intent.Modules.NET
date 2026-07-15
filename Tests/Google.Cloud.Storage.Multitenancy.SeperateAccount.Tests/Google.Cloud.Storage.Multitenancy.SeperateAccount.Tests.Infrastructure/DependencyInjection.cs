using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Application.Common.Interfaces;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Domain.Common.Interfaces;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Domain.Repositories;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.Configuration;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.MultiTenant;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.Persistence;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.Repositories;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var tenantInfo = sp.GetRequiredService<IMultiTenantContextAccessor<TenantExtendedInfo>>().MultiTenantContext?.TenantInfo;
                var connectionString = tenantInfo?.Identifier ?? throw new MultiTenantException(sp.GetRequiredService<IHostEnvironment>().IsDevelopment()
                    ? "Failed to resolve tenant connection information. If you are running EF Core CLI commands (e.g. 'dotnet ef migrations'), install the Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory module."
                    : "Failed to resolve tenant connection information.");
                options.UseInMemoryDatabase(connectionString);
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<ITenantConnections>(
                    provider => provider.GetRequiredService<IMultiTenantContextAccessor<TenantExtendedInfo>>().MultiTenantContext?.TenantInfo as TenantExtendedInfo ??
                    throw new MultiTenantException("Failed to resolve tenant info"));
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddGoogleCloudStorage(configuration);
            return services;
        }
    }
}
