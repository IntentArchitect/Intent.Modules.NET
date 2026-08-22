using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContractsMessageBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;
using WolverineBus = Wolverine.IMessageBus;

namespace Wolverine.Publish.RabbitMQ.Infrastructure.Eventing
{
    /// <summary>
    /// Transactional Outbox = None. Publishes and sends straight through Wolverine's own bus with
    /// no durable store and no database, which is the module's default configuration.
    /// </summary>
    /// <remarks>
    /// R8.3: two different interfaces are both called <c>IMessageBus</c> here - the Intent Eventing
    /// Contracts one this class implements, and Wolverine's own that it delegates to. They are
    /// aliased at the using site (<c>ContractsMessageBus</c> / <c>WolverineBus</c>) rather than
    /// fully qualified inline, so which one is which is unambiguous on sight. A developer following
    /// a Wolverine tutorial who injects the wrong one bypasses the Composite Message Bus entirely.
    /// </remarks>
    public class WolverineMessageBus : ContractsMessageBus
    {
        private readonly WolverineBus _bus;

        // Wolverine's IMessageBus.PublishAsync/SendAsync return ValueTask, not Task - verified by
        // the compiler, not assumed.
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

            var actionsToFlush = new List<Func<WolverineBus, ValueTask>>(_pendingActions);
            _pendingActions.Clear();

            foreach (var action in actionsToFlush)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await action(_bus);
            }
        }
    }
}
