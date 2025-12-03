using System.Reflection;
using AzureFunctions.AzureServiceBus.Application.Common.Behaviours;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Application.Common.Validation;
using AzureFunctions.AzureServiceBus.Application.IntegrationEvents.EventHandlers;
using AzureFunctions.AzureServiceBus.GroupB.Eventing.Messages;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application
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
                cfg.AddOpenBehavior(typeof(MessageBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IIntegrationEventHandler<SpecificQueueOneMessageEvent>, SpecificQueueMessageHandler>();
            services.AddTransient<IIntegrationEventHandler<SpecificQueueTwoMessageEvent>, SpecificQueueMessageHandler>();
            services.AddTransient<IIntegrationEventHandler<SpecificTopicOneMessageEvent>, SpecificTopicMessageHandler>();
            services.AddTransient<IIntegrationEventHandler<SpecificTopicTwoMessageEvent>, SpecificTopicMessageHandler>();
            return services;
        }
    }
}