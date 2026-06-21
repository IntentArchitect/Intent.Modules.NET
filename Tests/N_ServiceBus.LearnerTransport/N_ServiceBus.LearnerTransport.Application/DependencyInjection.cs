using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N_ServiceBus.LearnerTransport.Application.Common.Behaviours;
using N_ServiceBus.LearnerTransport.Application.Common.Eventing;
using N_ServiceBus.LearnerTransport.Application.Common.Validation;
using N_ServiceBus.LearnerTransport.Application.Implementation.Animals;
using N_ServiceBus.LearnerTransport.Application.IntegrationEvents.EventHandlers;
using N_ServiceBus.LearnerTransport.Application.Interfaces.Animals;
using N_ServiceBus.LearnerTransport.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.Application
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