using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using Wolverine;
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
        private readonly IMultiTenantContextAccessor _multiTenantContextAccessor;

        public WolverineMessageBus(WolverineBus bus, IMultiTenantContextAccessor multiTenantContextAccessor)
        {
            _bus = bus;
            _multiTenantContextAccessor = multiTenantContextAccessor;
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(bus => bus.PublishAsync(message, BuildDeliveryOptions()));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(bus => bus.SendAsync(message, BuildDeliveryOptions()));
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

        private DeliveryOptions? BuildDeliveryOptions()
        {
            var tenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier;

            return tenantIdentifier is null ? null : new DeliveryOptions { TenantId = tenantIdentifier };
        }
    }
}