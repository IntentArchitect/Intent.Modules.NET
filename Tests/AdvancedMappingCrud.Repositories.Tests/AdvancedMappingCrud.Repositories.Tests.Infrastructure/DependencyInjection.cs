using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure
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
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IQuoteRepository, QuoteRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}