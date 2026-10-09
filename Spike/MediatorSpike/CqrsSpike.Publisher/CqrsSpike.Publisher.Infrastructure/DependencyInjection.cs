using CqrsSpike.Publisher.Application.Common.Interfaces;
using CqrsSpike.Publisher.Domain.Common.Interfaces;
using CqrsSpike.Publisher.Domain.Repositories;
using CqrsSpike.Publisher.Infrastructure.Configuration;
using CqrsSpike.Publisher.Infrastructure.Persistence;
using CqrsSpike.Publisher.Infrastructure.Repositories;
using CqrsSpike.Publisher.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsSpike.Publisher.Infrastructure
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
            services.AddTransient<IAuditEntryRepository, AuditEntryRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}