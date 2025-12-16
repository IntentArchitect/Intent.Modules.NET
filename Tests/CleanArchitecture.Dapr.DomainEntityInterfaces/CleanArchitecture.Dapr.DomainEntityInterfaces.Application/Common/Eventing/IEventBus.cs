using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace CleanArchitecture.Dapr.DomainEntityInterfaces.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
        void Send<TMessage>(TMessage message)
            where TMessage : class;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}