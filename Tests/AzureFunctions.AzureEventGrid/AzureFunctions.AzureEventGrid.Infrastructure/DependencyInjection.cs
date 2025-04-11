using AzureFunctions.AzureEventGrid.Application.Common.Interfaces;
using AzureFunctions.AzureEventGrid.Domain.Common.Interfaces;
using AzureFunctions.AzureEventGrid.Infrastructure.Configuration;
using AzureFunctions.AzureEventGrid.Infrastructure.Persistence;
using AzureFunctions.AzureEventGrid.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure
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
            services.ConfigureEventGrid(configuration);
            return services;
        }
    }
}