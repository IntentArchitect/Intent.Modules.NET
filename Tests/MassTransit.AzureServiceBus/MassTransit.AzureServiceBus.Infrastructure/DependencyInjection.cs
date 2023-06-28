using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Application.Common.Interfaces;
using MassTransit.AzureServiceBus.Domain.Common.Interfaces;
using MassTransit.AzureServiceBus.Infrastructure.Configuration;
using MassTransit.AzureServiceBus.Infrastructure.Eventing;
using MassTransit.AzureServiceBus.Infrastructure.Persistence;
using MassTransit.AzureServiceBus.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure
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
            services.AddScoped<MassTransitEventBus>();
            services.AddTransient<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}