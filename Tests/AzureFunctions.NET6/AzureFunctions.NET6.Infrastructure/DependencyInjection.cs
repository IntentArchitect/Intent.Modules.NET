using System.Reflection;
using AutoMapper;
using AzureFunctions.NET6.Application.Common.Interfaces;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using AzureFunctions.NET6.Domain.Repositories;
using AzureFunctions.NET6.Infrastructure.Caching;
using AzureFunctions.NET6.Infrastructure.Configuration;
using AzureFunctions.NET6.Infrastructure.Persistence;
using AzureFunctions.NET6.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.NET6.Infrastructure
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
            services.AddDistributedMemoryCache();
            services.AddSingleton<IDistributedCacheWithUnitOfWork, DistributedCacheWithUnitOfWork>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ISampleDomainRepository, SampleDomainRepository>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}