using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using AspNetCoreCleanArchitecture.Sample.Domain.Common.Interfaces;
using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using AspNetCoreCleanArchitecture.Sample.Infrastructure.Persistence;
using AspNetCoreCleanArchitecture.Sample.Infrastructure.Repositories;
using AspNetCoreCleanArchitecture.Sample.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Infrastructure
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
            services.AddTransient<IBuyerRepository, BuyerRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}