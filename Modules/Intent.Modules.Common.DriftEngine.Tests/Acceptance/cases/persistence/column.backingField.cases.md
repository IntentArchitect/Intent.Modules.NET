# column.backingField

The field an attribute of a persisted class is mapped through: eShopCustom's own Backing Field stereotype's Field, such as `_cardNumber` on a `PaymentMethod` that keeps its card details in private fields with no properties. EF Core states it with `Property("_cardNumber")` in the configuration. eShopCustom checked it as `persistence.field-unmapped` (ESC-DOM-41). No stock pack knows the module's stereotype, so these cases give its vocabulary in a house pack. The kind is phase `later`, so these cases are too: they need no outcome triple, and they fix what the kind must do when it arrives. The claim depends on its class, not on `member.exists`, because the attribute has no property of its own name to find. `column.backingField` is `stated: whenSet`: a field EF Core would bind by convention is never read in place of a modelled one (decision 4).

## verified

`PaymentMethod.CardNumber` is modelled with Backing Field `_cardNumber`, and the configuration maps it with `Property("_cardNumber")`.

```json case
{ "phase": "later", "kinds": ["column.backingField"], "traces": ["ESC-DOM-41"], "requirements": ["R2", "R29"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }, { "id": "intent.efcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "column.backingField",
          "for": { "element": "Attribute", "where": "parent.hasStereotype('Table') && hasStereotype('Backing Field')" },
          "value": "stereotype('Backing Field').Field",
          "bindings": { "Entity": "parent.name", "Attribute": "name", "BackingField": "stereotype('Backing Field').Field" },
          "own": ["Attribute"],
          "dependsOn": "parent:type.exists",
          "source": "the Backing Field stereotype's Field"
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
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "payment-method-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "payment-method-card-number", "type": "Attribute", "name": "CardNumber", "typeRef": { "type": "string" },
                "stereotypes": { "Backing Field": { "Field": "_cardNumber" } } }
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

```csharp file src/Domain/PaymentMethod.cs
namespace Shop.Domain;

public class PaymentMethod
{
    public int Id { get; private set; }

    private string _cardNumber;
}
```

```csharp file src/Infrastructure/Persistence/PaymentMethodConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.Property("_cardNumber").IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.backingField", "element": "payment-method-card-number", "outcome": "verified", "expected": "_cardNumber", "actual": "_cardNumber",
      "at": "src/Infrastructure/Persistence/PaymentMethodConfiguration.cs:11" }
  ]
}
```

## contradicted-not-stated

The configuration never maps `_cardNumber`. EF Core doesn't map a private field that no property exposes, so the card number isn't persisted at all: the modelled backing field is contradicted, "not stated" (decision 4), the drift `persistence.field-unmapped` caught.

```json case
{ "phase": "later", "base": "verified", "kinds": ["column.backingField"], "traces": ["ESC-DOM-41"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/PaymentMethodConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.backingField", "element": "payment-method-card-number", "outcome": "contradicted", "expected": "_cardNumber", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.backingField", "severity": "error", "subject": "payment-method-card-number", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The attribute has no Backing Field stereotype, and the configuration maps `Property("_cardNumber")`. Nothing in the model states a backing field, so there is no claim and nothing is reported about one (principle 1).

```json case
{ "phase": "later", "base": "verified", "kinds": ["column.backingField"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "payment-method-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "payment-method-card-number", "type": "Attribute", "name": "CardNumber", "typeRef": { "type": "string" } }
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

```json expect
{
  "claims": [
    { "kind": "column.backingField", "element": "payment-method-card-number", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.backingField", "count": 0 }
  ]
}
```
