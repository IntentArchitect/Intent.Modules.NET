# relationship.foreignKey

The attributes whose Foreign Key stereotype's Association property refers to an association (an element-valued property, `{ "ref": … }` in the model block), against the property the code's `HasForeignKey` names for that relationship. The claim is stated by the association, so its `element` is the association. `relationship.foreignKey` is `stated: whenSet`: an association no attribute's Foreign Key names gives no claim, and a modelled key the code doesn't name is contradicted, never filled in from EF Core's `<Navigation>Id` convention (decision 4).

## verified

`Order.BuyerId` carries the Foreign Key stereotype for the `Order` → `Buyer` association, and the configuration's `HasForeignKey(o => o.BuyerId)` names the same property (ESC-DOM-57).

```json case
{ "phase": 4, "kinds": ["relationship.foreignKey"], "traces": ["ESC-DOM-57"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "relationship.foreignKey", "element": "order-buyer", "outcome": "verified", "expected": "BuyerId", "actual": "BuyerId",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The model puts the relationship's Foreign Key on `PaymentId`, but `HasForeignKey` names `BuyerId`, which carries none. The attribute the code names must be the one carrying Foreign Key, so the claim is contradicted, and the finding shows both (ESC-DOM-57).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.foreignKey"], "traces": ["ESC-DOM-57"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int", "nullable": true } },
              { "id": "order-payment-id", "type": "Attribute", "name": "PaymentId", "typeRef": { "type": "int", "nullable": true },
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

    public int? PaymentId { get; private set; }

    public Buyer? Buyer { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "outcome": "contradicted", "expected": "PaymentId", "actual": "BuyerId",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/relationship.foreignKey", "severity": "error", "subject": "order-buyer", "messageContains": "code `BuyerId`" }
  ]
}
```

## verified-by-convention

The configuration has no `HasForeignKey`, and EF Core's convention picks `BuyerId` from the navigation's name, the property the model marks. The Domain designer marks every relationship's foreign key, so it can't leave a conventional one unstated as a `whenSet` value elsewhere can: the code's key is the one EF Core discovers, read from the convention as `key`'s `Id` is (ESC-DOM-57).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.foreignKey"], "traces": ["ESC-DOM-57"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.HasOne(o => o.Buyer).WithMany();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "outcome": "verified", "expected": "BuyerId", "actual": "BuyerId", "fromSilence": true }
  ]
}
```

## contradicted-not-stated

The model marks `PurchaserId` as the foreign key, but the configuration has no `HasForeignKey`, and EF Core's convention looks for `BuyerId`, which `Order` doesn't have: it would add a shadow `BuyerId`, not use `PurchaserId`. The code doesn't state the modelled key, so it is contradicted, with the code side "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.foreignKey"], "traces": ["ESC-DOM-57"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-purchaser-id", "type": "Attribute", "name": "PurchaserId", "typeRef": { "type": "int", "nullable": true },
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
    public int? PurchaserId { get; private set; }
    public Buyer? Buyer { get; private set; }
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
        builder.HasOne(o => o.Buyer).WithMany();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "outcome": "contradicted", "expected": "PurchaserId", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/relationship.foreignKey", "severity": "error", "subject": "order-buyer", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-without-foreign-key

No attribute carries Foreign Key for the association, though the code names `BuyerId`. An empty value is no claim (decision 10), so nothing is checked and nothing is reported: code the model doesn't mention isn't drift (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.foreignKey"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int", "nullable": true } }
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

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "absent": true }
  ],
  "findings": [
    { "rule": "claim/relationship.foreignKey", "count": 0 }
  ]
}
```

## cant-verify-ambiguous

