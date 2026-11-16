# intent.raises

A Domain Event Origin (Intent.Modelers.Domain.Events) from a class's constructor or operation to a Domain Event says that member raises the event. The claim is stated by the association, so its `element` is the association. The stock messaging extension, `intent.messaging`, states and reads it whatever raises the event (user decision, 2026-11-14): the member, or a private method of its class it calls, transitively, hands a new event to a call. A member whose code raises nothing contradicts the link, and the reading is located at the member.

The cases trace ESC-DOM-30. The stock reading follows private calls of any name, as the check did, so a helper named after the event is read like any other.

A link says the member can raise the event, not that it always does (user decision, 2026-11-16). A raise in any branch bears it out, and so does a raise of what a method of the code returns, when one of the events it can return is this one.

## verified-direct

`SetShippedStatus` adds `new OrderShippedDomainEvent(this)` in its own body, and the model links it to `OrderShippedDomainEvent` (ESC-DOM-30, a direct raise).

```json case
{ "phase": 6, "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.messaging"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-ctor", "type": "Class Constructor", "name": "Order",
                "children": [
                  { "id": "order-ctor-user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }
                ] },
              { "id": "order-set-shipped", "type": "Operation", "name": "SetShippedStatus" }
            ] },
          { "id": "order-started", "type": "Domain Event", "name": "OrderStartedDomainEvent" },
          { "id": "order-shipped", "type": "Domain Event", "name": "OrderShippedDomainEvent" }
        ]
      }],
      "associations": [
        { "id": "order-ctor-raises-started", "type": "Domain Event Origin Association",
          "source": { "element": "order-ctor", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-started", "multiplicity": "1", "navigable": true } },
        { "id": "order-set-shipped-raises-shipped", "type": "Domain Event Origin Association",
          "source": { "element": "order-set-shipped", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-shipped", "multiplicity": "1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order : Entity
{
    public Order(string userId)
    {
        AddOrderStartedDomainEvent(userId);
    }

    public void SetShippedStatus()
    {
        AddDomainEvent(new OrderShippedDomainEvent(this));
    }

    private void AddOrderStartedDomainEvent(string userId)
    {
        AddDomainEvent(new OrderStartedDomainEvent(this, userId));
    }
}
```

```csharp file src/Domain/Events/OrderStartedDomainEvent.cs
namespace Ordering.Domain.Events;

public record OrderStartedDomainEvent(Order Order, string UserId) : INotification;
```

```csharp file src/Domain/Events/OrderShippedDomainEvent.cs
namespace Ordering.Domain.Events;

public record OrderShippedDomainEvent(Order Order) : INotification;
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Domain/Order.cs:12" }
  ]
}
```

## verified-helper-named-after-event

The constructor doesn't build the event itself: it calls the private helper `AddOrderStartedDomainEvent`, named after the event, as eShop's `Order` does. The stock reading follows the call into the helper, which adds the event, so the link to `OrderStartedDomainEvent` is verified, located at the constructor's call (ESC-DOM-30, a helper named after the event).

```json case
{ "phase": 6, "base": "verified-direct", "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.messaging"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-ctor-raises-started", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Domain/Order.cs:7" }
  ]
}
```

## verified-raised-conditionally

`SetShippedStatus` raises `OrderShippedDomainEvent` only when the order is paid. The code to raise it is there, so the link is verified, located at the raise (ESC-DOM-30).

```json case
{ "phase": 6, "base": "verified-direct", "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.messaging"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order : Entity
{
    public Order(string userId)
    {
        AddOrderStartedDomainEvent(userId);
    }

    public bool IsPaid { get; private set; }

    public void SetShippedStatus()
    {
        if (IsPaid)
        {
            AddDomainEvent(new OrderShippedDomainEvent(this));
        }
    }

    private void AddOrderStartedDomainEvent(string userId)
    {
        AddDomainEvent(new OrderStartedDomainEvent(this, userId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Domain/Order.cs:16" }
  ]
}
```

