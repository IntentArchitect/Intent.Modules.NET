using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Application.Common.Interfaces;
using MassTransit.RabbitMQ.Application.IntegrationServices;
using MassTransit.RabbitMQ.Domain.Common.Interfaces;
using MassTransit.RabbitMQ.Domain.Repositories;
using MassTransit.RabbitMQ.Infrastructure.Configuration;
using MassTransit.RabbitMQ.Infrastructure.Eventing;
using MassTransit.RabbitMQ.Infrastructure.Persistence;
using MassTransit.RabbitMQ.Infrastructure.Repositories;
using MassTransit.RabbitMQ.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RabbitMQ.Infrastructure
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
            services.AddTransient<IAnimalRepository, AnimalRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<ICQRSService, CQRSService>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}