# key

A persisted Domain class's primary key: the names of its attributes with the Primary Key stereotype, in model order, as a list (`OrderId, ProductId`). The code states it with `HasKey`, or, with no `HasKey`, by the `Id` property EF Core takes as the key, declared on the class or inherited from a base such as eShopCustom's SeedWork `Entity`. Over JPA it is the field marked `@Id`. `key` is `stated: always` and compared with the `list` comparer: the same items in the same order (R10).

## verified

eShopCustom's shape: `Order` derives from the SeedWork `Entity`, which declares `public virtual int Id`, and the configuration has no `HasKey`. The model's `Id` attribute isn't declared on `Order` itself, so it must be the inherited `Id`, and it is (ESC-DOM-36). That needs the `Entity` base to declare a non-private `Id`, which is ESC-DOM-08's key half.

```json case
{ "phase": 4, "kinds": ["key"], "traces": ["ESC-DOM-36", "ESC-DOM-08"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-order-date", "type": "Attribute", "name": "OrderDate", "typeRef": { "type": "datetime" } }
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

```csharp file src/Domain/SeedWork/Entity.cs
namespace Shop.Domain.SeedWork;

public abstract class Entity
{
    public virtual int Id { get; protected set; }
}
```

```csharp file src/Domain/Order.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain;

public class Order : Entity
{
    public DateTime OrderDate { get; private set; }
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
        builder.ToTable("orders");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order", "outcome": "verified", "expected": "Id", "actual": "Id",
      "at": "src/Domain/SeedWork/Entity.cs:5" }
  ]
}
```

## contradicted-inherited

The model names the key attribute `OrderId`, but `Order` doesn't declare it and has no `HasKey`: its key is the `Id` it inherits from `Entity`. ESC-DOM-36's rule, that a primary key the class doesn't declare must be the inherited `Id`, is broken, so the claim is contradicted with the code's `Id` and its line.

```json case
{ "phase": 4, "base": "verified", "kinds": ["key"], "traces": ["ESC-DOM-36"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "OrderId", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-order-date", "type": "Attribute", "name": "OrderDate", "typeRef": { "type": "datetime" } }
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
    { "kind": "key", "element": "order", "outcome": "contradicted", "expected": "OrderId", "actual": "Id",
      "at": "src/Domain/SeedWork/Entity.cs:5" }
  ],
  "findings": [
    { "rule": "claim/key", "severity": "error", "subject": "order", "messageContains": "Primary Key" }
  ]
}
```

## not-found-private-id

The SeedWork `Entity` declares its `Id` private, the drift ESC-DOM-08 guards against. EF Core doesn't take a private property as a key by convention, there is no `HasKey`, and an entity with no key isn't valid EF Core, so the code states no key at all: no reading, and no silence that means anything. The claim is can't verify, `notFound`, saying where it looked, rather than a guess.

```json case
{ "phase": 4, "base": "verified", "kinds": ["key"], "cases": ["engine/notFound"], "traces": ["ESC-DOM-08"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/SeedWork/Entity.cs
namespace Shop.Domain.SeedWork;

public abstract class Entity
{
    private int Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order", "outcome": "cantVerify", "reason": "notFound" }
  ],
  "findings": [
    { "rule": "claim/key", "severity": "error", "subject": "order" }
  ]
}
```

## verified-composite

`OrderItem`'s key is two attributes, and the configuration's `HasKey` names both, in the model's order.

```json case
{ "phase": 4, "kinds": ["key"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-item-order-id", "type": "Attribute", "name": "OrderId", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-item-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } }
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

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

public class OrderItem
{
    public Guid OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Units { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/OrderItemConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => new { i.OrderId, i.ProductId });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order-item", "outcome": "verified", "expected": "OrderId, ProductId", "actual": "OrderId, ProductId",
      "at": "src/Infrastructure/Persistence/OrderItemConfiguration.cs:11" }
  ]
}
```

## contradicted-order

The same two key columns in the other order. A composite key's order is its column order in the index, so the `list` comparer, which needs the same items in the same order, contradicts it (R10).

```json case
{ "phase": 4, "base": "verified-composite", "kinds": ["key"], "cases": ["engine/comparers"], "requirements": ["R1", "R10"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderItemConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => new { i.ProductId, i.OrderId });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order-item", "outcome": "contradicted", "expected": "OrderId, ProductId", "actual": "ProductId, OrderId",
      "at": "src/Infrastructure/Persistence/OrderItemConfiguration.cs:11" }
  ]
}
```

## java-verified

Over JPA the key is the field marked `@Id` (JAV-DOM-18's first half). The Java profile's naming maps the model's `Id` to the field `id`, so the reading compares as `Id`.

```json case
{ "phase": 4, "kinds": ["key"], "traces": ["JAV-DOM-18"], "requirements": ["R2", "R33", "R42"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-code", "type": "Attribute", "name": "Code", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
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

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    private String code;
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order", "outcome": "verified", "expected": "Id", "actual": "Id" }
  ]
}
```

## java-contradicted

`@Id` sits on `code`, not on the field the model marks Primary Key.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["key"], "traces": ["JAV-DOM-18"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Order {
    private UUID id;

    @Id
    private String code;
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order", "outcome": "contradicted", "expected": "Id", "actual": "Code" }
  ],
  "findings": [
    { "rule": "claim/key", "severity": "error", "subject": "order" }
  ]
}
```

## keyless-verified

A Table with no Primary Key attribute states that it has no key, `none`, as a database-first model of a keyless table or view does. The code says so with `HasNoKey()` in its inline configuration, the shape `Scaffold-DbContext` writes.

