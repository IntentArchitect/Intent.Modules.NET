using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaProducerInterface", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Eventing.Kafka
{
    public interface IKafkaProducer<TValue>
        where TValue : class
    {
        Task Produce(ConcurrentQueue<TValue> messageQueue, CancellationToken cancellationToken = default);
    }
}
