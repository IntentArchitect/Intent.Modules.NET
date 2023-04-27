using GraphQL.CQRS.TestApplication.Application.Common.Interfaces;
using GraphQL.CQRS.TestApplication.Domain.Common.Interfaces;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using GraphQL.CQRS.TestApplication.Infrastructure.Persistence;
using GraphQL.CQRS.TestApplication.Infrastructure.Repositories;
using GraphQL.CQRS.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Infrastructure
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
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}