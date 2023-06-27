using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.OutboxMemory.Application;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Interfaces;
using Subscribe.MassTransit.OutboxMemory.Domain.Common.Interfaces;
using Subscribe.MassTransit.OutboxMemory.Infrastructure.Configuration;
using Subscribe.MassTransit.OutboxMemory.Infrastructure.Eventing;
using Subscribe.MassTransit.OutboxMemory.Infrastructure.Persistence;
using Subscribe.MassTransit.OutboxMemory.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxMemory.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<MassTransitEventBus>();
            services.AddTransient<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}