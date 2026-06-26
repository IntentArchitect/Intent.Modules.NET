using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N_ServiceBus.Persistence.NHibernate.Publish.Eventing.Messages;
using N_ServiceBus.Persistence.NHibernate.Subscribe.Application.Common.Behaviours;
using N_ServiceBus.Persistence.NHibernate.Subscribe.Application.Common.Eventing;
using N_ServiceBus.Persistence.NHibernate.Subscribe.Application.Common.Validation;
using N_ServiceBus.Persistence.NHibernate.Subscribe.Application.IntegrationEvents.EventHandlers;
using N_ServiceBus.Persistence.NHibernate.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace N_ServiceBus.Persistence.NHibernate.Subscribe.Application
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
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageHandler>();
            services.AddTransient<IIntegrationEventHandler<TestCommand>, TestCommandHandler>();
            services.AddTransient<IIntegrationEventHandler<TestEvent>, TestEventHandler>();
            return services;
        }
    }
}