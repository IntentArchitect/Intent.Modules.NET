using System.Reflection;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Application;
using GraphQL.CQRS.TestApplication.Domain.Common.Interfaces;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using GraphQL.CQRS.TestApplication.Infrastructure.Persistence;
using GraphQL.CQRS.TestApplication.Infrastructure.Repositories;
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
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProfitCenterRepository, ProfitCenterRepository>();
            return services;
        }
    }
}