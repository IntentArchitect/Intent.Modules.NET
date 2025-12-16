using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
        /// <summary>
        /// Queues a message to be published with additional metadata (Azure Event Grid-specific extension attributes).
        /// </summary>
        /// <typeparam name="TMessage">The concrete message type.</typeparam>
        /// <param name="message">The message instance to publish.</param>
        /// <param name="additionalData">Additional metadata to include with the message (e.g., Subject, extension attributes).</param>
        /// <remarks>
        /// The message is buffered until <see cref="FlushAllAsync"/> is invoked. Providers that do not support additional data may ignore this overload.
        /// </remarks>
        void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class;
        void Send<TMessage>(TMessage message)
            where TMessage : class;
        /// <summary>
        /// Queues a point-to-point message with additional metadata (Azure Event Grid-specific extension attributes).
        /// </summary>
        /// <typeparam name="TMessage">The concrete message type.</typeparam>
        /// <param name="message">The message instance to send.</param>
        /// <param name="additionalData">Additional metadata to include with the message (e.g., Subject, extension attributes).</param>
        /// <remarks>
        /// The message is buffered until <see cref="FlushAllAsync"/> is invoked. Providers that do not support additional data may ignore this overload.
        /// </remarks>
        void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}