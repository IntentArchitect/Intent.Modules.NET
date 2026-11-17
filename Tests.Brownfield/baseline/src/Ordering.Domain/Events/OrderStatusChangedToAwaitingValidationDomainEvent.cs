using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Domain.Guardrails.Contracts.DomainEventContract", Version = "1.0")]

namespace eShop.Ordering.Domain.Events;

/// <summary>
/// Event used when the grace period order is confirmed
/// </summary>
public class OrderStatusChangedToAwaitingValidationDomainEvent
        : INotification
{
    public int OrderId { get; }
    public IEnumerable<OrderItem> OrderItems { get; }

    public OrderStatusChangedToAwaitingValidationDomainEvent(int orderId,
        IEnumerable<OrderItem> orderItems)
    {
        OrderId = orderId;
        OrderItems = orderItems;
    }
}
