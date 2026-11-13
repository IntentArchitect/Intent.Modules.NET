# member.wireName

A DTO-Field's serialized name, where the model sets one: the Serialization Settings stereotype with a Custom Name. The code side is the property's `[JsonPropertyName]`. It is a `stated: whenSet` kind: a field with no custom name gives no claim, and a custom name the code doesn't state is contradicted as not stated, even where the serializer's camel-case convention would produce the same name (decision 4, R4). The kind's phase is `later`, so these cases trace the check rather than gate a phase.

## verified

`OrderSummary.BuyerName` is serialized as `buyer`, as its Serialization Settings say, and the property's `[JsonPropertyName("buyer")]` states it. `OrderNumber` has no custom name: no claim.

```json case
{ "phase": "later", "kinds": ["member.wireName"], "traces": ["ESC-SVC-48"], "requirements": ["R2", "R30", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } },
              { "id": "order-summary-buyer-name", "type": "DTO-Field", "name": "BuyerName", "typeRef": { "type": "string" },
                "stereotypes": { "Serialization Settings": { "Naming Convention": "Custom", "Custom Name": "buyer" } } }
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

```csharp file src/Ordering.API/Application/Queries/OrderSummary.cs
using System.Text.Json.Serialization;

namespace eShop.Ordering.API.Application.Queries;

public record OrderSummary
{
    public int OrderNumber { get; init; }

    [JsonPropertyName("buyer")] public string BuyerName { get; init; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "member.wireName", "element": "order-summary-buyer-name", "outcome": "verified", "expected": "buyer", "actual": "buyer", "at": "src/Ordering.API/Application/Queries/OrderSummary.cs:9" },
    { "kind": "member.wireName", "element": "order-summary-order-number", "absent": true }
  ]
}
```

## contradicted

The property is serialized as `buyerName`: contradicted, `*.field.wire-name` in eShopCustom's terms.

```json case
{ "phase": "later", "base": "verified", "kinds": ["member.wireName"], "traces": ["ESC-SVC-48"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Queries/OrderSummary.cs
using System.Text.Json.Serialization;

namespace eShop.Ordering.API.Application.Queries;

public record OrderSummary
{
    public int OrderNumber { get; init; }

    [JsonPropertyName("buyerName")] public string BuyerName { get; init; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "member.wireName", "element": "order-summary-buyer-name", "outcome": "contradicted", "expected": "buyer", "actual": "buyerName", "at": "src/Ordering.API/Application/Queries/OrderSummary.cs:9" }
  ],
  "findings": [
    { "rule": "claim/member.wireName", "severity": "error", "subject": "order-summary-buyer-name" }
  ]
}
```

## not-stated

The model's custom name is `buyerName`, and the property has no `[JsonPropertyName]`. System.Text.Json's camel-case policy would serialize `BuyerName` as `buyerName`, but the code doesn't state it, so the claim is contradicted as not stated rather than verified from the convention (decision 4).

```json case
{ "phase": "later", "base": "verified", "kinds": ["member.wireName"], "traces": ["ESC-SVC-48"], "requirements": ["R4"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } },
              { "id": "order-summary-buyer-name", "type": "DTO-Field", "name": "BuyerName", "typeRef": { "type": "string" },
                "stereotypes": { "Serialization Settings": { "Naming Convention": "Custom", "Custom Name": "buyerName" } } }
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

```csharp file src/Ordering.API/Application/Queries/OrderSummary.cs
namespace eShop.Ordering.API.Application.Queries;

public record OrderSummary
{
    public int OrderNumber { get; init; }

    public string BuyerName { get; init; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "member.wireName", "element": "order-summary-buyer-name", "outcome": "contradicted", "expected": "buyerName" }
  ],
  "findings": [
    { "rule": "claim/member.wireName", "severity": "error", "subject": "order-summary-buyer-name", "messageContains": "doesn't state" }
  ]
}
```
