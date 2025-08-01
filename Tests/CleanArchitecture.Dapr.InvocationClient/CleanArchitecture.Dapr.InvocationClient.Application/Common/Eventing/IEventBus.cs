using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(T message)
            where T : class, IEvent;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}