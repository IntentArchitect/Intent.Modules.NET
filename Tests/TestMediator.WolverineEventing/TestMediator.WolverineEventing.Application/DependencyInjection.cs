using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.WolverineEventing.Application.Common.Eventing;
using TestMediator.WolverineEventing.Application.Common.Validation;
using TestMediator.WolverineEventing.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;
using TestMediator.WolverineEventing.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestMediator.WolverineEventing.Application
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