## verified-returned-by-helper

`SetShippedStatus` raises whatever `StatusChangedEvent()` returns, and that method returns `OrderShippedDomainEvent` in one arm of its switch. The code can raise the event, so the link is verified, located at the raise (ESC-DOM-30).

```json case
{ "phase": 6, "base": "verified-direct", "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.messaging"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order : Entity
{
    private OrderStatus _status;

    public Order(string userId)
    {
        AddOrderStartedDomainEvent(userId);
    }

    public void SetShippedStatus()
    {
        _status = OrderStatus.Shipped;
        AddDomainEvent(StatusChangedEvent());
    }

    private void AddOrderStartedDomainEvent(string userId)
    {
        AddDomainEvent(new OrderStartedDomainEvent(this, userId));
    }

    private INotification StatusChangedEvent() => _status switch
    {
        OrderStatus.Shipped => new OrderShippedDomainEvent(this),
        _ => new OrderStatusChangedDomainEvent(this, _status)
    };
}
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Domain/Order.cs:15" }
  ]
}
```

## contradicted-not-raised

`SetShippedStatus` changes the order's state and raises nothing. The member is found, and neither its body nor a method it calls hands on an `OrderShippedDomainEvent`, so the code reads as not raising it, located at the member, and the modelled link is contradicted (ESC-DOM-30).

```json case
{ "phase": 6, "base": "verified-direct", "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R1", "R3", "R11"], "packs": ["intent.core", "intent.domain", "intent.messaging"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order : Entity
{
    public Order(string userId)
    {
        AddOrderStartedDomainEvent(userId);
    }

    public bool IsShipped { get; private set; }

    public void SetShippedStatus()
    {
        IsShipped = true;
    }

    private void AddOrderStartedDomainEvent(string userId)
    {
        AddDomainEvent(new OrderStartedDomainEvent(this, userId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "contradicted", "expected": "present", "actual": "absent",
      "at": "src/Domain/Order.cs:12" },
    { "kind": "intent.raises", "element": "order-ctor-raises-started", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/intent.raises", "severity": "error", "subject": "order-set-shipped-raises-shipped" }
  ]
}
```

## stock-reading

The stock messaging extension reads both Domain Event Origins from the code, and nothing else is reported. The constructor raises `OrderStartedDomainEvent` through a private method it calls, so the reading is located at that call; `SetShippedStatus` raises `OrderShippedDomainEvent` in its own body.

```json case
{ "phase": 1, "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.messaging"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-ctor", "type": "Class Constructor", "name": "Order",
                "children": [
                  { "id": "order-ctor-user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }
                ] },
              { "id": "order-set-shipped", "type": "Operation", "name": "SetShippedStatus" }
            ] },
          { "id": "order-started", "type": "Domain Event", "name": "OrderStartedDomainEvent" },
          { "id": "order-shipped", "type": "Domain Event", "name": "OrderShippedDomainEvent" }
        ]
      }],
      "associations": [
        { "id": "order-ctor-raises-started", "type": "Domain Event Origin Association",
          "source": { "element": "order-ctor", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-started", "multiplicity": "1", "navigable": true } },
        { "id": "order-set-shipped-raises-shipped", "type": "Domain Event Origin Association",
          "source": { "element": "order-set-shipped", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-shipped", "multiplicity": "1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order : Entity
{
    public Order(string userId)
    {
        AddOrderStartedDomainEvent(userId);
    }

    public void SetShippedStatus()
    {
        AddDomainEvent(new OrderShippedDomainEvent(this));
    }

    private void AddOrderStartedDomainEvent(string userId)
    {
        AddDomainEvent(new OrderStartedDomainEvent(this, userId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-ctor-raises-started", "outcome": "verified", "expected": "present", "actual": "present", "at": "src/Domain/Order.cs:7" },
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "verified", "expected": "present", "actual": "present", "at": "src/Domain/Order.cs:12" }
  ],
  "findings": []
}
```
