namespace CqrsSpike.Subscriber.Application.Common.Eventing
{
    public interface IIntegrationEventHandler<in TMessage>
        where TMessage : class
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}