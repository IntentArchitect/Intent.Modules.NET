using Mediator;

namespace CqrsSpike.Publisher.Infrastructure.Configuration
{
    /// <summary>
    /// Runs notification handlers one at a time and stops at the first failure, rethrowing the
    /// handler's own exception (MediatR's default semantics). Mediator's built-in
    /// <see cref="ForeachAwaitPublisher"/> keeps running the remaining handlers and wraps failures
    /// in an <see cref="AggregateException"/>, which the API exception mapping turns into a 500.
    /// </summary>
    public sealed class SequentialNotificationPublisher : INotificationPublisher
    {
        public async ValueTask Publish<TNotification>(
            NotificationHandlers<TNotification> handlers,
            TNotification notification,
            CancellationToken cancellationToken)
            where TNotification : INotification
        {
            foreach (var handler in handlers)
            {
                await handler.Handle(notification, cancellationToken);
            }
        }
    }
}
