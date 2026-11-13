# scope/reason

Every `Out` row says why (R23). Scope can't hide drift: an exclusion the team can't explain is the one a reviewer most needs to see, so a row without a reason is an error on the element that holds it. `In` rows may give a reason, but needn't. This is new with the scope model; no eShopCustom check had it.

## out-row-without-reason

Row 2 excludes `src/Legacy/**` and says nothing about why.

```json case
{ "phase": 1, "cases": ["scope/reason"], "requirements": ["R20", "R23"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out" }
        ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }
}
```

```csharp file src/Legacy/OrderService.cs
namespace Shop.Legacy;

public class OrderService
{
}
```

```json expect
{
  "findings": [
    { "rule": "scope/reason", "severity": "error", "subject": "root", "messageContains": "src/Legacy/**" }
  ]
}
```

## empty-reason

A reason that is empty text is no reason: values are trimmed, and empty text is nothing.

```json case
{ "phase": 1, "base": "out-row-without-reason", "cases": ["scope/reason"], "requirements": ["R23"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "  " }
        ] } } }]
    }
  }
}
```

## in-row-needs-no-reason

The same scope with the `Out` row's reason given, and no reason on the `In` row. Nothing is reported: only `Out` rows need one.

```json case
{ "phase": 1, "base": "out-row-without-reason", "cases": ["scope/reason"], "requirements": ["R23"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" }
        ] } } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "scope/reason", "count": 0 }
  ]
}
```

## folder-row-without-reason

The unexplained row sits on the `src` Folder element rather than the Root Folder, so the error names that folder: the finding's subject is the element that holds the row, which is where the developer adds the reason.

```json case
{ "phase": 1, "base": "out-row-without-reason", "cases": ["scope/reason"], "requirements": ["R20", "R23"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" }
        ] } },
        "elements": [
          { "id": "src", "type": "Folder", "name": "src",
            "stereotypes": { "Drift Scope": { "Entries": [
              { "Glob": "Legacy/**", "Scope": "Out" }
            ] } } }
        ] }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "scope/reason", "severity": "error", "subject": "src", "messageContains": "Legacy/**" },
    { "rule": "scope/reason", "subject": "root", "count": 0 }
  ]
}
```

## types-row-without-reason

A row that excludes types is an `Out` row too, so it needs a reason as well. basislink5 keeps test helpers in production files; excluding them is fine, but the row must say so.

```json case
{ "phase": 1, "base": "out-row-without-reason", "cases": ["scope/reason", "scope/type"], "requirements": ["R23", "R26"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Domain/**", "Types": ["OrderTestData"], "Scope": "Out" }
        ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }
}

internal static class OrderTestData
{
    public static Order Any() => new() { Id = Guid.NewGuid() };
}
```

```json expect
{
  "findings": [
    { "rule": "scope/reason", "severity": "error", "subject": "root", "messageContains": "OrderTestData" }
  ]
}
```
