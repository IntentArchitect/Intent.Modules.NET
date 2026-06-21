using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N_ServiceBus.AzureServiceBus.Application.Common.Behaviours;
using N_ServiceBus.AzureServiceBus.Application.Common.Eventing;
using N_ServiceBus.AzureServiceBus.Application.Common.Validation;
using N_ServiceBus.AzureServiceBus.Application.Implementation.Animals;
using N_ServiceBus.AzureServiceBus.Application.IntegrationEvents.EventHandlers;
using N_ServiceBus.AzureServiceBus.Application.Interfaces.Animals;
using N_ServiceBus.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace N_ServiceBus.AzureServiceBus.Application
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
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAnimalsService, AnimalsService>();
            services.AddTransient<IIntegrationEventHandler<OrderAnimal>, OrderAnimalHandler>();
            services.AddTransient<IIntegrationEventHandler<MakeSoundCommand>, OrderAnimalHandler>();
            services.AddTransient<IIntegrationEventHandler<TalkToPersonCommand>, OrderAnimalHandler>();
            services.AddTransient<IIntegrationEventHandler<CreatePersonIdentity>, OrderAnimalHandler>();
            services.AddTransient<IIntegrationEventHandler<TestMessageEvent>, TestMessageHandler>();
            return services;
        }
    }
}