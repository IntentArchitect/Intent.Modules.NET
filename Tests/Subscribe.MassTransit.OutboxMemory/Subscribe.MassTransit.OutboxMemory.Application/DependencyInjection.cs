using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Behaviours;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;
using Subscribe.MassTransit.OutboxMemory.Application.IntegrationEventHandlers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxMemory.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(EventBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<RoleCreatedEvent>, RoleCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<RoleUpdatedEvent>, RoleUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<RoleDeletedEvent>, RoleDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<DelayedNotificationEvent>, DelayedNotificationEventHandler>();
            return services;
        }
    }
}