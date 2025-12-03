using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.SolaceEventDispatcherInterface", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public interface ISolaceEventDispatcher<TMessage>
        where TMessage : class
    {
        Task Dispatch(TMessage message, CancellationToken cancellationToken);
    }
}