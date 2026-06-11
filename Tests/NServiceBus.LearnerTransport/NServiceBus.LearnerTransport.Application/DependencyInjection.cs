using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.LearnerTransport.Application.Common.Behaviours;
using NServiceBus.LearnerTransport.Application.Common.Eventing;
using NServiceBus.LearnerTransport.Application.Common.Validation;
using NServiceBus.LearnerTransport.Application.Implementation.Animals;
using NServiceBus.LearnerTransport.Application.IntegrationEvents.EventHandlers;
using NServiceBus.LearnerTransport.Application.Interfaces.Animals;
using NServiceBus.LearnerTransport.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace NServiceBus.LearnerTransport.Application
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
            services.AddTransient<IIntegrationEventHandler<OrderAnimal>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<MakeSoundCommand>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<TalkToPersonCommand>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<CreatePersonIdentity>, CatchAllHandler>();
            services.AddTransient<IIntegrationEventHandler<TestMessageEvent>, TestMessageHandler>();
            return services;
        }
    }
}