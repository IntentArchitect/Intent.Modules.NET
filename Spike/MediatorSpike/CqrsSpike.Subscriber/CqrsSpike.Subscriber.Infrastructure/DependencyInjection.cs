using CqrsSpike.Subscriber.Domain.Common.Interfaces;
using CqrsSpike.Subscriber.Domain.Repositories;
using CqrsSpike.Subscriber.Infrastructure.Configuration;
using CqrsSpike.Subscriber.Infrastructure.Persistence;
using CqrsSpike.Subscriber.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsSpike.Subscriber.Infrastructure
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
            services.AddTransient<IOrderRecordRepository, OrderRecordRepository>();
            services.AddTransient<IShipmentRepository, ShipmentRepository>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}