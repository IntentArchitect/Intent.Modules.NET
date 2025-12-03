using System.Collections.Concurrent;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaProducerInterface", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public interface IKafkaProducer<TValue>
        where TValue : class
    {
        Task Produce(ConcurrentQueue<TValue> messageQueue, CancellationToken cancellationToken = default);
    }
}