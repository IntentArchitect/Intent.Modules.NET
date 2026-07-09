using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using MassTransitFinbuckle.Test.Application.Common.Eventing;
using MassTransitFinbuckle.Test.Application.Common.Interfaces;
using MassTransitFinbuckle.Test.Application.IntegrationServices;
using MassTransitFinbuckle.Test.Domain.Common.Interfaces;
using MassTransitFinbuckle.Test.Infrastructure.Configuration;
using MassTransitFinbuckle.Test.Infrastructure.Eventing;
using MassTransitFinbuckle.Test.Infrastructure.MultiTenant;
using MassTransitFinbuckle.Test.Infrastructure.Persistence;
using MassTransitFinbuckle.Test.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Infrastructure
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
                options.UseInMemoryDatabase(connectionString);
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IRequestResponseService, RequestResponseService>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}
