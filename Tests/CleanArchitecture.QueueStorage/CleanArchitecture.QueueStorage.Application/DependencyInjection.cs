using System.Reflection;
using AzureFunction.QueueStorage.Eventing.Messages;
using CleanArchitecture.QueueStorage.Application.Common.Behaviours;
using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using CleanArchitecture.QueueStorage.Application.Common.Validation;
using CleanArchitecture.QueueStorage.Application.IntegrationEvents.EventHandlers;
using CleanArchitecture.QueueStorage.Eventing.Messages;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Application
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
            services.AddTransient<IIntegrationEventHandler<CreateStockForProductCommand>, CreateStockForProductCommandHandler>();
            services.AddTransient<IIntegrationEventHandler<ProductCreatedEvent>, ProductCreatedEventHandler>();
            return services;
        }
    }
}