```json case
{ "phase": 4, "kinds": ["key"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "sales-summary", "type": "Class", "name": "SalesSummary",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "sales-summary-region", "type": "Attribute", "name": "Region", "typeRef": { "type": "string" } },
              { "id": "sales-summary-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } }
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

```csharp file src/Domain/SalesSummary.cs
namespace Shop.Domain;

public partial class SalesSummary
{
    public string Region { get; set; }

    public decimal Total { get; set; }
}
```

```csharp file src/Data/ShopContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Data;

public class ShopContext : DbContext
{
    public DbSet<SalesSummary> SalesSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesSummary>(entity =>
        {
            entity.HasNoKey();
            entity.ToTable("SalesSummary");
        });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "sales-summary", "outcome": "verified", "expected": "none", "actual": "none", "at": "src/Data/ShopContext.cs:14" }
  ]
}
```

## keyless-with-a-conditional-key

The context gives `SalesSummary` a shadow key only on the in-memory test provider, and declares it keyless otherwise. The model describes the database, keyless, and the code declares that too, so the claim is verified rather than read from the first key it meets.

```json case
{ "phase": 4, "base": "keyless-verified", "kinds": ["key"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Data/ShopContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Data;

public class ShopContext : DbContext
{
    public DbSet<SalesSummary> SalesSummaries { get; set; }

    public bool IsInMemory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesSummary>(entity =>
        {
            if (IsInMemory)
            {
                modelBuilder.Entity<SalesSummary>().Property<int>("Id");
                modelBuilder.Entity<SalesSummary>().HasKey("Id");
            }
            else
                entity.HasNoKey();

            entity.ToTable("SalesSummary");
        });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "sales-summary", "outcome": "verified", "expected": "none", "actual": "none", "at": "src/Data/ShopContext.cs:22" }
  ]
}
```

## keyless-contradicted

The model's `SalesSummary` still has no key, but the code gives it one, `Region`.

```json case
{ "phase": 4, "base": "keyless-verified", "kinds": ["key"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Data/ShopContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Data;

public class ShopContext : DbContext
{
    public DbSet<SalesSummary> SalesSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesSummary>(entity =>
        {
            entity.HasKey(e => e.Region);
            entity.ToTable("SalesSummary");
        });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "sales-summary", "outcome": "contradicted", "expected": "none", "actual": "Region", "at": "src/Data/ShopContext.cs:14" }
  ],
  "findings": [
    { "rule": "claim/key", "severity": "error", "subject": "sales-summary", "messageContains": "`SalesSummary` primary key: model `none`, code `Region`." }
  ]
}
```

## inherited-from-a-modelled-base

The model has the SeedWork base too: an abstract `Entity` with the `Id` Primary Key, and `Order` derives it through a Generalization, so `Order`'s key is the inherited `Id` and is verified against the code's. An abstract base with no Table or View isn't a table of its own, so `Entity` has no key claim.

```json case
{ "phase": 4, "base": "verified", "kinds": ["key"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "entity", "type": "Class", "name": "Entity", "abstract": true,
            "children": [
              { "id": "entity-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "order", "type": "Class", "name": "Order", "generalization": "entity",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-order-date", "type": "Attribute", "name": "OrderDate", "typeRef": { "type": "datetime" } }
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
    { "kind": "key", "element": "order", "outcome": "verified", "expected": "Id", "actual": "Id", "at": "src/Domain/SeedWork/Entity.cs:5" },
    { "kind": "key", "element": "entity", "absent": true }
  ],
  "findings": [
    { "rule": "claim/key", "count": 0 }
  ]
}
```

## expression-bodied-entity-lambda

A key configured in an expression-bodied `Entity<T>(x => x.HasKey(...))` lambda is read as one in a block-bodied lambda is: the lambda's body is the chain itself. Its columns are required, as EF Core maps every primary key column. From an adopted application's `DataContext` (`builder.Entity<UserActivity>(x => x.HasKey(ua => new { ua.AppUserId, ua.ActivityId }))`).

```json case
{ "phase": 4, "kinds": ["key", "member.nullable"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-line-order-id", "type": "Attribute", "name": "OrderId", "typeRef": { "type": "string" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-line-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "string" }, "stereotypes": { "Primary Key": {} } }
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

```csharp file src/Domain/OrderLine.cs
namespace Shop.Domain;

public class OrderLine
{
    public string OrderId { get; set; }
    public string ProductId { get; set; }
}
```

```csharp file src/Infrastructure/ShopContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Infrastructure;

public class ShopContext : DbContext
{
    public DbSet<OrderLine> OrderLines { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<OrderLine>(x => x.HasKey(l => new { l.OrderId, l.ProductId }));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order-line", "outcome": "verified", "expected": "OrderId, ProductId", "actual": "OrderId, ProductId", "at": "src/Infrastructure/ShopContext.cs:12" }
  ],
  "findings": [
    { "rule": "claim/key", "count": 0 },
    { "rule": "claim/member.nullable", "count": 0 }
  ]
}
```

## expression-bodied-entity-lambda-contradicted

The lambda's key is in the other order, which the model's key attributes contradict.

```json case
{ "phase": 4, "base": "expression-bodied-entity-lambda", "kinds": ["key"], "requirements": ["R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/ShopContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Infrastructure;

public class ShopContext : DbContext
{
    public DbSet<OrderLine> OrderLines { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<OrderLine>(x => x.HasKey(l => new { l.ProductId, l.OrderId }));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order-line", "outcome": "contradicted", "expected": "OrderId, ProductId", "actual": "ProductId, OrderId" }
  ],
  "findings": [
    { "rule": "claim/key", "severity": "error", "subject": "order-line" }
  ]
}
```
