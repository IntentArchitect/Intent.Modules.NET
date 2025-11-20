using System;
using CompositePublishTest.Eventing.Messages;
using CompositePublishTest.Infrastructure.Eventing;
using CompositePublishTest.Infrastructure.Eventing.Kafka;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConfiguration", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Configuration;

/// <summary>
/// Configuration for Kafka message broker provider.
/// Sets up Confluent Kafka client with schema registry support and Composite Message Bus integration.
/// </summary>
public static class KafkaConfiguration
{
    /// <summary>
    /// Configures Kafka infrastructure for the Composite Message Bus.
    /// </summary>
    public static IServiceCollection ConfigureKafka(
        this IServiceCollection services,
        IConfiguration configuration,
        MessageBrokerRegistry registry)
    {
        // Register Schema Registry client for message serialization
        var registryConfig = new SchemaRegistryConfig
        {
            Url = configuration["Kafka:SchemaRegistry:Url"] ?? "http://localhost:8081"
        };
        var schemaRegistryClient = new CachedSchemaRegistryClient(registryConfig);
        services.AddSingleton<ISchemaRegistryClient>(schemaRegistryClient);

        // Register default Kafka producer configuration
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            AllowAutoCreateTopics = configuration.GetValue<bool?>("Kafka:AllowAutoCreateTopics") ?? true,
            Acks = Acks.All,
            CompressionType = CompressionType.Snappy
        };
        services.AddSingleton(producerConfig);

        // Register Kafka provider as scoped
        services.AddScoped<KafkaMessageBus>();

        // Example registrations (users should add their own here or in a separate config block)
        services.AddKafkaProducer<string, ClientCreatedEvent>("client-events", evt => evt.Name.ToString(), registry);

        return services;
    }

    /// <summary>
    /// Registers a Kafka producer for a specific message type.
    /// </summary>
    /// <typeparam name="TKey">The type of the Kafka message key</typeparam>
    /// <typeparam name="TValue">The type of the message (value)</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="topicName">The topic name (optional, defaults to type name or config)</param>
    /// <param name="keyProvider">Function to extract the key from the message</param>
    /// <param name="registry">The central message broker registry</param>
    public static IServiceCollection AddKafkaProducer<TKey, TValue>(
        this IServiceCollection services,
        string? topicName,
        Func<TValue, TKey>? keyProvider,
        MessageBrokerRegistry registry)
        where TValue : class
    {
        // Register the producer as a singleton
        services.AddSingleton<IKafkaProducer<TValue>>(sp => new KafkaProducer<TKey, TValue>(
            sp.GetRequiredService<ISchemaRegistryClient>(),
            keyProvider,
            sp.GetRequiredService<IConfiguration>(),
            sp.GetRequiredService<ProducerConfig>(),
            topicName,
            sp.GetRequiredService<ILogger<KafkaProducer<TKey, TValue>>>()));

        // Register the type in the registry so the provider knows about it
        services.AddSingleton<IKafkaMessageRegistration>(new KafkaMessageRegistration(typeof(TValue)));

        // Register message type in the central registry
        registry.Register<TValue, KafkaMessageBus>();

        return services;
    }
}

public interface IKafkaMessageRegistration
{
    Type MessageType { get; }
}

public class KafkaMessageRegistration : IKafkaMessageRegistration
{
    public KafkaMessageRegistration(Type messageType) => MessageType = messageType;
    public Type MessageType { get; }
}
