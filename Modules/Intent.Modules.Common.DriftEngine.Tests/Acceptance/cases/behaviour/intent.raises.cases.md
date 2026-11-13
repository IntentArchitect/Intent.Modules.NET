# intent.raises

A Domain Event Origin (Intent.Modelers.Domain.Events) from a class's constructor or operation to a Domain Event says that member raises the event. The claim is stated by the association, so its `element` is the association. The stock Domain pack states the claim, and since no stack pack reads `intent.raises`, its rule comes from the same pack as its vocabulary, or from a house pack (packs.md, "Who ships what"). These cases use eShopCustom's house pack, `house.eshop`, holding its Common.Drift domain-events recipe as converted: find the member, `class:{Entity} > method:{Member}` (a constructor by its class's name), and read its body for `new {DomainEvent}` or a call of a helper named after the event, `Add{DomainEvent}(…)`. `intent.raises` is an existence kind, `stated: always`: a member whose body raises nothing states that it doesn't raise the event, and a silent reading is located at the member. The kind's gate is phase 6.

These cases trace ESC-DOM-30 in part. The check also followed private calls of any name, transitively, to the method that raises the event; a recipe reads one member's body and can't follow calls, so that part needs a plug-in rule and is left out here. The helper named after the event is the part a recipe covers.

## verified-direct

`SetShippedStatus` adds `new OrderShippedDomainEvent(this)` in its own body, and the model links it to `OrderShippedDomainEvent` (ESC-DOM-30, a direct raise).

```json case
{ "phase": 6, "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R2", "R46"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "description": "eShopCustom's house pack: its Common.Drift domain-events recipe, as converted.",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "rules": {
    "Domain": {
      "facts": {
        "intent.raises": {
          "files": "src/Domain/**/*.cs",
          "find": "class:{Entity} > method:{Member}",
          "read": [
            { "part": "body", "match": "\\bnew {DomainEvent}\\b", "value": "present" },
            { "part": "body", "match": "\\bAdd{DomainEvent}\\(", "value": "present" }
          ],
          "default": "absent"
        }
      }
    }
  }
}
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

The constructor doesn't build the event itself: it calls the private helper `AddOrderStartedDomainEvent`, named after the event, as eShop's `Order` does. The recipe's second read case finds the call, so the link to `OrderStartedDomainEvent` is verified (ESC-DOM-30, a helper named after the event).

```json case
{ "phase": 6, "base": "verified-direct", "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R2", "R46"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-ctor-raises-started", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Domain/Order.cs:7" }
  ]
}
```

## contradicted-not-raised

`SetShippedStatus` changes the order's state and raises nothing. The member is found and its body raises no `OrderShippedDomainEvent`, so the recipe's `default` applies: the code states, by saying nothing, that the member doesn't raise it, and the modelled link is contradicted (ESC-DOM-30).

```json case
{ "phase": 6, "base": "verified-direct", "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R1", "R3", "R11"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
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
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "contradicted", "expected": "present", "actual": "absent", "fromSilence": true,
      "at": "src/Domain/Order.cs:12" },
    { "kind": "intent.raises", "element": "order-ctor-raises-started", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/intent.raises", "severity": "error", "subject": "order-set-shipped-raises-shipped" }
  ]
}
```

## contradicted-raised-by-another-class

`SetShippedStatus` hands the work to a `Shipment`, and it is `Shipment.Ship` that adds `OrderShippedDomainEvent`. The rule reads the linked member's own body; a call into another class isn't followed, so the event reads as not raised and the link is contradicted. The fix is in the model: link the event to the member that raises it (ESC-DOM-30).

```json case
{ "phase": 6, "base": "verified-direct", "kinds": ["intent.raises"], "traces": ["ESC-DOM-30"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order : Entity
{
    private readonly Shipment _shipment = new();

    public Order(string userId)
    {
        AddOrderStartedDomainEvent(userId);
    }

    public void SetShippedStatus()
    {
        _shipment.Ship(this);
    }

    private void AddOrderStartedDomainEvent(string userId)
    {
        AddDomainEvent(new OrderStartedDomainEvent(this, userId));
    }
}
```

```csharp file src/Domain/Shipment.cs
namespace Ordering.Domain;

public class Shipment : Entity
{
    public void Ship(Order order)
    {
        AddDomainEvent(new OrderShippedDomainEvent(order));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "contradicted", "expected": "present", "actual": "absent", "fromSilence": true,
      "at": "src/Domain/Order.cs:12" }
  ],
  "findings": [
    { "rule": "claim/intent.raises", "severity": "error", "subject": "order-set-shipped-raises-shipped" }
  ]
}
```

## no-rule

Without the house pack, the stock Domain pack still states every Domain Event Origin, but no active pack reads one. Each link is can't verify, `noRule`, one error each, never a silent skip (principle 2). The case doesn't build on `verified-direct`, whose pack block it would inherit.

```json case
{ "phase": 1, "kinds": ["intent.raises"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
    { "kind": "intent.raises", "element": "order-ctor-raises-started", "outcome": "cantVerify", "reason": "noRule" },
    { "kind": "intent.raises", "element": "order-set-shipped-raises-shipped", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/intent.raises", "severity": "error", "subject": "order-ctor-raises-started", "messageContains": "no rule" },
    { "rule": "claim/intent.raises", "severity": "error", "subject": "order-set-shipped-raises-shipped", "messageContains": "no rule" }
  ]
}
```
