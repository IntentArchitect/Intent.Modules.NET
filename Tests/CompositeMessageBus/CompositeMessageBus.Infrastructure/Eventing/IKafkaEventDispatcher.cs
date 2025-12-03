using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaEventDispatcherInterface", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public interface IKafkaEventDispatcher<in T>
        where T : class
    {
        Task Dispatch(T message, CancellationToken cancellationToken = default);
    }
}