using System.Reflection;
using AutoMapper;
using AzureFunctions.AzureEventGrid.Application.Common.Behaviours;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Application.Common.Validation;
using AzureFunctions.AzureEventGrid.Application.Implementation;
using AzureFunctions.AzureEventGrid.Application.IntegrationEvents.EventHandlers;
using AzureFunctions.AzureEventGrid.Application.Interfaces;
using AzureFunctions.AzureEventGrid.EventDomain;
using AzureFunctions.AzureEventGrid.GroupA.Eventing.Messages;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application
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
            services.AddTransient<ISpecificChannelService, SpecificChannelService>();
            services.AddTransient<IIntegrationEventHandler<ClientCreatedEvent>, ClientCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
            return services;
        }
    }
}