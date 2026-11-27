# scope/glob

A row's Glob follows the File Classification matcher's rules (R20, scope.md's Glob rules), which take neither a leading `/`, braces `{a,b}` nor character classes `[ab]`. A line using one matches no file, which would quietly change what the row means, so each is a `config/glob` error on the element that holds the row, naming the row, the line and the fix. That holds for a Drift Scope row and for a Drift Check Opt-in Rules row alike. A `!` line and a `#` comment are the matcher's own and are fine.

## unsupported-scope-globs

Row 1 is fine: a comment, `src/**`, and a `!` line taking generated files back. Rows 2, 3 and 4 each use something the matcher can't take, so each is one finding. None of them leaves anything out, so `src/Legacy/OrderService.cs` and `src/Tools/Seed.cs` are still read. They decide no file, but they aren't also `scope/stale`: the error has said why.

```json case
{ "phase": 1, "cases": ["scope/glob"], "requirements": ["R20"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
          { "Glob": "# All the source\nsrc/**\n!src/**/*.g.cs", "Scope": "In" },
          { "Glob": "/src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" },
          { "Glob": "src/{Tools,Scripts}/**", "Scope": "Out", "Reason": "Build helpers" },
          { "Glob": "src/Domain/[AB]*.cs", "Scope": "Out", "Reason": "Scaffolded" }
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

```csharp file src/Tools/Seed.cs
namespace Shop.Tools;

public static class Seed
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "config": [
    { "rule": "config/glob", "severity": "error", "subject": "root", "messageContains": "Scope row 2 (/src/Legacy/**) has the glob `/src/Legacy/**`, which matches no file: a glob is relative to the row's folder, so a leading `/` matches nothing; drop it." },
    { "rule": "config/glob", "severity": "error", "subject": "root", "messageContains": "Scope row 3 (src/{Tools,Scripts}/**) has the glob `src/{Tools,Scripts}/**`, which matches no file: braces aren't supported; give each alternative a line of its own." },
    { "rule": "config/glob", "severity": "error", "subject": "root", "messageContains": "Scope row 4 (src/Domain/[AB]*.cs) has the glob `src/Domain/[AB]*.cs`, which matches no file: character classes aren't supported" }
  ],
  "findings": [
    { "rule": "config/glob", "messageContains": "Scope row 1 ", "count": 0, "note": "A comment, ** and a ! line are the matcher's own" },
    { "rule": "scope/stale", "count": 0, "note": "Rows 2 to 4 decide nothing, and their config/glob says why" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs", "src/Legacy/OrderService.cs", "src/Tools/Seed.cs"] }
}
```

## unsupported-opt-in-glob

An Opt-in Rules row's Glob is read the same way, so braces in it are one `config/glob` error on the Root Folder that holds the Drift Check.

```json case
{ "phase": 2, "base": "unsupported-scope-globs", "cases": ["scope/glob", "optin/unmodelled.type"], "requirements": ["R20", "R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type", "Glob": "src/{Domain,Legacy}/**" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "config": [
    { "rule": "config/glob", "severity": "error", "subject": "root", "messageContains": "Opt-in Rules row 1 of the Drift Check (optin/unmodelled.type) has the glob `src/{Domain,Legacy}/**`, which matches no file: braces aren't supported" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.type", "count": 0, "note": "The row's only glob matches nothing" }
  ]
}
```
