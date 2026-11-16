# engine/ruleFailed

A claim reader that throws leaves its claim can't verify, `ruleFailed` (outcome step 3), with the module named in the message: the fault is in the module, not in the model or the code, so neither is blamed (R45; rules.md, Plug-ins). The run goes on, and the module's other claims and every other module's are read as usual.

## reader-throws

A house module reads `type.marker` with a claim reader of its own, which throws on `Order`'s Aggregate Root marker. The claim is can't verify, `ruleFailed`, and the finding names the module. Fixing the module's reader fixes the claim.

```json case
{ "phase": 2, "kinds": ["type.marker"], "cases": ["engine/ruleFailed"], "requirements": ["R1", "R5", "R45"], "packs": ["intent.core", "intent.domain", "house.shop", "house.faulty"], "language": "csharp" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "type.marker",
          "for": { "element": "Class", "where": "hasStereotype('Aggregate Root')" },
          "discriminator": "'AggregateRoot'",
          "value": "true",
          "dependsOn": "type.exists",
          "source": "the Aggregate Root stereotype"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Aggregate Root": {} },
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

public class Order : AggregateRoot
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified" },
    { "kind": "type.marker", "element": "order", "outcome": "cantVerify", "reason": "ruleFailed" }
  ],
  "findings": [
    { "rule": "claim/type.marker", "severity": "error", "subject": "order", "messageContains": "house.faulty" }
  ]
}
```
