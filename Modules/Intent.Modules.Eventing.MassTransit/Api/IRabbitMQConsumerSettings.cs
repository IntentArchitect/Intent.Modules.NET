namespace Intent.Eventing.MassTransit.Api;

public interface IRabbitMQConsumerSettings
{
    string Name { get; }
    int? PrefetchCount();
    bool Lazy();
    bool Durable();
    bool PurgeOnStartup();
    bool Exclusive();
}