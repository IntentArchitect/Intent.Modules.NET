using CleanArchitecture.OnlyModeledDomainEvents.Application.Common.Interfaces;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Common.Interfaces;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories;
using CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence;
using CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Repositories;
using CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<ICustomerRepository, CustomerCosmosDBRepository>();
            services.AddScoped<IOrderRepository, OrderCosmosDBRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddTransient<IAgg1Repository, Agg1Repository>();
            services.AddTransient<IAgg2Repository, Agg2Repository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}