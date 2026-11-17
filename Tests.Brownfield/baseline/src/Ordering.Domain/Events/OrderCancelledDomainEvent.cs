using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Domain.Guardrails.Contracts.DomainEventContract", Version = "1.0")]

namespace eShop.Ordering.Domain.Events;

public class OrderCancelledDomainEvent : INotification
{
    public Order Order { get; }

    public OrderCancelledDomainEvent(Order order)
    {
        Order = order;
    }
}

