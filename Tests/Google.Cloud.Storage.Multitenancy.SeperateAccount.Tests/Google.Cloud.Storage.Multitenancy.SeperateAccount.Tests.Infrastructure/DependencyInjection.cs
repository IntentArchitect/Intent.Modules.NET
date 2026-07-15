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
                // Design-time safe: at runtime the tenant is always resolved and its identifier keys a separate
                // in-memory database per tenant; at design time (EF tooling) no tenant is resolved, so fall back
                // to DefaultConnection so FindContextTypes()/migrations do not throw.
                var tenantInfo = sp.GetRequiredService<IMultiTenantContextAccessor<TenantExtendedInfo>>().MultiTenantContext?.TenantInfo;
                var connectionString = tenantInfo?.Identifier ?? configuration.GetConnectionString("DefaultConnection");
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
