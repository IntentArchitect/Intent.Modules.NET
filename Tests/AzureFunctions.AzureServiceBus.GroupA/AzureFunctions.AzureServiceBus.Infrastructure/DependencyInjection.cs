using AzureFunctions.AzureServiceBus.Application.Common.Interfaces;
using AzureFunctions.AzureServiceBus.Domain.Common.Interfaces;
using AzureFunctions.AzureServiceBus.Infrastructure.Configuration;
using AzureFunctions.AzureServiceBus.Infrastructure.Persistence;
using AzureFunctions.AzureServiceBus.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddAzureServiceBusConfiguration(configuration);
            return services;
        }
    }
}