# Intent.DomainEvents

This module generates C# Domain Events from modeled domain events in the Domain Designer, along with supporting base types and interfaces. Domain events are a key pattern in Domain-Driven Design (DDD) for implementing Rich Domain models.

## What are Domain Events?

Domain Events are a DDD pattern for capturing state changes or significant occurrences within your domain model. They are raised by domain entities (aggregates) when important business actions occur, allowing the domain to coordinate complex behaviors while maintaining encapsulation and separation of concerns.

Domain Events are:
- **Internal to your application** - Handled within the same process/bounded context
- **Part of Rich Domain models** - Entities raise events to notify about state changes
- **Coordination mechanism** - Allow decoupling between aggregates and domain logic

## What This Module Generates

The `Intent.DomainEvents` module generates:

- **Domain Event Classes** - C# classes representing domain events modeled in the Domain Designer
- **Base Types** - `DomainEvent` base class with metadata (timestamp, event ID)
- **HasDomainEvent Interface** - `IHasDomainEvent` for entities that can raise domain events

## Generated Code Example

### Domain Event Class

```csharp
public class OrderPlacedEvent : DomainEvent
{
    public OrderPlacedEvent(Guid orderId, decimal totalAmount)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
    }

    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
}
```

### Entity with Domain Events

```csharp
public class Order : IHasDomainEvent
{
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void PlaceOrder()
    {
        // Business logic
        Status = OrderStatus.Placed;
        
        // Raise domain event
        _domainEvents.Add(new OrderPlacedEvent(Id, TotalAmount));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

## Rich Domain and DDD

Domain Events are a fundamental pattern in Domain-Driven Design (DDD) for implementing Rich Domain models. In Rich Domain:

- **Entities encapsulate behavior** - Business logic lives in domain entities, not services
- **Events capture state changes** - When important actions occur, entities raise domain events
- **Decoupled coordination** - Aggregates don't directly reference each other; events coordinate behavior
- **Transaction boundaries** - Events raised during a transaction are dispatched when SaveChanges occurs

## How Domain Events are Dispatched

Domain events are dispatched through the **Entity Framework SaveChanges hook** provided by the `Intent.EntityFrameworkCore.Interop.DomainEvents` module:

1. Entity raises domain event (added to `DomainEvents` collection)
2. Application calls `SaveChangesAsync()` on DbContext
3. Before persisting, EF intercepts and dispatches all pending domain events
4. Event handlers execute (via MediatR when using `Intent.MediatR.DomainEvents`)
5. If all handlers succeed, changes are committed to database
6. If any handler fails, transaction rolls back

## When to Use Domain Events

Use domain events when implementing Rich Domain models (DDD) and you need to:
- **Coordinate side effects** - One aggregate action triggers behavior in another
- **Decouple aggregates** - Avoid direct dependencies between domain entities
- **Maintain transaction consistency** - Keep related changes in same database transaction
- **Implement complex domain workflows** - Multiple handlers respond to single domain action

## Integration with Other Modules

### Intent.EntityFrameworkCore.Interop.DomainEvents
Provides the critical hook into Entity Framework's SaveChanges to automatically dispatch domain events during the transaction.

### Intent.MediatR.DomainEvents
Provides the infrastructure for dispatching domain events using MediatR, allowing multiple handlers to respond to events.

### Intent.Entities
Domain entities implement `IHasDomainEvent` to raise events as part of their business logic.
