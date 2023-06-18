using System.Reflection;
using AutoMapper;
using Entities.PrivateSetters.EF.CosmosDb.Application;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Common.Interfaces;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Repositories;
using Entities.PrivateSetters.EF.CosmosDb.Infrastructure.Persistence;
using Entities.PrivateSetters.EF.CosmosDb.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseCosmos(
                    configuration["Cosmos:AccountEndpoint"],
                    configuration["Cosmos:AccountKey"],
                    configuration["Cosmos:DatabaseName"]);
                options.UseLazyLoadingProxies();
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(Application.DependencyInjection).Assembly);
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            return services;
        }
    }
}