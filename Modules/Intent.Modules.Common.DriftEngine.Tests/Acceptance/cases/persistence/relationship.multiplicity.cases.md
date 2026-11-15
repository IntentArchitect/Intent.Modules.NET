# relationship.multiplicity

The multiplicity of a Domain association's end that no navigation property states (user decision, 2026-11-15): how many of the navigation's owners each target has. Where only `Buyer.PaymentMethods` is navigable, the `Buyer` end says whether each payment method has exactly one buyer (`1`), at most one (`0..1`) or many (`*`). The navigable end's multiplicity is its property's, checked by `member.type` (a collection or not) and `member.nullable` (optional or not), so it isn't compared again here: one mismatch is one finding. The claim is the association's, on an association with exactly one navigable end whose navigation's owner the model persists (a Table or View). It waits on the navigation's `member.exists`, and is `stated: always`: the Domain designer always states an end's multiplicity.

Over EF Core the rule reads the chain that configures the navigation, else EF Core's convention for the navigation alone. Over JPA it reads the owning field's relationship annotation (JAV-DOM-23).

## verified

`Buyer` owns its payment methods, and only `Buyer.PaymentMethods` is navigable. The model's `Buyer` end is `1`: each payment method has exactly one buyer. The configuration's `HasMany(...).WithOne().IsRequired()` says the same.

```json case
{ "phase": 4, "kinds": ["relationship.multiplicity"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "payment-method-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "buyer-payment-methods", "type": "Association",
          "source": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": false },
          "target": { "element": "payment-method", "name": "PaymentMethods", "multiplicity": "*", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    private readonly List<PaymentMethod> _paymentMethods = new();

    public int Id { get; private set; }

    public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();
}
```

```csharp file src/Domain/PaymentMethod.cs
namespace Ordering.Domain;

public class PaymentMethod
{
    public int Id { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasMany(b => b.PaymentMethods).WithOne().IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "verified", "expected": "1", "actual": "1",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ]
}
```

## contradicted-optional-owner

The configuration makes the relationship optional, `IsRequired(false)`: a payment method may have no buyer. The model's `Buyer` end is `1`, so the claim is contradicted, and the finding shows both values (R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.multiplicity"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasMany(b => b.PaymentMethods).WithOne().IsRequired(false);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "contradicted", "expected": "1", "actual": "0..1",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/relationship.multiplicity", "severity": "error", "subject": "buyer-payment-methods", "messageContains": "code `0..1`" }
  ]
}
```

## contradicted-shared

`WithMany()` makes it many-to-many: a payment method is shared by many buyers, where the model says each has one.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.multiplicity"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasMany(b => b.PaymentMethods).WithMany();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "contradicted", "expected": "1", "actual": "*",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ]
}
```

## owner-from-foreign-key

The chain doesn't say whether the relationship is required; the dependent's foreign key does. `PaymentMethod.BuyerId` is an `int`, which can't be null, so each payment method has a buyer, `1`, and the key's line is where the code says it.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.multiplicity"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/PaymentMethod.cs
namespace Ordering.Domain;

public class PaymentMethod
{
    public int Id { get; private set; }

    public int BuyerId { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasMany(b => b.PaymentMethods).WithOne();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "verified", "expected": "1", "actual": "1",
      "at": "src/Domain/PaymentMethod.cs:7" }
  ]
}
```

## owner-from-shadow-key

eShop's payment methods hold their buyer's key in a shadow property their configuration declares, `Property<int>("BuyerId")`: an `int` can't be null, so each payment method has a buyer, `1`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.multiplicity"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasMany(b => b.PaymentMethods).WithOne();
    }
}
```

```csharp file src/Infrastructure/Persistence/PaymentMethodConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.Property<int>("BuyerId");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "verified", "expected": "1", "actual": "1",
      "at": "src/Infrastructure/Persistence/PaymentMethodConfiguration.cs:11" }
  ]
}
```

## from-the-dependent

