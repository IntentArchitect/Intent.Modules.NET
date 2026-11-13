# engine/unresolved

Each reader declares what it can do (R35): name resolution, nullable context, call targets. A rule that needs something the reader can't supply for this code gives can't verify, `unresolved` (outcome step 5), never a guess. For C# nullability that is the nullable context: inside one, `string` is not nullable and `string?` is; outside one, a reference type says nothing, and its reading is `unknown`.

## no-project-for-shared-file

`OrderNumber.cs` sits in a `Shared` folder that projects link in with `<Compile Include>`. The reader reads files and never evaluates project items, so the file belongs to no project it read and has no nullable context. The `Prefix` property's nullability reading is `unknown`, and the claim is can't verify, `unresolved`. Its type is still read: `string` is verified.

```json case
{ "phase": 1, "cases": ["engine/unresolved"], "requirements": ["R1", "R5", "R35"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-number", "type": "Value Object", "name": "OrderNumber",
            "children": [
              { "id": "order-number-prefix", "type": "Attribute", "name": "Prefix", "typeRef": { "type": "string" } },
              { "id": "order-number-sequence", "type": "Attribute", "name": "Sequence", "typeRef": { "type": "int" } }
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

```csharp file src/Shared/OrderNumber.cs
namespace Shop.Shared;

public class OrderNumber
{
    public string Prefix { get; set; }

    public int Sequence { get; set; }

    public override string ToString() => $"{Prefix}-{Sequence:D6}";
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-number-prefix", "outcome": "cantVerify", "reason": "unresolved" },
    { "kind": "member.type", "element": "order-number-prefix", "outcome": "verified", "at": "src/Shared/OrderNumber.cs:5" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "subject": "order-number-prefix", "count": 0 },
    { "rule": "engine/notStated", "severity": "info", "messageContains": "Not checked: 1 nullability claims (Domain 1)" }
  ]
}
```

## nullable-directive-gives-the-context

The same file, opted in with `#nullable enable` at its top. The reader knows the context from the directive alone, so `string Prefix` is not nullable, and the claim is verified.

```json case
{ "phase": 1, "base": "no-project-for-shared-file", "cases": ["engine/unresolved"], "requirements": ["R2", "R35"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Shared/OrderNumber.cs
#nullable enable

namespace Shop.Shared;

public class OrderNumber
{
    public string Prefix { get; set; } = "";

    public int Sequence { get; set; }

    public override string ToString() => $"{Prefix}-{Sequence:D6}";
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-number-prefix", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Shared/OrderNumber.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "subject": "order-number-prefix", "count": 0 }
  ]
}
```
