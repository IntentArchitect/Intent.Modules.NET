using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaEventDispatcherInterface", Version = "1.0")]

namespace Kafka.Consumer.Infrastructure.Eventing
{
    public interface IKafkaEventDispatcher<in T>
        where T : class
    {
        Task Dispatch(T message, CancellationToken cancellationToken = default);
    }
}