A database-first context configures each relationship from its dependent: `PaymentMethod`'s `HasOne(p => p.Buyer).WithMany(b => b.PaymentMethods)`. Its `HasOne` is the `Buyer` end, and the foreign key it names, an `int`, makes it required, `1`. The chain is the relationship's because its `WithMany` names the navigation and its `HasOne` navigation is a `Buyer`: another class's inverse may share the navigation's name.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/BuyerConfiguration.cs"], "kinds": ["relationship.multiplicity"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/PaymentMethod.cs
namespace Ordering.Domain;

public class PaymentMethod
{
    public int Id { get; private set; }

    public int BuyerId { get; private set; }

    public Buyer Buyer { get; private set; } = null!;
}
```

```csharp file src/Infrastructure/Persistence/PaymentMethodConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.HasOne(p => p.Buyer).WithMany(b => b.PaymentMethods).HasForeignKey(p => p.BuyerId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "verified", "expected": "1", "actual": "1",
      "at": "src/Domain/PaymentMethod.cs:7" }
  ]
}
```

## collection-by-convention-shadow-key

No configuration names the relationship, and the payment method has no `BuyerId`: the context maps both entities, and EF Core makes the relationship one-to-many by its convention, with a shadow foreign key that can be null. A payment method may have no buyer, `0..1`, where the model says `1`.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/BuyerConfiguration.cs"], "kinds": ["relationship.multiplicity"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderingContext(DbContextOptions<OrderingContext> options) : DbContext(options)
{
    public DbSet<Buyer> Buyers => Set<Buyer>();

    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "contradicted", "expected": "1", "actual": "0..1",
      "at": "src/Domain/Buyer.cs:9" }
  ]
}
```

## blocked-no-navigation

`Buyer` has no `PaymentMethods` property, so the navigation's `member.exists` is contradicted, and the relationship it would configure can't be read: the claim is blocked by it, one finding for the one missing property.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/BuyerConfiguration.cs"], "kinds": ["relationship.multiplicity", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "buyer-payment-methods", "outcome": "contradicted" },
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ]
}
```

## no-claim-unpersisted-owner

`Buyer` has no Table or View stereotype, so the model doesn't persist it, and no configuration or DbContext maps it: `Buyer.PaymentMethods` is a plain property, not a mapped relationship. The association states no multiplicity to check, so there is no claim and no finding, though `PaymentMethod` is a table (user decision, 2026-11-15). A Table class nothing maps is `entity.persisted`'s one finding, not one per relationship.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/BuyerConfiguration.cs"], "kinds": ["relationship.multiplicity"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "payment-method-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "buyer-payment-methods", "type": "Association",
          "source": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": false },
          "target": { "element": "payment-method", "name": "PaymentMethods", "multiplicity": "*", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "absent": true }
  ]
}
```

## verified-many-to-one

An order may have a buyer, and only `Order.Buyer` is navigable. The model's `Orders` end is `*`: a buyer has many orders. The configuration's `HasOne(o => o.Buyer).WithMany()` says the same.

