# engine/ruleFailed

A rule that throws, or returns something malformed, leaves its claim can't verify, `ruleFailed` (outcome step 3), with the rule named in the message: the fault is in the pack, not in the model or the code, so neither is blamed. Malformed means a value that doesn't fit the kind's value type (text for a `bool` or `number` kind), or a node field path that doesn't exist on the node the rule reached, and it applies to any rule, recipe, query or plug-in alike, since all three return the same reading (R45; rules.md, Plug-ins). What can be checked without code, such as a bad regular expression, is a `config/ruleInvalid` error when the pack loads instead (rules/recipe.cases.md). A plug-in that throws is shown in rules/plugin.cases.md.

## malformed-reading

The house pack's recipe for a Layer's contents reads the namespace of the first file under the Layer's Path (`"matches": "any"`). Its read case has no `value`, so it returns the captured namespace, `Shop.Domain`, for a kind whose value is a bool. That reading is malformed: the claim is can't verify, `ruleFailed`, and the finding names the rule. Giving the read case `"value": "true"` fixes the pack.

```json case
{ "phase": 2, "kinds": ["layer.populated"], "cases": ["engine/ruleFailed"], "requirements": ["R1", "R5", "R45"], "packs": ["intent.core", "house.shop"], "language": "csharp" }
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
          "read": [{ "match": "namespace ([\\w.]+)" }]
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
            "stereotypes": { "Layer Settings": { "Path": "src/Domain" } } }
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
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "cantVerify", "reason": "ruleFailed" }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "severity": "error", "subject": "domain-layer", "messageContains": "house.shop" }
  ]
}
```
