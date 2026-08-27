using Intent.RoslynWeaver.Attributes;
using WolverineBus = Wolverine.IMessageBus;
using ContractsMessageBus = WolverineEventing.MultiTenancy.Application.Common.Eventing.IMessageBus;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineMessageBus", Version = "1.0")]

namespace WolverineEventing.MultiTenancy.Infrastructure.Eventing
{
    public class WolverineMessageBus : ContractsMessageBus
    {
        private readonly List<Func<WolverineBus, ValueTask>> _pendingActions = new();
        private readonly WolverineBus _bus;
        private readonly WolverineTenantHeaderStrategy _tenantHeaderStrategy;

        public WolverineMessageBus(WolverineBus bus, WolverineTenantHeaderStrategy tenantHeaderStrategy)
        {
            _bus = bus;
            _tenantHeaderStrategy = tenantHeaderStrategy;
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(bus => bus.PublishAsync(message, _tenantHeaderStrategy.BuildDeliveryOptions()));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(bus => bus.SendAsync(message, _tenantHeaderStrategy.BuildDeliveryOptions()));
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