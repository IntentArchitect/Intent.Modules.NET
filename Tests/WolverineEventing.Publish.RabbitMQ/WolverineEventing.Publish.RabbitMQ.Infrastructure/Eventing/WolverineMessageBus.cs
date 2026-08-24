using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContractsMessageBus = WolverineEventing.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;
using WolverineBus = Wolverine.IMessageBus;

namespace WolverineEventing.Publish.RabbitMQ.Infrastructure.Eventing
{
    /// <summary>
    /// Implements the Eventing Contracts message bus over Wolverine's own bus. Transactional Outbox
    /// = None, so messages go straight to the transport with no durable store and no database.
    /// </summary>
    /// <remarks>
    /// Two different interfaces are called IMessageBus here - the Intent Eventing Contracts one this
    /// class implements, and Wolverine's own that it delegates to. They are aliased at the using
    /// site rather than qualified inline, so which is which is unambiguous on sight. A developer who
    /// injects Wolverine's by mistake bypasses the Composite Message Bus entirely.
    /// </remarks>
    public class WolverineMessageBus : ContractsMessageBus
    {
        private readonly WolverineBus _bus;

        // PublishAsync/SendAsync return ValueTask, not Task.
        private readonly List<Func<WolverineBus, ValueTask>> _pendingActions = new();

        public WolverineMessageBus(WolverineBus bus)
        {
            _bus = bus;
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(bus => bus.PublishAsync(message));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(bus => bus.SendAsync(message));
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_pendingActions.Count == 0)
            {
                return;
            }

            var toFlush = new List<Func<WolverineBus, ValueTask>>(_pendingActions);
            _pendingActions.Clear();

            foreach (var action in toFlush)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await action(_bus);
            }
        }
    }
}
