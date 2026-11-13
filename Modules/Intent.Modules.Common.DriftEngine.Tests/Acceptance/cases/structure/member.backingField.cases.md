# member.backingField

An attribute persisted from a private field names that field: eShopCustom's own Backing Field stereotype, with its Field property. No stock pack knows that stereotype, so eShopCustom's house pack gives the vocabulary and the rule (R28). It is a `stated: whenSet` kind: the claim exists only where the stereotype names a field, and a named field the code doesn't have is contradicted as not stated, never verified from a convention (decision 4, R4). The kind's phase is `later`, so these cases trace the check rather than gate a phase.

ESC-DOM-40 also compares the field's type with the attribute's. This kind compares only the field's name; the type half isn't shown here.

## verified

`PaymentMethod`'s `Expiration` and `CardTypeId` are persisted from the private fields `_expiration` and `_cardTypeId`, as their Backing Field stereotypes say. `Alias` has no Backing Field, so it gives no claim.

```json case
{ "phase": "later", "kinds": ["member.backingField"], "traces": ["ESC-DOM-40"], "requirements": ["R2", "R28", "R31", "R53"], "packs": ["intent.core", "intent.domain", "house.eshop.domain"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop.domain",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "member.backingField",
          "for": { "element": "Attribute", "where": "hasStereotype('Backing Field')" },
          "value": "stereotype('Backing Field').Field",
          "bindings": { "Entity": "parent.name", "Attribute": "name", "BackingField": "stereotype('Backing Field').Field" },
          "own": ["BackingField"],
          "dependsOn": "parent:type.exists",
          "source": "the Backing Field stereotype's Field",
          "fixHint": "Set the Backing Field to the field the code has, or add the field."
        }
      ]
    }
  },
  "rules": {
    "Domain": {
      "files": "src/**/*.cs",
      "facts": {
        "member.backingField": {
          "find": "type:{Entity} > field with \\b{BackingField}\\b",
          "read": [{ "node": "name" }],
          "note": "The field's declared name. A find that ends nowhere leaves the value not stated."
        }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "children": [
              { "id": "payment-method-expiration", "type": "Attribute", "name": "Expiration", "typeRef": { "type": "datetime" },
                "stereotypes": { "Backing Field": { "Field": "_expiration" } } },
              { "id": "payment-method-card-type-id", "type": "Attribute", "name": "CardTypeId", "typeRef": { "type": "int" },
                "stereotypes": { "Backing Field": { "Field": "_cardTypeId" } } },
              { "id": "payment-method-alias", "type": "Attribute", "name": "Alias", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/BuyerAggregate/PaymentMethod.cs
namespace eShop.Ordering.Domain.AggregatesModel.BuyerAggregate;

public class PaymentMethod : Entity
{
    private DateTime _expiration;

    private int _cardTypeId;

    public string Alias { get; private set; }

    protected PaymentMethod() { }
}
```

```json expect
{
  "claims": [
    { "kind": "member.backingField", "element": "payment-method-expiration", "outcome": "verified", "expected": "_expiration", "actual": "_expiration", "at": "src/Ordering.Domain/AggregatesModel/BuyerAggregate/PaymentMethod.cs:5" },
    { "kind": "member.backingField", "element": "payment-method-card-type-id", "outcome": "verified", "expected": "_cardTypeId", "actual": "_cardTypeId", "at": "src/Ordering.Domain/AggregatesModel/BuyerAggregate/PaymentMethod.cs:7" },
    { "kind": "member.backingField", "element": "payment-method-alias", "absent": true }
  ]
}
```

## field-missing

The class's field is `_expiry`, not `_expiration`. `PaymentMethod` is found, the find ends nowhere, and the kind is `whenSet`, so the claim is contradicted with the code side not stated: `attribute.field-missing` in eShopCustom's terms (R4).

```json case
{ "phase": "later", "base": "verified", "kinds": ["member.backingField"], "traces": ["ESC-DOM-40"], "requirements": ["R4", "R11"], "packs": ["intent.core", "intent.domain", "house.eshop.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/BuyerAggregate/PaymentMethod.cs
namespace eShop.Ordering.Domain.AggregatesModel.BuyerAggregate;

public class PaymentMethod : Entity
{
    private DateTime _expiry;

    private int _cardTypeId;

    public string Alias { get; private set; }

    protected PaymentMethod() { }
}
```

```json expect
{
  "claims": [
    { "kind": "member.backingField", "element": "payment-method-expiration", "outcome": "contradicted", "expected": "_expiration" },
    { "kind": "member.backingField", "element": "payment-method-card-type-id", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/BuyerAggregate/PaymentMethod.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.backingField", "severity": "error", "subject": "payment-method-expiration", "messageContains": "doesn't state" }
  ]
}
```