```json case
{ "phase": 4, "kinds": ["relationship.multiplicity"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int", "nullable": true },
                "stereotypes": { "Foreign Key": { "Association": { "ref": "order-buyer" } } } }
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
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "0..1", "navigable": true } }
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
        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-buyer", "outcome": "verified", "expected": "*", "actual": "*",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## verified-many-to-one-by-convention

No configuration names the relationship; the context maps both entities, and EF Core makes a reference navigation with no inverse many-to-one, so a buyer has many orders, `*`. The navigation is where the code says it.

```json case
{ "phase": 4, "base": "verified-many-to-one", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["relationship.multiplicity"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderingContext(DbContextOptions<OrderingContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    public DbSet<Buyer> Buyers => Set<Buyer>();
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-buyer", "outcome": "verified", "expected": "*", "actual": "*",
      "at": "src/Domain/Order.cs:9" }
  ]
}
```

## java-verified-many-to-one

The same claim over JPA. The owning field `product` in `OrderLine.java` is `@ManyToOne`: many lines share one product, so the model's `OrderLines` end, `*`, is the code's (JAV-DOM-23). Whether a line must have a product is the `Product` end's, which `member.nullable` reads from `optional = false`.

```json case
{ "phase": 4, "kinds": ["relationship.multiplicity"], "traces": ["JAV-DOM-23"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "product", "type": "Class", "name": "Product",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "product-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-line-product", "type": "Association",
          "source": { "element": "order-line", "name": "OrderLines", "multiplicity": "*", "navigable": false },
          "target": { "element": "product", "name": "Product", "multiplicity": "1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/OrderLine.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import java.util.UUID;

@Entity
public class OrderLine {
    @Id
    private UUID id;

    @ManyToOne(optional = false)
    @JoinColumn(name = "product_id")
    private Product product;
}
```

```java file src/main/java/com/shop/domain/Product.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Product {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-line-product", "outcome": "verified", "expected": "*", "actual": "*",
      "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ]
}
```

## java-contradicted-annotation

The model makes each line's product its own (the `OrderLine` end is `1`), which JPA states with `@OneToOne` and a cascade, or `@Embedded`. The code's `@ManyToOne` says many lines share a product, so the annotation doesn't match the model (JAV-DOM-23).

```json case
{ "phase": 4, "base": "java-verified-many-to-one", "kinds": ["relationship.multiplicity"], "traces": ["JAV-DOM-23"], "requirements": ["R1"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "product", "type": "Class", "name": "Product",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "product-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-line-product", "type": "Association",
          "source": { "element": "order-line", "name": "OrderLine", "multiplicity": "1", "navigable": false },
          "target": { "element": "product", "name": "Product", "multiplicity": "1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-line-product", "outcome": "contradicted", "expected": "1", "actual": "*",
      "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ]
}
```

## java-verified-composition

`Order` owns its lines: the `Order` end is `1`. The owning field is a `List<OrderLine>` with `@OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)`: the cascade makes the order the lines' owner (JAV-DOM-23, JAV-DOM-24).

```json case
{ "phase": 4, "kinds": ["relationship.multiplicity"], "traces": ["JAV-DOM-23", "JAV-DOM-24"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-lines", "type": "Association",
          "source": { "element": "order", "name": "Order", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-line", "name": "Lines", "multiplicity": "*", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
    @JoinColumn(name = "order_id")
    private List<OrderLine> lines = new ArrayList<>();
}
```

```java file src/main/java/com/shop/domain/OrderLine.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class OrderLine {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-lines", "outcome": "verified", "expected": "1", "actual": "1",
      "at": "src/main/java/com/shop/domain/Order.java:17" }
  ]
}
```

## java-contradicted-not-composition

The same field with a plain `@OneToMany`: no cascade, no orphan removal and a join column that can be null, so a line may belong to no order, `0..1`. The model says composition, `1` (JAV-DOM-24).

```json case
{ "phase": 4, "base": "java-verified-composition", "kinds": ["relationship.multiplicity"], "traces": ["JAV-DOM-24"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @OneToMany
    @JoinColumn(name = "order_id")
    private List<OrderLine> lines = new ArrayList<>();
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-lines", "outcome": "contradicted", "expected": "1", "actual": "0..1",
      "at": "src/main/java/com/shop/domain/Order.java:16" }
  ],
  "findings": [
    { "rule": "claim/relationship.multiplicity", "severity": "error", "subject": "order-lines", "messageContains": "code `0..1`" }
  ]
}
```

## java-required-join-column

No cascade, but the join column can't be null: every line has an order, `1` (JAV-DOM-25's other end).

```json case
{ "phase": 4, "base": "java-verified-composition", "kinds": ["relationship.multiplicity"], "traces": ["JAV-DOM-25"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @OneToMany
    @JoinColumn(name = "order_id", nullable = false)
    private List<OrderLine> lines = new ArrayList<>();
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-lines", "outcome": "verified", "expected": "1", "actual": "1",
      "at": "src/main/java/com/shop/domain/Order.java:16" }
  ]
}
```
