using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Common.Interfaces;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Repositories;
using Subscribe.MassTransit.DomainInteractionsRepro.Infrastructure.Configuration;
using Subscribe.MassTransit.DomainInteractionsRepro.Infrastructure.Persistence;
using Subscribe.MassTransit.DomainInteractionsRepro.Infrastructure.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICatalogueRepository, CatalogueRepository>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}