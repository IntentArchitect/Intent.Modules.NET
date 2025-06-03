using System.Reflection;
using AspNetCore.AzureServiceBus.GroupA.Eventing.Messages;
using AspNetCore.AzureServiceBus.GroupB.Application.Common.Behaviours;
using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupB.Application.IntegrationEvents.EventHandlers;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(EventBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddTransient<IIntegrationEventHandler<ClientCreatedEvent>, ClientCreatedEventHandler>();
            return services;
        }
    }
}