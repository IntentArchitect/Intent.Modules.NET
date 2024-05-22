using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Behaviours;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Application.Common.Validation;
using MassTransit.RabbitMQ.Application.IntegrationEventHandlers;
using MassTransit.RabbitMQ.Application.IntegrationEvents.EventHandlers;
using MassTransit.RabbitMQ.Application.IntegrationEvents.EventHandlers.NamingOverrides;
using MassTransit.RabbitMQ.Application.IntegrationEvents.EventHandlers.Test;
using MassTransit.RabbitMQ.Eventing.Messages;
using MassTransit.RabbitMQ.Services;
using MassTransit.RabbitMQ.Services.Animals;
using MassTransit.RabbitMQ.Services.People;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application
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
                cfg.AddOpenBehavior(typeof(EventBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IIntegrationEventHandler<OrderAnimal>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<MakeSoundCommand>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<CreatePersonIdentity>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<TalkToPersonCommand>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeHandler>();
            services.AddTransient<IIntegrationEventHandler<OverrideMessageStandardSubscribeEvent>, StandardMessageCustomSubscribeHandler>();
            services.AddTransient<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeHandler>();
            services.AddTransient<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageHandler>();
            services.AddTransient<IIntegrationEventHandler<TestMessageEvent>, TestMessageEventHandler>();
            return services;
        }
    }
}