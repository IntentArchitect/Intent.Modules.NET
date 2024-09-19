using System.Reflection;
using AutoMapper;
using AzureFunctions.TestApplication.Application;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using AzureFunctions.TestApplication.Domain.Repositories;
using AzureFunctions.TestApplication.Infrastructure.Configuration;
using AzureFunctions.TestApplication.Infrastructure.Persistence;
using AzureFunctions.TestApplication.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.TestApplication.Infrastructure
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
            services.AddTransient<ISampleDomainRepository, SampleDomainRepository>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}