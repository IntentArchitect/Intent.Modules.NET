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
