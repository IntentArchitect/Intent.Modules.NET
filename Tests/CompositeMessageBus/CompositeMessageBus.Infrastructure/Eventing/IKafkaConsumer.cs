using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConsumerInterface", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public interface IKafkaConsumer
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}