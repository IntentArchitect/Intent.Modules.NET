using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.NoEventing.Application.Common.Interfaces;
using TestMediator.NoEventing.Domain.Common.Interfaces;
using TestMediator.NoEventing.Domain.Repositories.Products;
using TestMediator.NoEventing.Infrastructure.Configuration;
using TestMediator.NoEventing.Infrastructure.Persistence;
using TestMediator.NoEventing.Infrastructure.Repositories.Products;
using TestMediator.NoEventing.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestMediator.NoEventing.Infrastructure
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
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddTransient<IPriceHistoryEntryRepository, PriceHistoryEntryRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddMediatorConfiguration();
            return services;
        }
    }
}