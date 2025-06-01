using Intent.Modules.NET.Tests.Module1.Application.Interfaces;
using Intent.Modules.NET.Tests.Module1.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module1.Domain.Repositories;
using Intent.Modules.NET.Tests.Module1.Infrastructure.Persistence;
using Intent.Modules.NET.Tests.Module1.Infrastructure.Repositories;
using Intent.Modules.NET.Tests.Module1.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Module1DbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("Module1");
                options.UseLazyLoadingProxies();
            });
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Module1DbContext>());
            return services;
        }
    }
}