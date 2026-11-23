# entity.persisted

Whether a persisted Domain class (one with the Table or View stereotype) really reaches the database: its EF Core configuration is applied in the DbContext's `OnModelCreating`, by `ApplyConfiguration(new XConfiguration())` or by `ApplyConfigurationsFromAssembly` over the assembly that holds it. eShopCustom checked this as `persistence.applied` (ESC-DOM-10). `entity.persisted` is `stated: always`: a configuration nothing applies is a statement that the class isn't mapped, so a `false` reading from silence is honest (principle 3).

## verified

`OrderingContext` applies `OrderConfiguration` explicitly.

```json case
{ "phase": 4, "kinds": ["entity.persisted"], "traces": ["ESC-DOM-10"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.HasKey(x => x.Id);
    }
}
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/OrderingContext.cs:12" }
  ]
}
```

## verified-from-assembly

The context applies every configuration in its own assembly with `ApplyConfigurationsFromAssembly`, the other form ESC-DOM-10 accepts. `OrderConfiguration` is in the same project as the context, so the call applies it.

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted"], "traces": ["ESC-DOM-10"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```xml file src/Infrastructure/Shop.Infrastructure.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingContext).Assembly);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/OrderingContext.cs:11" }
  ]
}
```

## contradicted-not-applied

The context applies another class's configuration and never `OrderConfiguration`, and nothing else maps `Order`. The configuration is dead code and `Order` isn't in EF Core's model. The code's silence states `false`, so the claim is contradicted from silence (R3), the drift eShopCustom's `persistence.applied` caught.

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted"], "cases": ["engine/silence"], "traces": ["ESC-DOM-10"], "requirements": ["R1", "R3", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BuyerConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "severity": "error", "subject": "order" }
  ]
}
```

## reader-failed

The call that applies `OrderConfiguration` is missing its closing parenthesis. The C# reader still reads the file, but the node the rule would read lies in the error's span and is `incomplete`, so the claim is can't verify, `readerFailed`, with the reader's diagnostic, rather than a value that may be wrong (R40, code-reader.md).

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted"], "cases": ["engine/readerFailed"], "requirements": ["R5", "R40"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "cantVerify", "reason": "readerFailed" }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "severity": "error", "subject": "order" }
  ]
}
```

## one-finding-for-an-unmapped-table

Nothing maps `Order`, and its configuration also names its column differently from the model's Column Name. A table nothing maps is one finding (user decision, 2026-11-15: "flag it once"): its table name, table kind, key and columns wait on `entity.persisted` (`waitsOn`), so they are blocked, with no finding of their own, rather than a finding per column for a configuration that never runs. The six blocked are the table name, table kind and key, the column name, and each attribute's `column.mapped`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted", "table.name", "column.name"], "cases": ["engine/blocked"], "traces": ["ESC-DOM-10"], "requirements": ["R1", "R3", "R5"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": { "Name": "Orders" } },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-payment-id", "type": "Attribute", "name": "PaymentId", "typeRef": { "type": "int" },
                "stereotypes": { "Column": { "Name": "PaymentMethodId" } } }
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

    public int PaymentId { get; private set; }
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
        builder.HasKey(x => x.Id);
        builder.Property(o => o.PaymentId).HasColumnName("PaymentId");
    }
}
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BuyerConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true },
    { "kind": "table.name", "element": "order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "entity.persisted" },
    { "kind": "table.kind", "element": "order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "entity.persisted" },
    { "kind": "key", "element": "order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "entity.persisted" },
    { "kind": "column.name", "element": "order-payment-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "entity.persisted" }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "severity": "error", "subject": "order" },
    { "rule": "claim/entity.persisted", "subject": "order", "messageContains": "can't be checked", "count": 0 }
  ]
}
```

## verified-by-dbset

