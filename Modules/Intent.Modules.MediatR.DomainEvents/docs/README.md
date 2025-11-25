# Intent.MediatR.DomainEvents

## What are Domain Events?
Domain Events capture significant state changes within the domain model (inside the bounded context) allowing decoupled reactions. This module generates MediatR handlers and supporting infrastructure for modeled domain events.

## What This Module Generates
- Aggregate Manager handlers coordinating event dispatch per aggregate root.
- Default domain event handler scaffolds (implicit handlers when none explicitly modeled if setting enabled).
- Explicit domain event handler templates for modeled events.
- `DomainEventNotification` wrapper integrating event into MediatR pipeline.
- Domain Event Service infrastructure to raise and publish events.

## Module Settings
### Create implicit domain event handlers (switch)
When true (default) generates a default handler for each domain event without an explicit handler model, ensuring events are not silently ignored.

## Examples
```csharp
// Raising a domain event inside an entity method
public void AddItem(OrderItem item)
{
    _items.Add(item);
    this.AddDomainEvent(new OrderItemAddedDomainEvent(Id, item.Id)); // intent pattern
}
```
```csharp
// Explicit handler (generated template skeleton)
public class OrderItemAddedDomainEventHandler : IDomainEventHandler<OrderItemAddedDomainEvent>
{
    public Task Handle(OrderItemAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Side effects within same transaction boundary
        return Task.CompletedTask;
    }
}
```

## When to Use
Adopt domain events to decouple aggregate behavior and cross-cutting reactions (auditing, policy enforcement) within the same bounded context. Use integration eventing (MassTransit, Dapr Pub/Sub) for cross-service communication; domain events stay internal.

## Integration with Other Modules
- Requires `Intent.Application.DependencyInjection.MediatR` for MediatR wiring.
- Complements `Intent.Application.DomainInteractions` enabling richer interaction modeling.
- Pairs with persistence modules; domain events typically dispatched post SaveChanges.
- Can bridge into integration events via explicit translators in service/eventing modules.

## Intent-Specific Patterns
- Event raising via entity method `AddDomainEvent()` captured by generated Aggregate Manager.
- Implicit handler feature prevents missed events while modeling is incomplete.

## External Resources
- Domain Events (Martin Fowler): https://martinfowler.com/eaaDev/DomainEvent.html
- MediatR Library: https://github.com/jbogard/MediatR

## Related Modules
- Intent.MediatR (CQRS handlers)
- Intent.Eventing.* (integration events)
- Persistence modules (EntityFrameworkCore, MongoDb, etc.)
