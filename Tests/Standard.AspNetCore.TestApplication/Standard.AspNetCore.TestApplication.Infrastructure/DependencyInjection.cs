using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Application;
using Standard.AspNetCore.TestApplication.Domain.Common.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Repositories;
using Standard.AspNetCore.TestApplication.Infrastructure.Configuration;
using Standard.AspNetCore.TestApplication.Infrastructure.Persistence;
using Standard.AspNetCore.TestApplication.Infrastructure.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure
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
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IPluralsRepository, PluralsRepository>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}