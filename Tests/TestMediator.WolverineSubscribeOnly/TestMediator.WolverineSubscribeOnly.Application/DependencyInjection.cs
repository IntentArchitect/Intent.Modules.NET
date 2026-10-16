using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.WolverineSubscribeOnly.Application.Common.Eventing;
using TestMediator.WolverineSubscribeOnly.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;
using TestMediator.WolverineSubscribeOnly.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestMediator.WolverineSubscribeOnly.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<MessageBusFlushOwner>();
            services.AddTransient<IIntegrationEventHandler<OrderPlacedEvent>, OrderPlacedEventHandler>();
            return services;
        }
    }
}