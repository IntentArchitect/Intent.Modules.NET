using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
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
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EventBusPublishBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehaviour<,>));
            services.AddTransient<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEventHandler>();
            return services;
        }
    }
}