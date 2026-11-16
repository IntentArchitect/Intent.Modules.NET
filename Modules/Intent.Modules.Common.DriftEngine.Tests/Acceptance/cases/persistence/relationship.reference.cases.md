# relationship.reference

A relationship with no navigation property: a foreign key attribute with eShopCustom's References stereotype, whose Target holds the referenced class, against `HasOne<Target>().WithMany().HasForeignKey(x => x.Fk)` with no navigation on either side. There is no association in the model for such a relationship, so the claim is stated by the foreign key attribute (claim-catalog.md, Notes on kinds). References is eShopCustom's own Domain module stereotype, so its vocabulary ships with that module's pack; these cases bring it inline as `eshop.domain` (packs.md, "Who ships what"), and the binding follows the Target to the class's name. The rule that reads the chain is EF Core's, keyed by kind. `relationship.reference` is an existence kind (`presence`): a modelled reference the code doesn't configure compares as `absent` and is contradicted.

## verified

`Order.PaymentId` references `PaymentMethod`, and the configuration has the navigation-less chain for it, as eShop's order configuration does (ESC-DOM-59).

```json case
{ "phase": 4, "kinds": ["relationship.reference"], "traces": ["ESC-DOM-59"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```json pack
{
  "id": "eshop.domain",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "description": "The vocabulary for eShopCustom's Domain module stereotype References.",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }, { "id": "intent.efcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "relationship.reference",
          "for": { "element": "Attribute", "where": "hasStereotype('References')" },
          "value": "'present'",
          "bindings": { "Entity": "parent.name", "Attribute": "name", "Target": "stereotype('References').Target.name" },
          "own": ["Entity", "Attribute"],
          "dependsOn": "parent:type.exists",
          "source": "the References stereotype on the foreign key attribute",
          "fixHint": "Configure the relationship as HasOne<{Target}>().WithMany().HasForeignKey(x => x.{Attribute}), or model it as an association if the code navigates it."
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-payment-id", "type": "Attribute", "name": "PaymentId", "typeRef": { "type": "int", "nullable": true },
                "stereotypes": { "References": { "Target": { "ref": "payment-method" } } } }
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
        builder.HasOne<PaymentMethod>().WithMany().HasForeignKey(o => o.PaymentId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.reference", "element": "order-payment-id", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted-missing

The configuration doesn't configure the relationship at all; `PaymentId` is a plain column. The reference is an existence claim, so nothing found is contradicted: the code lacks it (R8).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.reference"], "traces": ["ESC-DOM-59"], "requirements": ["R1", "R8", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
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
        builder.HasKey(o => o.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.reference", "element": "order-payment-id", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/relationship.reference", "severity": "error", "subject": "order-payment-id", "messageContains": "isn't in the code" }
  ]
}
```

## contradicted-other-target

The chain uses `PaymentId` as the foreign key of a relationship to `CardType`, not to the References Target `PaymentMethod`. The Target is part of what the model states, so no relationship to `PaymentMethod` is found and the claim is contradicted (ESC-DOM-59).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.reference"], "traces": ["ESC-DOM-59"], "requirements": ["R1", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
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
        builder.HasOne<CardType>().WithMany().HasForeignKey(o => o.PaymentId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.reference", "element": "order-payment-id", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/relationship.reference", "severity": "error", "subject": "order-payment-id" }
  ]
}
```

## contradicted-has-navigation

`Order` has a `PaymentMethod` navigation and the chain names it. References states a relationship with no navigation, so the navigation-less chain isn't there and the claim is contradicted; a navigated relationship is modelled as an association instead (ESC-DOM-59).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.reference"], "traces": ["ESC-DOM-59"], "requirements": ["R1", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public int? PaymentId { get; private set; }

    public PaymentMethod? PaymentMethod { get; private set; }
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
        builder.HasOne(o => o.PaymentMethod).WithMany().HasForeignKey(o => o.PaymentId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.reference", "element": "order-payment-id", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/relationship.reference", "severity": "error", "subject": "order-payment-id" }
  ]
}
```

## blocked-when-type-missing

`Order` itself isn't in the code. `type.exists` on `Order` is contradicted, and the reference on its attribute, which depends on it, is can't verify, `blocked`: only the missing type is reported (R5).

```json case
{ "phase": 4, "base": "verified", "without": ["src/Domain/Order.cs"], "kinds": ["relationship.reference", "type.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore", "eshop.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "contradicted" },
    { "kind": "relationship.reference", "element": "order-payment-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "isn't in the code" },
    { "rule": "claim/relationship.reference", "subject": "order-payment-id", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```
