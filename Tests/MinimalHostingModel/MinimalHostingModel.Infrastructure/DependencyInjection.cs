using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalHostingModel.Application.Common.Interfaces;
using MinimalHostingModel.Domain.Common.Interfaces;
using MinimalHostingModel.Infrastructure.Configuration;
using MinimalHostingModel.Infrastructure.Persistence;
using MinimalHostingModel.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MinimalHostingModel.Infrastructure
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
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            services.AddHttpClients(configuration);
            return services;
        }
    }
}