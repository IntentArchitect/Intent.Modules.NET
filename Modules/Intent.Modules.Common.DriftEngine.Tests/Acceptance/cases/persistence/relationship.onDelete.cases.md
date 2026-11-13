# relationship.onDelete

The On Delete stereotype's Behavior against the code's `OnDelete(DeleteBehavior.X)`. On Delete is eShopCustom's own Domain module stereotype, not a stock one, so its vocabulary ships with that module's pack; these cases bring it inline as `eshop.domain` (packs.md, "Who ships what"). A navigable association end's stereotype states it for the association, and a foreign key attribute with References (a relationship with no navigation) states it for the attribute. Rules are keyed by kind, so EF Core's rule reads `OnDelete` for both, whichever pack stated the claim. `relationship.onDelete` is `stated: whenSet`: a modelled behaviour the code doesn't state is contradicted, never taken from EF Core's default for the relationship (decision 4).

## verified

The `Buyer` end of `Order` → `Buyer` has On Delete `Restrict`, and the configuration's chain ends in `OnDelete(DeleteBehavior.Restrict)` (ESC-DOM-58).

```json case
{ "phase": 4, "kinds": ["relationship.onDelete"], "traces": ["ESC-DOM-58"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```json pack
{
  "id": "eshop.domain",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "description": "The vocabulary for eShopCustom's Domain module stereotypes On Delete and References.",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }, { "id": "intent.efcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "relationship.onDelete",
          "for": { "association": "Association", "where": "target.isNavigable" },
          "value": "target.stereotype('On Delete').Behavior",
          "bindings": { "Source": "source.element.name", "Target": "target.element.name", "SourceEnd": "source.name", "TargetEnd": "target.name" },
          "own": ["SourceEnd", "TargetEnd"],
          "source": "the On Delete stereotype's Behavior on the association's navigable end",
          "fixHint": "Change the On Delete stereotype's Behavior, or the code's OnDelete."
        },
        {
          "kind": "relationship.onDelete",
          "for": { "element": "Attribute", "where": "hasStereotype('References')" },
          "value": "stereotype('On Delete').Behavior",
          "bindings": { "Entity": "parent.name", "Attribute": "name", "Target": "stereotype('References').Target.name" },
          "own": ["Entity", "Attribute"],
          "dependsOn": "parent:type.exists",
          "source": "the On Delete stereotype's Behavior on the References attribute",
          "fixHint": "Change the On Delete stereotype's Behavior, or the code's OnDelete."
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
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "0..1", "navigable": true,
            "stereotypes": { "On Delete": { "Behavior": "Restrict" } } } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public int? BuyerId { get; private set; }

    public Buyer? Buyer { get; private set; }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId).OnDelete(DeleteBehavior.Restrict);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.onDelete", "element": "order-buyer", "outcome": "verified", "expected": "Restrict", "actual": "Restrict",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The code cascades deletes from a buyer to its orders, and the model says `Restrict`. The finding names the On Delete stereotype's Behavior as the model property that states it (R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.onDelete"], "traces": ["ESC-DOM-58"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId).OnDelete(DeleteBehavior.Cascade);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.onDelete", "element": "order-buyer", "outcome": "contradicted", "expected": "Restrict", "actual": "Cascade",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/relationship.onDelete", "severity": "error", "subject": "order-buyer", "messageContains": "code `Cascade`" }
  ]
}
```

## contradicted-not-stated

The chain has no `OnDelete`. EF Core's default for an optional relationship is `ClientSetNull`, but a `whenSet` value is never compared with a framework default: the code doesn't state a behaviour, so the modelled `Restrict` is contradicted, with the code side "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.onDelete"], "traces": ["ESC-DOM-58"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.onDelete", "element": "order-buyer", "outcome": "contradicted", "expected": "Restrict", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/relationship.onDelete", "severity": "error", "subject": "order-buyer", "messageContains": "doesn't state" }
  ]
}
```

## verified-references-attribute

A relationship with no navigation: `Order.PaymentId` carries References (Target `PaymentMethod`) and On Delete `Restrict`, and the configuration's `HasOne<PaymentMethod>().WithMany().HasForeignKey(o => o.PaymentId)` ends in `OnDelete(DeleteBehavior.Restrict)`, as eShop's order configuration has it. The claim is stated by the attribute (ESC-DOM-58).

```json case
{ "phase": 4, "base": "verified", "without": ["src/Domain/Buyer.cs"], "kinds": ["relationship.onDelete"], "traces": ["ESC-DOM-58"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-payment-id", "type": "Attribute", "name": "PaymentId", "typeRef": { "type": "int", "nullable": true },
                "stereotypes": { "References": { "Target": { "ref": "payment-method" } }, "On Delete": { "Behavior": "Restrict" } } }
            ] },
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "payment-method-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public int? PaymentId { get; private set; }
}
```

```csharp file src/Domain/PaymentMethod.cs
namespace Ordering.Domain;

public class PaymentMethod
{
    public int Id { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne<PaymentMethod>().WithMany().HasForeignKey(o => o.PaymentId).OnDelete(DeleteBehavior.Restrict);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.onDelete", "element": "order-payment-id", "outcome": "verified", "expected": "Restrict", "actual": "Restrict",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## cant-verify-out-of-scope

The only configuration for `Order` sits under `src/Legacy`, which a scope row excludes. The engine never opens it, but its path matches the find's named step, so the claim is can't verify, `outOfScope`, naming the file and the row (R25), rather than "not stated".

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["relationship.onDelete"], "cases": ["scope/shadow"], "traces": ["ESC-DOM-58"], "requirements": ["R5", "R22", "R25"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "0..1", "navigable": true,
            "stereotypes": { "On Delete": { "Behavior": "Restrict" } } } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Infrastructure" }
        ] } } }]
    }
  }
}
```

```csharp file src/Legacy/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Legacy;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId).OnDelete(DeleteBehavior.Restrict);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.onDelete", "element": "order-buyer", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/relationship.onDelete", "severity": "error", "subject": "order-buyer", "messageContains": "src/Legacy/OrderConfiguration.cs" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Legacy/**"] }
}
```
