using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.MessageBusInterface", Version = "1.0")]

namespace MassTransit.RequestResponse.Client.Application.Common.Eventing
{
    /// <summary>
    /// Provides an abstraction for dispatching messages to one or more underlying message brokers.
    /// Messages are queued via <see cref="Publish{TMessage}"/> (fan-out / broadcast semantics) or <see cref="Send{TMessage}"/> (point-to-point semantics) and are flushed in batches when <see cref="FlushAllAsync"/> is invoked.
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Queues a message to be published to all interested subscribers (topic / fan-out semantics).
        /// </summary>
        /// <typeparam name="TMessage">The concrete message type.</typeparam>
        /// <param name="message">The message instance to publish.</param>
        /// <remarks>
        /// The message is buffered until <see cref="FlushAllAsync"/> is called.
        /// </remarks>
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
        /// <summary>
        /// Queues a message for point-to-point delivery to a single consumer endpoint.
        /// </summary>
        /// <typeparam name="TMessage">The concrete message type.</typeparam>
        /// <param name="message">The message instance to send.</param>
        /// <remarks>
        /// The message is buffered until <see cref="FlushAllAsync"/> is called. Use send for commands or direct messages; use publish for events.
        /// </remarks>
        void Send<TMessage>(TMessage message)
            where TMessage : class;
        /// <summary>
        /// Queues a point-to-point message for dispatch to a specific broker address.
        /// </summary>
        /// <typeparam name="TMessage">The concrete message type.</typeparam>
        /// <param name="message">The message instance to send.</param>
        /// <param name="address">The destination address understood by providers that support explicit addressing (MassTransit-specific concept).</param>
        /// <remarks>
        /// The message is buffered until <see cref="FlushAllAsync"/> is invoked. Providers that do not support explicit addressing may ignore this overload.
        /// </remarks>
        void Send<TMessage>(TMessage message, Uri address)
            where TMessage : class;
        /// <summary>
        /// Flushes and dispatches all queued messages to the underlying broker(s).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
        /// <remarks>
        /// Implementations may optimize by batching publish/send operations. Internal buffers are typically cleared after a successful flush.
        /// </remarks>
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}