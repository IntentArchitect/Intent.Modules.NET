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
using Microsoft.Extensions.Hosting;

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
                var tenantInfo = sp.GetRequiredService<IMultiTenantContextAccessor<TenantExtendedInfo>>().MultiTenantContext?.TenantInfo;
                var connectionString = tenantInfo?.ConnectionString ?? throw new MultiTenantException(sp.GetRequiredService<IHostEnvironment>().IsDevelopment()
                    ? "Failed to resolve tenant connection information. If you are running EF Core CLI commands (e.g. 'dotnet ef migrations'), install the Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory module."
                    : "Failed to resolve tenant connection information.");
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
