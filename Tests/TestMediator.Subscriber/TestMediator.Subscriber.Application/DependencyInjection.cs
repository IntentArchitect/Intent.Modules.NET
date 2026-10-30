using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.Publisher.Eventing.Messages;
using TestMediator.Subscriber.Application.Common.Eventing;
using TestMediator.Subscriber.Application.Common.Validation;
using TestMediator.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestMediator.Subscriber.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddScoped<MessageBusFlushOwner>();
            services.AddTransient<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<OrderCancelledEvent>, OrderRelayHandler>();
            services.AddTransient<IIntegrationEventHandler<OrderShippedEvent>, OrderShippedEventHandler>();
            return services;
        }
    }
}