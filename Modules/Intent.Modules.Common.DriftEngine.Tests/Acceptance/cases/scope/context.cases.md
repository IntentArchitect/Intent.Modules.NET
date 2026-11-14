# scope/context

A `Context` row reads its files without checking them (R20). Some code is needed for what it says about other code, but isn't code the model describes: the composition root that registers handlers, the DbContext that configures entities, a shared base class. Made `In`, an opt-in rule would report it; made `Out`, nothing could read it. Its files are read, so claims verify from them and rules can use them, but no finding is about their own code: no opt-in rule reports them, and a rule's finding whose subject is one of them is dropped. A Context row needs a reason, as an Out row does (R23).

## read-not-checked

`unmodelled.file` is on. `Order.cs` holds the modelled `Order`. `OrderNumber.cs` is In and holds nothing the model describes, so it is reported. `Program.cs` holds nothing the model describes either, but its row is Context: it is read, and not reported.

```json case
{ "phase": 2, "cases": ["scope/context", "optin/unmodelled.file"], "requirements": ["R20", "R22", "R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/Api/**", "Scope": "Context", "Reason": "The composition root: rules read its registrations, and the model doesn't describe it" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ] }
        } }]
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

```csharp file src/Domain/OrderNumber.cs
namespace Shop.Domain;

public static class OrderNumber
{
    public static string Next() => Guid.NewGuid().ToString("N")[..8];
}
```

```csharp file src/Api/Program.cs
namespace Shop.Api;

public static class Program
{
    public static void Main(string[] args)
    {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/Domain/OrderNumber.cs:1" },
    { "rule": "optin/unmodelled.file", "subject": "src/Api/Program.cs:1", "count": 0, "note": "Context code is read, not checked" },
    { "rule": "scope/reason", "count": 0 },
    { "rule": "scope/coverage", "severity": "info", "messageContains": "read for context only" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs", "src/Domain/OrderNumber.cs", "src/Api/Program.cs"] }
}
```

## claims-verify-from-context

The model describes `Order`, whose code is in a Context file. The file is read, so the claim verifies from it: Context keeps the file's own code from being reported, not the model from being checked against it. `Order`'s unmodelled public member isn't reported either.

```json case
{ "phase": 2, "cases": ["scope/context", "optin/unmodelled.member"], "requirements": ["R20", "R22", "R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/Shared/**", "Scope": "Context", "Reason": "Shared with the billing service, which owns it" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Shared/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }

    public string Reference { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Shared/Order.cs:3" },
    { "kind": "member.exists", "element": "order-id", "outcome": "verified", "at": "src/Shared/Order.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 0, "note": "Order.Reference is in a Context file" }
  ],
  "filesRead": { "include": ["src/Shared/Order.cs"] }
}
```

## context-row-without-reason

Row 2 reads `src/Api/**` for context and says nothing about why. Context keeps code from being checked as an Out row does, so it needs a reason as much.

```json case
{ "phase": 1, "cases": ["scope/context", "scope/reason"], "requirements": ["R20", "R23"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
          { "Glob": "src/Api/**", "Scope": "Context" }
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

```csharp file src/Api/Program.cs
namespace Shop.Api;

public static class Program
{
}
```

```json expect
{
  "findings": [
    { "rule": "scope/reason", "severity": "error", "subject": "root", "messageContains": "is Context with no reason" }
  ],
  "filesRead": { "include": ["src/Api/Program.cs"] }
}
```
