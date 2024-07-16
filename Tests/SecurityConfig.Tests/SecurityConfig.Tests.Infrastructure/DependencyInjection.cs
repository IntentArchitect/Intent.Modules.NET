using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecurityConfig.Tests.Application.Common.Interfaces;
using SecurityConfig.Tests.Domain.Common.Interfaces;
using SecurityConfig.Tests.Domain.Repositories;
using SecurityConfig.Tests.Infrastructure.Configuration;
using SecurityConfig.Tests.Infrastructure.Persistence;
using SecurityConfig.Tests.Infrastructure.Repositories;
using SecurityConfig.Tests.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace SecurityConfig.Tests.Infrastructure
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
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}