using Finbuckle.MultiTenant;
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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var tenantInfo = sp.GetService<ITenantInfo>() ?? throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant info.");
                options.UseInMemoryDatabase(tenantInfo.ConnectionString);
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<ITenantConnections>(
                provider => provider.GetService<ITenantInfo>() as TenantExtendedInfo ??
                throw new MultiTenantException("Failed to resolve tenant info"));
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddGoogleCloudStorage(configuration);
            return services;
        }
    }
}