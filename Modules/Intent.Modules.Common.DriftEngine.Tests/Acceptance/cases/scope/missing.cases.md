# scope/missing

With no `Drift Scope` row anywhere, no code is in scope (R21). Nothing is read, every claim is can't verify, and one error says so with the number of claims, instead of a finding per claim. eShopCustom's Architecture module only warned here; the engine makes it an error, since nothing modelled can be checked (principle 2).

## no-drift-scope

The Codebase Structure designer has a Root Folder with no `Drift Scope`. The type's claim is `outOfScope`; its member's is blocked behind it.

```json case
{ "phase": 1, "cases": ["scope/missing"], "traces": ["ESC-DOM-01", "ESC-SVC-01", "ESC-FE-01", "ESC-ARC-05"], "requirements": ["R21", "R22"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder" }]
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

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "outOfScope" },
    { "kind": "member.exists", "element": "order-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "scope/missing", "severity": "error", "subject": "root", "messageContains": "No Drift Scope" }
  ],
  "noOtherFindings": true,
  "filesRead": { "exclude": ["**"] }
}
```

## empty-entries

A `Drift Scope` with no rows is no scope at all.

```json case
{ "phase": 1, "base": "no-drift-scope", "cases": ["scope/missing"], "traces": ["ESC-DOM-01"], "requirements": ["R21"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
        "stereotypes": { "Drift Scope": { "Entries": [] } } }]
    }
  }
}
```
