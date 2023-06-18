using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Common.Interfaces;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Configuration;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Eventing;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Persistence;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure
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
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(Application.DependencyInjection).Assembly);
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<MassTransitEventBus>();
            services.AddTransient<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}