# member.static

A modelled operation is static exactly when the model's `static` says so; the code side is the method's `static` modifier. It is a `stated: always` kind: a method with no `static` modifier states that it isn't static, so a non-static operation is verified from silence (R3). It depends on the member: when the method isn't there, the claim is blocked.

## verified

`Order.NewDraft` is static in the model and has the `static` modifier. `ConfirmDraft` isn't static in the model, and its method has no `static` modifier: the code says so by saying nothing, so that claim is verified from silence.

```json case
{ "phase": 1, "kinds": ["member.static"], "cases": ["engine/silence"], "traces": ["ESC-DOM-64"], "requirements": ["R2", "R3", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-is-draft", "type": "Attribute", "name": "IsDraft", "typeRef": { "type": "bool" } },
              { "id": "order-new-draft", "type": "Operation", "name": "NewDraft", "static": true, "typeRef": { "element": "order" } },
              { "id": "order-confirm-draft", "type": "Operation", "name": "ConfirmDraft" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public bool IsDraft { get; private set; }

    public static Order NewDraft()
    {
        return new Order { IsDraft = true };
    }

    public void ConfirmDraft()
    {
        IsDraft = false;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.static", "element": "order-new-draft", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" },
    { "kind": "member.static", "element": "order-confirm-draft", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true, "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:12" }
  ],
  "findings": [
    { "rule": "claim/member.static", "count": 0 }
  ]
}
```

## contradicted

`NewDraft` has lost its `static` modifier. The method's silence states that it isn't static, against the model's static operation: contradicted, from silence.

```json case
{ "phase": 1, "base": "verified", "kinds": ["member.static"], "cases": ["engine/silence"], "traces": ["ESC-DOM-64"], "requirements": ["R1", "R3", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public bool IsDraft { get; private set; }

    public Order NewDraft()
    {
        return new Order { IsDraft = true };
    }

    public void ConfirmDraft()
    {
        IsDraft = false;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.static", "element": "order-new-draft", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true, "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.static", "severity": "error", "subject": "order-new-draft" }
  ]
}
```

## blocked

`Order` has no `NewDraft` at all. The operation's `member.exists` claim is contradicted, and its `member.static` claim is blocked by it rather than verified or contradicted from a method that isn't there.

```json case
{ "phase": 1, "base": "verified", "kinds": ["member.static", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public bool IsDraft { get; private set; }

    public void ConfirmDraft()
    {
        IsDraft = false;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-new-draft", "outcome": "contradicted" },
    { "kind": "member.static", "element": "order-new-draft", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-new-draft" },
    { "rule": "claim/member.static", "count": 0 }
  ]
}
```
