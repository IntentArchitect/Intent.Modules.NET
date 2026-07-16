using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Mapperly.Application.Common.Interfaces;
using Wolverine.Mapperly.Domain.Common.Interfaces;
using Wolverine.Mapperly.Domain.Repositories;
using Wolverine.Mapperly.Infrastructure.Persistence;
using Wolverine.Mapperly.Infrastructure.Repositories;
using Wolverine.Mapperly.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Wolverine.Mapperly.Infrastructure
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
            services.AddTransient<IProductRepository, ProductRepository>();
            return services;
        }
    }
}