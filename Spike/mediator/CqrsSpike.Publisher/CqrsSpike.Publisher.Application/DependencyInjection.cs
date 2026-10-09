using System.Reflection;
using CqrsSpike.Publisher.Application.Common.Behaviours;
using CqrsSpike.Publisher.Application.Common.Eventing;
using CqrsSpike.Publisher.Application.Common.Validation;
using CqrsSpike.Publisher.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;
using CqrsSpike.Subscriber.Eventing.Messages;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CqrsSpike.Publisher.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddMediator((MediatorOptions options) =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.PipelineBehaviors =
                [
                    typeof(LoggingBehaviour<,>),
                    typeof(UnhandledExceptionBehaviour<,>),
                    typeof(PerformanceBehaviour<,>),
                    typeof(AuthorizationBehaviour<,>),
                    typeof(MessageBusPublishBehaviour<,>),
                    typeof(ValidationBehaviour<,>),
                    typeof(UnitOfWorkBehaviour<,>)
                ];
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IIntegrationEventHandler<ShipmentCreatedEvent>, ShipmentCreatedEventHandler>();
            return services;
        }
    }
}