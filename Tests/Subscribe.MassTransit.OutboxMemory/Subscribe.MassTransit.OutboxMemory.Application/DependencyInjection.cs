using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.BasketItems;
using MassTransit.Messages.Shared.Baskets;
using MassTransit.Messages.Shared.Roles;
using MassTransit.Messages.Shared.Scheduled;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Behaviours;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Validation;
using Subscribe.MassTransit.OutboxMemory.Application.IntegrationEvents.EventHandlers;

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
                cfg.AddOpenBehavior(typeof(MessageBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<DelayedNotificationEvent>, DelayedNotificationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<RoleCreatedEvent>, RoleCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<RoleDeletedEvent>, RoleDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<RoleUpdatedEvent>, RoleUpdatedEventHandler>();
            return services;
        }
    }
}