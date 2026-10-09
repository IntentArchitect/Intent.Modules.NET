using System.Reflection;
using CqrsSpike.Publisher.Application.Common.Eventing;
using CqrsSpike.Publisher.Application.Common.Validation;
using CqrsSpike.Publisher.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;
using CqrsSpike.Subscriber.Eventing.Messages;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsSpike.Publisher.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IIntegrationEventHandler<ShipmentCreatedEvent>, ShipmentCreatedEventHandler>();
            return services;
        }
    }
}