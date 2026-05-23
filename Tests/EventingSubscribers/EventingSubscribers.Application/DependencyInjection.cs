using System.Reflection;
using EventingSubscribers.Application.Common.Behaviours;
using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Application.Implementation;
using EventingSubscribers.Application.IntegrationEvents.EventHandlers;
using EventingSubscribers.Application.Interfaces;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EventingSubscribers.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(MessageBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IIntegrationEventHandler<AccountUpgradedEvent>, AccountUpgradedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CommandInvokedEvent>, CommandInvokedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<EntityOperationInvokedEvent>, EntityOperationInvokedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<InvoiceCreatedEvent>, InvoiceCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ItemCreatedEvent>, ItemCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ProductCreatedEvent>, ProductCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ProductLookedUpEvent>, ProductLookedUpEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ProductRemovedEvent>, ProductRemovedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ServiceOperationInvokedEvent>, ServiceOperationInvokedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ShipmentLookedUpEvent>, ShipmentLookedUpEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ShipmentReceivedEvent>, ShipmentReceivedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<TaskAssignedEvent>, TaskAssignedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<TaskLookedUpEvent>, TaskLookedUpEventHandler>();
            return services;
        }
    }
}
