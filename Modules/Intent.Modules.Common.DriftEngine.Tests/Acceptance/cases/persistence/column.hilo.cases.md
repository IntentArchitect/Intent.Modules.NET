# column.hilo

The database sequence a key attribute takes its values from in blocks: eShopCustom's own HiLo stereotype's Sequence, such as `orderseq` on `Order.Id`. EF Core states it with `UseHiLo("orderseq")`. eShopCustom checked it both ways (ESC-DOM-44). No stock pack knows the module's stereotype, so these cases give its vocabulary in a house pack. The kind is phase `later`, so these cases are too: they need no outcome triple, and they fix what the kind must do when it arrives. `column.hilo` is `stated: whenSet`: a sequence name EF Core would choose by convention is never read in place of a modelled one (decision 4).

## verified

eShopCustom's `Order` takes its ids from the `orderseq` sequence, and the configuration says `UseHiLo("orderseq")`.

```json case
{ "phase": "later", "kinds": ["column.hilo"], "traces": ["ESC-DOM-44"], "requirements": ["R2", "R29"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
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
          "kind": "column.hilo",
          "for": { "element": "Attribute", "where": "parent.hasStereotype('Table') && hasStereotype('HiLo')" },
          "value": "stereotype('HiLo').Sequence",
          "bindings": { "Entity": "parent.name", "Attribute": "name" },
          "own": ["Attribute"],
          "dependsOn": "member.exists",
          "source": "the HiLo stereotype's Sequence"
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
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" },
                "stereotypes": { "Primary Key": {}, "HiLo": { "Sequence": "orderseq" } } }
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
    public int Id { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Id).UseHiLo("orderseq");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.hilo", "element": "order-id", "outcome": "verified", "expected": "orderseq", "actual": "orderseq",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The configuration names another sequence. The finding names the HiLo stereotype's Sequence as the model's side (R14).

```json case
{ "phase": "later", "base": "verified", "kinds": ["column.hilo"], "traces": ["ESC-DOM-44"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Id).UseHiLo("order_hilo");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.hilo", "element": "order-id", "outcome": "contradicted", "expected": "orderseq", "actual": "order_hilo",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.hilo", "severity": "error", "subject": "order-id", "messageContains": "HiLo" }
  ]
}
```

## contradicted-not-stated

The model says the key comes from `orderseq`, and the configuration has no `UseHiLo`, so the key is an identity column. The modelled sequence is contradicted, "not stated" (decision 4): ESC-DOM-44's model-to-code way.

```json case
{ "phase": "later", "base": "verified", "kinds": ["column.hilo"], "traces": ["ESC-DOM-44"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.hilo", "element": "order-id", "outcome": "contradicted", "expected": "orderseq", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.hilo", "severity": "error", "subject": "order-id", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The key has no HiLo stereotype, and the configuration says `UseHiLo("orderseq")`. Nothing in the model states a sequence, so there is no claim and nothing is reported (principle 1). ESC-DOM-44's code-to-model way is the opt-in `unmodelled.value` rule's job.

```json case
{ "phase": "later", "base": "verified", "kinds": ["column.hilo"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
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
    { "kind": "column.hilo", "element": "order-id", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.hilo", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```
