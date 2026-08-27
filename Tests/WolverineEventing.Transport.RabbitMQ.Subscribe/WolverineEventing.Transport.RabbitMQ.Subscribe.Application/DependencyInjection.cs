using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Application.Common.Eventing;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Application.Common.Validation;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Application.IntegrationEvents.EventHandlers;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace WolverineEventing.Transport.RabbitMQ.Subscribe.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ProcessOrderCommand>, ProcessOrderCommandHandler>();
            return services;
        }
    }
}