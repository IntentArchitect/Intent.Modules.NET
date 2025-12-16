using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace GrpcServer.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
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
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}