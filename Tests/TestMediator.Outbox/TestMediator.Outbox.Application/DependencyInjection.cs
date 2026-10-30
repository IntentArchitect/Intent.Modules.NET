using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.Outbox.Application.Common.Eventing;
using TestMediator.Outbox.Application.Common.Validation;
using TestMediator.Outbox.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;
using TestMediator.Outbox.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestMediator.Outbox.Application
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
            services.AddTransient<IIntegrationEventHandler<OrderPlacedEvent>, OrderPlacedEventHandler>();
            return services;
        }
    }
}