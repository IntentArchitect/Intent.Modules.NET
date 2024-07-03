using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Behaviours;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Application.Common.Validation;
using MassTransit.AzureServiceBus.Application.IntegrationEventHandlers;
using MassTransit.AzureServiceBus.Application.IntegrationEvents.EventHandlers;
using MassTransit.AzureServiceBus.Application.IntegrationEvents.EventHandlers.Configuration;
using MassTransit.AzureServiceBus.Application.IntegrationEvents.EventHandlers.NamingOverrides;
using MassTransit.AzureServiceBus.Application.IntegrationEvents.EventHandlers.Test;
using MassTransit.AzureServiceBus.Eventing.Messages;
using MassTransit.AzureServiceBus.Services;
using MassTransit.AzureServiceBus.Services.Animals;
using MassTransit.AzureServiceBus.Services.People;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application
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
            services.AddTransient<IIntegrationEventHandler<ConfigTestMessageEvent>, ConfigTestMessageHandler>();
            services.AddTransient<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, ReceiveConsumerHandler>();
            services.AddTransient<IIntegrationEventHandler<OverrideMessageStandardSubscribeEvent>, ReceiveConsumerHandler>();
            services.AddTransient<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, ReceiveConsumerHandler>();
            services.AddTransient<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, SubscriptionConsumerHandler>();
            services.AddTransient<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, SubscriptionConsumerHandler>();
            services.AddTransient<IIntegrationEventHandler<AnotherTestMessageEvent>, TestMessageHandler>();
            services.AddTransient<IIntegrationEventHandler<TestMessageEvent>, TestMessageEventHandler>();
            return services;
        }
    }
}