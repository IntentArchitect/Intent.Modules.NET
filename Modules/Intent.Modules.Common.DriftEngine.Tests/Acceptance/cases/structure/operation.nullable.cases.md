# operation.nullable

A modelled operation with a return type returns null exactly when its type reference says so. The code side is the method's return type, unwrapped of the language profile's async wrappers as for `operation.returns` (`Task<Buyer?>` is `Buyer?`), and its `nullable`: `true`, `false` or `unknown`, as for `member.nullable`. `operation.returns` compares the type without its nullability, so a return that differs only in nullability is this kind's one finding. An operation with no return type gives no claim. An `unknown` reading is can't verify, `unresolved`, and listed in the one `engine/notStated` note (R35).

## verified

The project turns nullable reference types on. `FindByEmail` may return no buyer, as the model's nullable return says; `GetName` always returns a name.

```json case
{ "phase": 1, "kinds": ["operation.nullable"], "traces": ["ESC-DOM-64"], "requirements": ["R2", "R35", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer" },
          { "id": "buyers", "type": "Domain Service", "name": "BuyerLookup",
            "children": [
              { "id": "buyers-find", "type": "Operation", "name": "FindByEmail", "typeRef": { "element": "buyer", "nullable": true } },
              { "id": "buyers-name", "type": "Operation", "name": "GetName", "typeRef": { "type": "string" } }
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

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/Domain/Buyer.cs
namespace Shop.Domain;

public class Buyer
{
}
```

```csharp file src/Domain/BuyerLookup.cs
namespace Shop.Domain;

public class BuyerLookup
{
    public Task<Buyer?> FindByEmail() => Task.FromResult<Buyer?>(null);

    public string GetName() => "";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.nullable", "element": "buyers-find", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Domain/BuyerLookup.cs:5" },
    { "kind": "operation.nullable", "element": "buyers-name", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Domain/BuyerLookup.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.nullable", "count": 0 }
  ]
}
```

## contradicted

`FindByEmail` returns `Task<Buyer>`, never null, against the model's nullable return: contradicted. Its type agrees, so `operation.returns` is verified and this is the one finding.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.nullable", "operation.returns"], "traces": ["ESC-DOM-64"], "requirements": ["R1", "R11", "R35"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/BuyerLookup.cs
namespace Shop.Domain;

public class BuyerLookup
{
    public Task<Buyer> FindByEmail() => Task.FromResult(new Buyer());

    public string GetName() => "";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "buyers-find", "outcome": "verified" },
    { "kind": "operation.nullable", "element": "buyers-find", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/Domain/BuyerLookup.cs:5" }
  ],
  "findings": [
    { "rule": "claim/operation.nullable", "severity": "error", "subject": "buyers-find" },
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## blocked

`BuyerLookup` has no `FindByEmail`. Its `member.exists` claim is contradicted, and its nullability waits on it: can't verify, `blocked`.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.nullable", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/BuyerLookup.cs
namespace Shop.Domain;

public class BuyerLookup
{
    public string GetName() => "";
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "buyers-find", "outcome": "contradicted" },
    { "kind": "operation.nullable", "element": "buyers-find", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "buyers-find" },
    { "rule": "claim/operation.nullable", "count": 0 }
  ]
}
```

## unresolved-nullable-disabled

The project turns nullable reference types off, so `Task<Buyer>` says nothing about whether the buyer may be null: can't verify, `unresolved`, listed in the `engine/notStated` note rather than reported. `GetName`'s `string` is unknown too.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.nullable"], "cases": ["engine/unresolved", "engine/notStated"], "requirements": ["R1", "R5", "R35"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/Domain/BuyerLookup.cs
namespace Shop.Domain;

public class BuyerLookup
{
    public Task<Buyer> FindByEmail() => Task.FromResult(new Buyer());

    public string GetName() => "";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.nullable", "element": "buyers-find", "outcome": "cantVerify", "reason": "unresolved" },
    { "kind": "operation.nullable", "element": "buyers-name", "outcome": "cantVerify", "reason": "unresolved" }
  ],
  "findings": [
    { "rule": "claim/operation.nullable", "count": 0 },
    { "rule": "engine/notStated", "severity": "info", "messageContains": "Not checked: 2 nullability claims (Domain 2)" }
  ]
}
```
