# engine/ambiguous

Where a claim's code must be one place, several matches are can't verify, `ambiguous` (outcome step 7), and the finding names every candidate with its file and line. The engine never picks one: comparing whichever it found first could verify against the wrong code. An element's type is found by name among the in-scope types of the kinds its vocabulary allows (R43), so two in-scope types of one name, both of an allowed kind, make it ambiguous. The parts of a partial type are one type (R37), never two candidates.

eShopCustom's Domain module compared one of a type's declarations and warned about the rest. Here the claim isn't compared at all, and it is an error, since the model's claim went unchecked (principle 2).

## type-declared-twice

`Order` was moved into an `Orders` folder and the old file was never deleted, so two classes named `Order` are in scope. The type claim is `ambiguous`, naming both. `Id`'s claims depend on finding `Order`, so they are `blocked` behind it, with no finding of their own, rather than reported one by one.

```json case
{ "phase": 1, "cases": ["engine/ambiguous", "engine/blocked"], "traces": ["ESC-DOM-15"], "requirements": ["R5", "R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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

```csharp file src/Domain/Orders/Order.cs
namespace Shop.Domain.Orders;

public class Order
{
    public Guid Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "ambiguous" },
    { "kind": "member.exists", "element": "order-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Domain/Order.cs:3" },
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Domain/Orders/Order.cs:3" },
    { "rule": "claim/member.exists", "subject": "order-id", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## partial-type-is-one-type

`Order` is declared in two files, as the two parts of one partial class. That is one type, not two candidates: the type claim is verified, and the operation in the second part is found in it.

```json case
{ "phase": 1, "cases": ["engine/ambiguous"], "requirements": ["R37", "R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-submitted-on", "type": "Attribute", "name": "SubmittedOn", "typeRef": { "type": "datetime", "nullable": true } },
              { "id": "order-submit", "type": "Operation", "name": "Submit" }
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

public partial class Order
{
    public Guid Id { get; set; }

    public DateTime? SubmittedOn { get; private set; }
}
```

```csharp file src/Domain/Order.Behaviour.cs
namespace Shop.Domain;

public partial class Order
{
    public void Submit()
    {
        SubmittedOn ??= DateTime.UtcNow;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified" },
    { "kind": "member.exists", "element": "order-submit", "outcome": "verified", "at": "src/Domain/Order.Behaviour.cs:5" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "subject": "order", "count": 0 }
  ]
}
```
