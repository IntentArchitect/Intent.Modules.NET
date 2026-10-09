using System.Reflection;
using CqrsSpike.Publisher.Eventing.Messages;
using CqrsSpike.Subscriber.Application.Common.Behaviours;
using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Application.Common.Validation;
using CqrsSpike.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;
using CqrsSpike.Subscriber.Eventing.Messages;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
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
            services.AddTransient<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<RequestShipmentCommand>, RequestShipmentCommandHandler>();
            return services;
        }
    }
}