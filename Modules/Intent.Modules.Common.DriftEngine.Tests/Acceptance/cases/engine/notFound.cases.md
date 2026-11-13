# engine/notFound

When a rule finds nothing, or finds its place but no read case matches, and nothing else in the outcome order applies (no shadow in excluded code, not an existence kind, not `whenSet`, no `absent` or `default` in the recipe), the claim is can't verify, `notFound`, and the finding says which of the two happened (outcome step 12). When no code was found, the finding names what was searched instead of a file and line (R11).

Both cases use a house pack whose recipe for a Layer's contents finds the C# files under the Layer's Path and reads `true` from one that declares a namespace. With `"matches": "any"`, a Layer of many files reads its first rather than being `ambiguous`. It deliberately has no `absent`: nothing found is "can't tell", not "empty".

## nothing-found

The Application Layer's Path, `src/Application`, holds no files, and no excluded path matches it either. The claim is can't verify, `notFound`, and the finding names the Path it searched. The Domain Layer is verified.

```json case
{ "phase": 2, "kinds": ["layer.populated"], "cases": ["engine/notFound"], "requirements": ["R5", "R11"], "packs": ["intent.core", "house.shop"], "language": "csharp" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["Codebase Structure"],
  "kinds": [
    { "kind": "layer.populated", "label": "layer contents", "valueType": "bool", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Codebase Structure": {
      "entries": [
        {
          "kind": "layer.populated",
          "for": { "element": "Layer" },
          "value": "true",
          "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
          "own": ["Layer"],
          "source": "the Layer and its Layer Settings Path"
        }
      ]
    }
  },
  "rules": {
    "Codebase Structure": {
      "facts": {
        "layer.populated": {
          "find": "file:{Path}/**/*.cs",
          "matches": "any",
          "read": [{ "match": "\\bnamespace\\s", "value": "true" }]
        }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/Domain" } } },
          { "id": "application-layer", "type": "Layer", "name": "Application",
            "stereotypes": { "Layer Settings": { "Path": "src/Application" } } }
        ] }]
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
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "verified", "at": "src/Domain/Order.cs:1" },
    { "kind": "layer.populated", "element": "application-layer", "outcome": "cantVerify", "reason": "notFound" }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "severity": "error", "subject": "application-layer", "messageContains": "src/Application" }
  ]
}
```

## found-but-no-read-case-matched

Now `src/Application` holds one file, its global usings, which declares no namespace. The find found it, but no read case matched, so the claim is again can't verify, `notFound`, and this time the finding names the file it found.

```json case
{ "phase": 2, "base": "nothing-found", "kinds": ["layer.populated"], "cases": ["engine/notFound"], "requirements": ["R5", "R11"], "packs": ["intent.core", "house.shop"], "language": "csharp" }
```

```csharp file src/Application/GlobalUsings.cs
global using MediatR;
global using Shop.Domain;
```

```json expect
{
  "claims": [
    { "kind": "layer.populated", "element": "application-layer", "outcome": "cantVerify", "reason": "notFound" }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "severity": "error", "subject": "application-layer", "messageContains": "src/Application/GlobalUsings.cs" }
  ]
}
```