The context maps `Order` with a `DbSet`, and applies no configuration: EF Core maps the class by its conventions.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["entity.persisted"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/OrderingContext.cs:10" }
  ]
}
```

## verified-by-identity-base

ASP.NET Core Identity's `IdentityDbContext<ApplicationUser>` declares the `DbSet` of its user itself, in a package, so `ApplicationDbContext` maps `ApplicationUser` with no `DbSet` or configuration of its own. `ApplicationUser : IdentityUser` inherits `Id`, a `string`, from that package's `IdentityUser<string>`, so the modelled `Id` is a member of it and EF Core's conventional key, and `CardNumber` is its own. The adapted code is eShop's `Identity.API`.

```json case
{ "phase": 4, "kinds": ["entity.persisted", "member.exists", "member.type", "key"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Identity.API",
        "elements": [
          { "id": "user", "type": "Class", "name": "ApplicationUser",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "user-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "string" }, "stereotypes": { "Primary Key": {} } },
              { "id": "user-card-number", "type": "Attribute", "name": "CardNumber", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Identity", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Identity.API/Models/ApplicationUser.cs
namespace Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    public string CardNumber { get; set; }
}
```

```csharp file src/Identity.API/Data/ApplicationDbContext.cs
using Identity.API.Models;

namespace Identity.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "user", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Identity.API/Data/ApplicationDbContext.cs:5" },
    { "kind": "member.exists", "element": "user-id", "outcome": "verified", "at": "src/Identity.API/Models/ApplicationUser.cs:3" },
    { "kind": "member.type", "element": "user-id", "outcome": "verified" },
    { "kind": "member.exists", "element": "user-card-number", "outcome": "verified" },
    { "kind": "key", "element": "user", "outcome": "verified", "actual": "Id" }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "count": 0 },
    { "rule": "claim/member.exists", "count": 0 }
  ]
}
```

## verified-by-navigation

No `DbSet` or configuration names `OrderItem`, but `Order`, which the context maps, navigates to it: EF Core maps an entity a mapped one navigates to, by convention. The code doesn't state it; the framework's default does.

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted"], "requirements": ["R2", "R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
            ] },
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
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

    public List<OrderItem> Items { get; private set; } = new();
}
```

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

public class OrderItem
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "verified", "actual": "true", "at": "src/Infrastructure/OrderingContext.cs:12" },
    { "kind": "entity.persisted", "element": "order-item", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Domain/Order.cs:7" }
  ]
}
```

## no-dbcontext

No DbContext is in scope, so what EF Core maps can't be read: the claim is can't verify, `notFound`. That is no evidence the class isn't mapped, so the claims waiting on it are checked on their own: its key is verified from its configuration.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/OrderingContext.cs"], "kinds": ["entity.persisted", "key"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "cantVerify", "reason": "notFound" },
    { "kind": "key", "element": "order", "outcome": "verified" }
  ]
}
```

## java-verified-entity

Over JPA, `@Entity` maps the class.

```json case
{ "phase": 4, "kinds": ["entity.persisted"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": { "Name": "orders" } },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "customer", "type": "Class", "name": "Customer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "customer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
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
import jakarta.persistence.Table;
import java.util.UUID;

@Entity
@Table(name = "orders")
public class Order {
    @Id
    private UUID id;
}
```

```java file src/main/java/com/shop/domain/Customer.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Customer {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/main/java/com/shop/domain/Order.java:8" }
  ]
}
```

## java-contradicted-not-an-entity

`Order` has no `@Entity`, so JPA doesn't map it, though `Customer` shows JPA is in the code. The claim is contradicted from silence, and its table name waits on it: one finding.

```json case
{ "phase": 4, "base": "java-verified-entity", "kinds": ["entity.persisted", "table.name"], "cases": ["engine/blocked"], "requirements": ["R1", "R3", "R5"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import java.util.UUID;

public class Order {
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true },
    { "kind": "table.name", "element": "order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "entity.persisted" }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "severity": "error", "subject": "order" }
  ]
}
```
