# engine/report

Findings go through Intent.Common.DriftReporting exactly as Common.Drift's do, in the engine's own Areas so both engines can run on one application (R12). A claim's finding is in the Area `<Designer> (DriftEngine)`, category `ModelCode`; its subject is an `Element` anchor to the innermost model element, so the report links straight to it; and what it was compared with is a `File` anchor (path and line), for a member the code lacks its owner's declaration (R11). Its message names the claim's label and the element, never an id, and says what disagrees and which model property states it, with code in backticks (R14).

The suite's `expect` block asserts a finding's rule, severity, subject and message, not its Area, category or anchors; those are stated here for the hub adapter's own tests.

## contradiction-names-what-disagrees

The model types `Order.Total` as `decimal`; the code declares a `double`. The one finding is in the `Domain (DriftEngine)` Area, its subject an `Element` anchor to `Total` and its `File` anchor `src/Domain/Order.cs:7`. Its message names the claim's label (type), the element and both values; the line is the finding's file link, so the message doesn't repeat it.

```json case
{ "phase": 1, "kinds": ["member.type"], "cases": ["engine/report"], "requirements": ["R11", "R12", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "order-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }

    public double Total { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-total", "outcome": "contradicted", "at": "src/Domain/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "order-total", "messageContains": "`decimal`" },
    { "rule": "claim/member.type", "severity": "error", "subject": "order-total", "messageContains": "`double`" },
    { "rule": "claim/member.type", "severity": "error", "subject": "order-total", "messageContains": "`Order.Total` type: model `decimal`, code `double`." },
    { "rule": "claim/member.type", "subject": "order-total", "messageContains": "Order.cs", "count": 0 }
  ]
}
```

## missing-is-one-plain-finding

The model has a `Customer` class the code doesn't have. The finding says only that: how the engine searched, and how many claims wait on the type, are no help to whoever reads the report. `Customer`'s other claims are `blocked` behind the missing type and have no finding of their own.

```json case
{ "phase": 1, "base": "contradiction-names-what-disagrees", "kinds": ["type.exists"], "cases": ["engine/report", "engine/blocked"], "requirements": ["R5", "R8", "R11", "R12"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
            ] },
          { "id": "customer", "type": "Class", "name": "Customer",
            "children": [
              { "id": "customer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "customer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
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
    { "kind": "type.exists", "element": "customer", "outcome": "contradicted", "expected": "present", "actual": "absent" },
    { "kind": "member.exists", "element": "customer-name", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "customer", "messageContains": "`Customer` isn't in the code." },
    { "rule": "claim/type.exists", "subject": "customer", "messageContains": "Searched", "count": 0 },
    { "rule": "claim/type.exists", "subject": "customer", "messageContains": "can't be checked", "count": 0 },
    { "rule": "claim/member.exists", "subject": "customer-name", "count": 0 }
  ]
}
```
