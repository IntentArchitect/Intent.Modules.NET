using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.RabbitMQ.Application.Common.Behaviours;
using NServiceBus.RabbitMQ.Application.Common.Eventing;
using NServiceBus.RabbitMQ.Application.Common.Validation;
using NServiceBus.RabbitMQ.Application.Implementation.Animals;
using NServiceBus.RabbitMQ.Application.IntegrationEvents.EventHandlers;
using NServiceBus.RabbitMQ.Application.Interfaces.Animals;
using NServiceBus.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace NServiceBus.RabbitMQ.Application
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