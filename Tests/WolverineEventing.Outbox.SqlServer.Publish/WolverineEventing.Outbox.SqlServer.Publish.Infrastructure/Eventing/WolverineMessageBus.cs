using Intent.RoslynWeaver.Attributes;
using WolverineBus = Wolverine.IMessageBus;
using ContractsMessageBus = WolverineEventing.Outbox.SqlServer.Publish.Application.Common.Eventing.IMessageBus;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineMessageBus", Version = "1.0")]

namespace WolverineEventing.Outbox.SqlServer.Publish.Infrastructure.Eventing
{
    public class WolverineMessageBus : ContractsMessageBus
    {
        private readonly List<Func<WolverineBus, ValueTask>> _pendingActions = new();
        private readonly WolverineBus _bus;

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