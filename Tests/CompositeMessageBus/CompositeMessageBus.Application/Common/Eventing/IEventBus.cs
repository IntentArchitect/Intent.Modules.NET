using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace CompositeMessageBus.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
        void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class;
        void Send<TMessage>(TMessage message)
            where TMessage : class;
        void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
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
        Task FlushAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Queues a message to be published at a specific scheduled UTC time.
        /// </summary>
        /// <typeparam name="TMessage">The concrete message type.</typeparam>
        /// <param name="message">The message instance to publish.</param>
        /// <param name="scheduled">The UTC date/time when the message should be dispatched (MassTransit scheduling capability).</param>
        /// <remarks>
        /// Scheduling is a provider-specific feature; if the underlying implementation does not support scheduling this overload may be ignored.
        /// The message will be buffered until <see cref="FlushAllAsync"/> is invoked (at which point the schedule instruction is applied by the provider).
        /// </remarks>
        void SchedulePublish<TMessage>(TMessage message, DateTime scheduled)
            where TMessage : class;
        /// <summary>
        /// Queues a message to be published after a delay relative to the current UTC time.
        /// </summary>
        /// <typeparam name="TMessage">The concrete message type.</typeparam>
        /// <param name="message">The message instance to publish.</param>
        /// <param name="delay">The time span to add to the current UTC time for scheduling (MassTransit scheduling capability).</param>
        /// <remarks>
        /// Scheduling is provider-specific; if unsupported this overload may be ignored. The actual dispatch occurs when <see cref="FlushAllAsync"/> is called and the provider processes the schedule metadata.
        /// </remarks>
        void SchedulePublish<TMessage>(TMessage message, TimeSpan delay)
            where TMessage : class;
    }
}