The relationship is configured twice: in `OrderConfiguration` and again in the context's `OnModelCreating`. Both name `BuyerId`, but the find ends at two places where one is expected, so the claim is can't verify, `ambiguous`, and the finding names every candidate rather than picking one.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.foreignKey"], "cases": ["engine/ambiguous"], "traces": ["ESC-DOM-57"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderingContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingContext).Assembly);

        modelBuilder.Entity<Order>().HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/relationship.foreignKey", "severity": "error", "subject": "order-buyer", "messageContains": "src/Infrastructure/Persistence/OrderingContext.cs:14" }
  ]
}
```

## code-only-key-reported-when-opted-in

The model as `not-claimed-without-foreign-key` has it, no attribute with Foreign Key, and the code's `HasForeignKey` names `BuyerId`. Still no claim; with `optin/unmodelled.value` on, the key the code states and the model leaves unset is reported where the code states it, as Common.Drift's empty foreign key fact caught it.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.foreignKey"], "cases": ["optin/unmodelled.value"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int", "nullable": true } }
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
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "absent": true }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Infrastructure/Persistence/OrderConfiguration.cs:11",
      "messageContains": "`Order.Buyer` foreign key: the code states `BuyerId`, which the model leaves unset" },
    { "rule": "claim/relationship.foreignKey", "count": 0 }
  ]
}
```

## jpa-join-column

JPA states a foreign key as the dependent's fields mapped to the column its owning navigation's `@JoinColumn` names. `Order.buyer` is a read-only `@ManyToOne` on `buyer_id`, and `buyerId` is mapped to that column by Spring Boot's snake-case naming, so the key is `BuyerId`. `Order.customer` names its column `customer_ref`, and `customerRef` is mapped to it by `@Column(name)`.

```json case
{ "phase": 4, "kinds": ["relationship.foreignKey"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "guid" },
                "stereotypes": { "Foreign Key": { "Association": { "ref": "order-buyer" } } } },
              { "id": "order-customer-ref", "type": "Attribute", "name": "CustomerRef", "typeRef": { "type": "guid" },
                "stereotypes": { "Foreign Key": { "Association": { "ref": "order-customer" } } } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": true } },
        { "id": "order-customer", "type": "Association",
          "source": { "element": "order", "name": "CustomerOrders", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Customer", "multiplicity": "1", "navigable": true } }
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

import jakarta.persistence.*;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    private UUID buyerId;

    @ManyToOne
    @JoinColumn(name = "buyer_id", insertable = false, updatable = false)
    private Buyer buyer;

    @Column(name = "customer_ref")
    private UUID customerRef;

    @ManyToOne
    @JoinColumn(name = "customer_ref", insertable = false, updatable = false)
    private Buyer customer;
}
```

```java file src/main/java/com/shop/domain/Buyer.java
package com.shop.domain;

import jakarta.persistence.*;
import java.util.UUID;

@Entity
public class Buyer {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "outcome": "verified", "expected": "BuyerId", "actual": "BuyerId", "at": "src/main/java/com/shop/domain/Order.java:14" },
    { "kind": "relationship.foreignKey", "element": "order-customer", "outcome": "verified", "expected": "CustomerRef", "actual": "CustomerRef", "at": "src/main/java/com/shop/domain/Order.java:21" }
  ]
}
```

## jpa-join-column-contradicted

A planted drift: `Order.buyer`'s join column is now `buyer_ref`, which no field of `Order` is mapped to. The foreign key is contradicted, naming the column.

```json case
{ "phase": 4, "base": "jpa-join-column", "kinds": ["relationship.foreignKey"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.*;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    private UUID buyerId;

    @ManyToOne
    @JoinColumn(name = "buyer_ref", insertable = false, updatable = false)
    private Buyer buyer;

    @Column(name = "customer_ref")
    private UUID customerRef;

    @ManyToOne
    @JoinColumn(name = "customer_ref", insertable = false, updatable = false)
    private Buyer customer;
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.foreignKey", "element": "order-buyer", "outcome": "contradicted", "expected": "BuyerId", "actual": "none" },
    { "kind": "relationship.foreignKey", "element": "order-customer", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/relationship.foreignKey", "severity": "error", "subject": "order-buyer", "messageContains": "No field of `Order` is mapped to its join column `buyer_ref`" }
  ]
}
```
