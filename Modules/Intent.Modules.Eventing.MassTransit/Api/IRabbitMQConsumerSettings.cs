namespace Intent.Eventing.MassTransit.Api;

// This interface will allow us to seamlessly use any "RabbitMQConsumerSettings" stereotype that's been assigned to various elements.
public interface IRabbitMQConsumerSettings
{
    string Name { get; }
    string EndpointName();
    int? PrefetchCount();
    bool Lazy();
    bool Durable();
    bool PurgeOnStartup();
    bool Exclusive();
    int? ConcurrentMessageLimit();
}