using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using MassTransitFinbuckle.Test.Application.Common.Eventing;
using MassTransitFinbuckle.Test.Application.Common.Interfaces;
using MassTransitFinbuckle.Test.Application.IntegrationServices;
using MassTransitFinbuckle.Test.Domain.Common.Interfaces;
using MassTransitFinbuckle.Test.Infrastructure.Configuration;
using MassTransitFinbuckle.Test.Infrastructure.Eventing;
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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var tenantInfo = sp.GetService<ITenantInfo>() ?? throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant info.");
                options.UseInMemoryDatabase(tenantInfo.ConnectionString);
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