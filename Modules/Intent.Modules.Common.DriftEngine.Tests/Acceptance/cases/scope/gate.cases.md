# scope/gate

Opt-in "must be modelled" rules report in-scope code only (R49). Scope is what the team has agreed to describe; code outside it is out of the conversation, so no rule may report it, however the rule is narrowed. Out-of-scope files are not even read (R22).

eShopCustom's Services module fired every code-to-model rule only for adopted files. With scope on the Codebase Structure designer, that guard is the engine's, for every opt-in rule.

## opt-in-rule-reports-in-scope-code-only

`unmodelled.file` is on. `OrderSummaryDto.cs` is in scope and nothing in the model locates it, so it is reported. `LegacyOrderDto.cs` is excluded by row 2 and `SeedOrders.cs` is in no row: both are just as unmodelled, and neither is reported or read.

```json case
{ "phase": 1, "cases": ["scope/gate", "optin/unmodelled.file"], "traces": ["ESC-SVC-06"], "requirements": ["R22", "R49"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/Application/Legacy/**", "Scope": "Out", "Reason": "v1 API contracts, kept until v1 is retired" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ] }
        } }]
    }
  }
}
```

```csharp file src/Application/Orders/OrderDto.cs
namespace Shop.Application.Orders;

public record OrderDto(Guid Id);
```

```csharp file src/Application/Orders/OrderSummaryDto.cs
namespace Shop.Application.Orders;

public record OrderSummaryDto(Guid Id, decimal Total);
```

```csharp file src/Application/Legacy/LegacyOrderDto.cs
namespace Shop.Application.Legacy;

public record LegacyOrderDto(int OrderNumber);
```

```csharp file tools/SeedOrders.cs
namespace Shop.Tools;

public static class SeedOrders
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-dto", "outcome": "verified", "at": "src/Application/Orders/OrderDto.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "messageContains": "OrderSummaryDto.cs" },
    { "rule": "optin/unmodelled.file", "messageContains": "LegacyOrderDto.cs", "count": 0 },
    { "rule": "optin/unmodelled.file", "messageContains": "SeedOrders.cs", "count": 0 }
  ],
  "filesRead": { "include": ["src/Application/Orders/*.cs"], "exclude": ["src/Application/Legacy/**", "tools/**"] }
}
```

## opt-in-glob-cannot-widen-scope

The opt-in row is narrowed by a glob to the legacy folder, the only place it would find anything unmodelled. A rule's glob narrows within scope and never widens it, so the rule reports nothing at all, and the legacy file is still not read.

```json case
{ "phase": 1, "base": "opt-in-rule-reports-in-scope-code-only", "cases": ["scope/gate", "optin/unmodelled.file"], "traces": ["ESC-SVC-06"], "requirements": ["R22", "R49"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/Application/Legacy/**", "Scope": "Out", "Reason": "v1 API contracts, kept until v1 is retired" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file", "Glob": "src/Application/Legacy/**" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.file", "count": 0 }
  ],
  "filesRead": { "exclude": ["src/Application/Legacy/**"] }
}
```

## not-line-takes-back-scope

One In row does what the first case's two rows do, as a File Classification row would: a `#` line is a comment, and its `!` line takes the legacy folder back out of `src/**`, the last matching line deciding. `LegacyOrderDto.cs` is then in no row, so it is neither read nor reported.

```json case
{ "phase": 1, "base": "opt-in-rule-reports-in-scope-code-only", "cases": ["scope/gate", "optin/unmodelled.file"], "requirements": ["R22", "R27", "R49"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "# Everything under src but the v1 contracts\nsrc/**\n!src/Application/Legacy/**", "Scope": "In" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "messageContains": "OrderSummaryDto.cs" },
    { "rule": "optin/unmodelled.file", "messageContains": "LegacyOrderDto.cs", "count": 0 },
    { "rule": "optin/unmodelled.file", "messageContains": "SeedOrders.cs", "count": 0 }
  ],
  "filesRead": { "include": ["src/Application/Orders/*.cs"], "exclude": ["src/Application/Legacy/**", "tools/**"] }
}
```
