using CompositePublishTest.Application.Common.Eventing;
using CompositePublishTest.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CompositePublishTest.Infrastructure.Configuration;

/// <summary>
/// Configuration for the Composite Message Bus and its message broker providers.
/// </summary>
public static class CompositeMessageBusConfiguration
{
    /// <summary>
    /// Configures the composite message bus infrastructure.
    /// Creates a central registry, configures all providers to register their message types,
    /// and sets up the resolver and bus.
    /// </summary>
    public static IServiceCollection ConfigureCompositeMessageBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Create the central registry instance (mutable during startup)
        var registry = new MessageBrokerRegistry();

        // Configure all message broker providers, passing the registry to each
        services.ConfigureEventGrid(configuration, registry);
        services.ConfigureAzureServiceBus(configuration, registry);
        services.ConfigureAzureQueueStorage(configuration, registry);
        services.ConfigureMassTransit(configuration, registry);
        services.ConfigureKafka(configuration, registry);
        services.ConfigureSolace(configuration, registry);

        // Register the now-populated registry as a singleton
        services.AddSingleton(registry);

        // Register the resolver (scoped, uses IServiceProvider to resolve providers per request)
        services.AddScoped<MessageBrokerResolver>();

        // Register CompositeMessageBus as IMessageBus (scoped per request)
        services.AddScoped<IMessageBus, CompositeMessageBus>();

        return services;
    }
}