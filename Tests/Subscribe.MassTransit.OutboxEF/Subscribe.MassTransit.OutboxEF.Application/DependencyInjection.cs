using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.Orders;
using MassTransit.Messages.Shared.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.OutboxEF.Application.Common.Behaviours;
using Subscribe.MassTransit.OutboxEF.Application.Common.Eventing;
using Subscribe.MassTransit.OutboxEF.Application.Common.Validation;
using Subscribe.MassTransit.OutboxEF.Application.IntegrationEvents.EventHandlers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Application
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
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<OrderDeletedEvent>, OrderDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<OrderUpdatedEvent>, OrderUpdatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<UserDeletedEvent>, UserDeletedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<UserUpdatedEvent>, UserUpdatedEventHandler>();
            return services;
        }
    }
}