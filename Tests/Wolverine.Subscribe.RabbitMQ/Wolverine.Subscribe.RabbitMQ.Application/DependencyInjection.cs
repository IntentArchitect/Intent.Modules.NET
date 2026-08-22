using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Publish.RabbitMQ.Eventing.Messages;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Eventing;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Validation;
using Wolverine.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Wolverine.Subscribe.RabbitMQ.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IIntegrationEventHandler<OrderShippedEvent>, OrderShippedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ProcessOrderCommand>, ProcessOrderCommandHandler>();
            return services;
        }
    }
}