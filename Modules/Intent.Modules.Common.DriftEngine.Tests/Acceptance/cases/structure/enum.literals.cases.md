# enum.literals

A modelled enum's literals, in model order, against the enum's members. The comparer is `subsequence`: the model's literals must appear in the code in the model's order, and the code may have more (claims-and-findings.md, Comparers). A literal the code has and the model doesn't is never a contradiction of this claim; reporting it is the `unmodelled.member` opt-in rule's job (R44). It is a `stated: always` kind that depends on the enum: when the enum isn't there, the claim is blocked. The cases don't spell out `expected` and `actual`, since a list's canonical text isn't settled.

## verified

`OrderStatus` has the model's six literals in the model's order. The values the code gives them aren't modelled here, so they give no claims.

```json case
{ "phase": 1, "kinds": ["enum.literals"], "traces": ["ESC-DOM-24", "ESC-DOM-26"], "requirements": ["R2", "R10", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-status-submitted", "type": "Enum-Literal", "name": "Submitted" },
              { "id": "order-status-awaiting-validation", "type": "Enum-Literal", "name": "AwaitingValidation" },
              { "id": "order-status-stock-confirmed", "type": "Enum-Literal", "name": "StockConfirmed" },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid" },
              { "id": "order-status-shipped", "type": "Enum-Literal", "name": "Shipped" },
              { "id": "order-status-cancelled", "type": "Enum-Literal", "name": "Cancelled" }
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
    AwaitingValidation = 2,
    StockConfirmed = 3,
    Paid = 4,
    Shipped = 5,
    Cancelled = 6
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "order-status", "outcome": "verified" },
    { "kind": "enum.literal.value", "element": "order-status-paid", "absent": true }
  ],
  "findings": [
    { "rule": "claim/enum.literals", "count": 0 }
  ]
}
```

## verified-extra-code-literal

The code adds a `Draft` literal the model doesn't have. The model's six still appear in its order, so the `subsequence` comparer verifies the claim. `Draft` is no finding while the `unmodelled.member` rule is off (principle 1).

```json case
{ "phase": 1, "base": "verified", "kinds": ["enum.literals"], "cases": ["engine/comparers"], "traces": ["ESC-DOM-24"], "requirements": ["R2", "R10", "R44"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public enum OrderStatus
{
    Draft = 0,
    Submitted = 1,
    AwaitingValidation = 2,
    StockConfirmed = 3,
    Paid = 4,
    Shipped = 5,
    Cancelled = 6
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "order-status", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/enum.literals", "count": 0 },
    { "rule": "optin/unmodelled.member", "count": 0, "note": "Draft isn't modelled, and the rule is off" }
  ]
}
```

## contradicted-missing

The code has no `StockConfirmed`: the model's literals don't all appear, so the claim is contradicted, `enum.literal` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "verified", "kinds": ["enum.literals"], "traces": ["ESC-DOM-24"], "requirements": ["R1", "R10", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public enum OrderStatus
{
    Submitted = 1,
    AwaitingValidation = 2,
    Paid = 4,
    Shipped = 5,
    Cancelled = 6
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "order-status", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/enum.literals", "severity": "error", "subject": "order-status", "messageContains": "StockConfirmed" }
  ]
}
```

## contradicted-order

Every literal is there, but `Paid` comes before `StockConfirmed`. The model's order is part of the claim: contradicted, `enum.order` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "verified", "kinds": ["enum.literals"], "traces": ["ESC-DOM-26"], "requirements": ["R1", "R10", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public enum OrderStatus
{
    Submitted = 1,
    AwaitingValidation = 2,
    Paid = 4,
    StockConfirmed = 3,
    Shipped = 5,
    Cancelled = 6
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "order-status", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/enum.literals", "severity": "error", "subject": "order-status" }
  ]
}
```

## blocked

The enum's file is gone, so `OrderStatus`'s `type.exists` claim is contradicted, and its literals claim waits on it: can't verify, `blocked`. The one finding is the missing enum's (R5).

```json case
{ "phase": 1, "base": "verified", "without": ["src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderStatus.cs"], "kinds": ["enum.literals", "type.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-status", "outcome": "contradicted" },
    { "kind": "enum.literals", "element": "order-status", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order-status" },
    { "rule": "claim/enum.literals", "count": 0 }
  ]
}
```

## services-enum

A Services designer Enum against its enum: eShop's `WebhookType`.

```json case
{ "phase": 1, "kinds": ["enum.literals"], "traces": ["ESC-SVC-49"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Webhooks.API",
        "elements": [
          { "id": "webhook-type", "type": "Enum", "name": "WebhookType",
            "children": [
              { "id": "webhook-type-catalog-item-price-change", "type": "Enum-Literal", "name": "CatalogItemPriceChange" },
              { "id": "webhook-type-order-shipped", "type": "Enum-Literal", "name": "OrderShipped" },
              { "id": "webhook-type-order-paid", "type": "Enum-Literal", "name": "OrderPaid" }
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

```csharp file src/Webhooks.API/Model/WebhookType.cs
namespace Webhooks.API.Model;

public enum WebhookType
{
    CatalogItemPriceChange = 1,
    OrderShipped = 2,
    OrderPaid = 3
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "webhook-type", "outcome": "verified" }
  ]
}
```

## services-enum-missing-literal

The code has no `OrderPaid`: contradicted, `services.enum.literal.missing` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "services-enum", "kinds": ["enum.literals"], "traces": ["ESC-SVC-49"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Webhooks.API/Model/WebhookType.cs
namespace Webhooks.API.Model;

public enum WebhookType
{
    CatalogItemPriceChange = 1,
    OrderShipped = 2
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "webhook-type", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/enum.literals", "severity": "error", "subject": "webhook-type", "messageContains": "OrderPaid" }
  ]
}
```

## java

The same claim over a Java enum, in Java's order. The code's extra `Refunded`, at the end, doesn't disturb the subsequence.

```json case
{ "phase": 4, "kinds": ["enum.literals"], "traces": ["JAV-DOM-28"], "requirements": ["R2", "R10", "R33", "R41"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-placed", "type": "Enum-Literal", "name": "Placed" },
              { "id": "order-status-shipped", "type": "Enum-Literal", "name": "Shipped" },
              { "id": "order-status-delivered", "type": "Enum-Literal", "name": "Delivered" },
              { "id": "order-status-cancelled", "type": "Enum-Literal", "name": "Cancelled" },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/OrderStatus.java
package com.shop.domain;

public enum OrderStatus {
    Placed, Shipped, Delivered, Cancelled, Paid, Refunded
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "order-status", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 0, "note": "Refunded isn't modelled, and the rule is off" }
  ]
}
```

## java-order

`Paid` now comes before `Delivered` in the Java enum, out of the model's order: contradicted.

```json case
{ "phase": 4, "base": "java", "kinds": ["enum.literals"], "traces": ["JAV-DOM-28"], "requirements": ["R1", "R10", "R11"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```java file src/main/java/com/shop/domain/OrderStatus.java
package com.shop.domain;

public enum OrderStatus {
    Placed, Shipped, Paid, Delivered, Cancelled, Refunded
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "order-status", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/enum.literals", "severity": "error", "subject": "order-status" }
  ]
}
```
