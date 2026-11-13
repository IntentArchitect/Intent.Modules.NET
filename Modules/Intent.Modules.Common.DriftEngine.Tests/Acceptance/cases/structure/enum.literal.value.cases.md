# enum.literal.value

A modelled enum literal's value, where the model sets one, against the enum member's written value. The comparer is `number`: both sides parse as the same number. It is a `stated: whenSet` kind: a literal with no modelled value gives no claim, and a modelled value the code doesn't write is contradicted as not stated, even when C#'s implicit numbering would give the same number (decision 4, R4). It depends on the enum: when the enum isn't there, the claim is blocked.

## verified

`Submitted` and `Paid` have modelled values, and the code writes the same ones. `StockConfirmed` has no modelled value, so the code's `= 2` gives no claim.

```json case
{ "phase": 1, "kinds": ["enum.literal.value"], "traces": ["ESC-DOM-25"], "requirements": ["R2", "R10", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-submitted", "type": "Enum-Literal", "name": "Submitted", "value": "1" },
              { "id": "order-status-stock-confirmed", "type": "Enum-Literal", "name": "StockConfirmed" },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid", "value": "3" }
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

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public enum OrderStatus
{
    Submitted = 1,
    StockConfirmed = 2,
    Paid = 3
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literal.value", "element": "order-status-submitted", "outcome": "verified", "expected": "1", "actual": "1", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs:5" },
    { "kind": "enum.literal.value", "element": "order-status-paid", "outcome": "verified", "expected": "3", "actual": "3", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs:7" },
    { "kind": "enum.literal.value", "element": "order-status-stock-confirmed", "absent": true }
  ],
  "findings": [
    { "rule": "claim/enum.literal.value", "count": 0 }
  ]
}
```

## contradicted

`Paid` is written as `4`: contradicted, `enum.value` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "verified", "kinds": ["enum.literal.value"], "traces": ["ESC-DOM-25"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public enum OrderStatus
{
    Submitted = 1,
    StockConfirmed = 2,
    Paid = 4
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literal.value", "element": "order-status-paid", "outcome": "contradicted", "expected": "3", "actual": "4", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs:7" }
  ],
  "findings": [
    { "rule": "claim/enum.literal.value", "severity": "error", "subject": "order-status-paid", "messageContains": "code `4`" }
  ]
}
```

## not-stated

`Paid` has no written value. C# numbers it `3`, after `StockConfirmed = 2`, which is the model's value, but the code doesn't state it: contradicted with the code side not stated, never verified from the language's numbering (decision 4, R4).

```json case
{ "phase": 1, "base": "verified", "kinds": ["enum.literal.value"], "traces": ["ESC-DOM-25"], "requirements": ["R4", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public enum OrderStatus
{
    Submitted = 1,
    StockConfirmed = 2,
    Paid
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literal.value", "element": "order-status-paid", "outcome": "contradicted", "expected": "3", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/enum.literal.value", "severity": "error", "subject": "order-status-paid", "messageContains": "doesn't state" }
  ]
}
```

## blocked

The enum's file is gone: `OrderStatus`'s `type.exists` claim is contradicted, and each modelled value's claim waits on it, can't verify, `blocked`.

```json case
{ "phase": 1, "base": "verified", "without": ["src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs"], "kinds": ["enum.literal.value", "type.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-status", "outcome": "contradicted" },
    { "kind": "enum.literal.value", "element": "order-status-submitted", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" },
    { "kind": "enum.literal.value", "element": "order-status-paid", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order-status" },
    { "rule": "claim/enum.literal.value", "count": 0 }
  ]
}
```
