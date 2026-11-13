# relationship.multiplicity

The multiplicities of a Domain association's two ends, as `<source>-><target>` (`1->*`, `*->0..1`), against the relationship the code configures. The claim is stated by the association, so its `element` is the association. `relationship.multiplicity` is `stated: always`: a reference navigation with no inverse collection states that many sources share one target. Over EF Core the find reads the fluent chain, and falls back to the navigation property when there is none (ESC-DOM-54); over JPA it reads the owning field's relationship annotation (JAV-DOM-23).

## verified

`Buyer` owns its payment methods, `1->*`. The configuration's `HasMany(...).WithOne().IsRequired()` says the same: a collection navigation (`*`), and a required principal (`1`).

```json case
{ "phase": 4, "kinds": ["relationship.multiplicity"], "traces": ["ESC-DOM-55"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "verified", "expected": "1->*", "actual": "1->*",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ]
}
```

## contradicted-collection-flag

The model says a buyer has at most one payment method (`1->0..1`), but the code navigates a collection. The navigation's collection flag decides the target end, so the claim is contradicted, and the finding shows both values (R14).

This traces ESC-DOM-55 in part. The multiplicity value carries no type, so the check's other half, a navigation typed to a class other than the association's target, is left out here.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.multiplicity"], "traces": ["ESC-DOM-55"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
          "target": { "element": "payment-method", "name": "PaymentMethods", "multiplicity": "0..1", "navigable": true } }
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
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "contradicted", "expected": "1->0..1", "actual": "1->*",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/relationship.multiplicity", "severity": "error", "subject": "buyer-payment-methods", "messageContains": "code `1->*`" }
  ]
}
```

## cant-verify-no-navigation

The `PaymentMethods` end is navigable, but `Buyer` has no `PaymentMethods` property and no configuration names one. The find falls back from the chain to the property and finds neither. Multiplicity isn't an existence kind and the stock rule has no `absent`, so the claim is can't verify, `notFound`, naming what was searched, rather than a guess (ESC-DOM-54; principle 3).

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/BuyerConfiguration.cs"], "kinds": ["relationship.multiplicity"], "cases": ["engine/notFound"], "traces": ["ESC-DOM-54"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "relationship.multiplicity", "element": "buyer-payment-methods", "outcome": "cantVerify", "reason": "notFound" }
  ],
  "findings": [
    { "rule": "claim/relationship.multiplicity", "severity": "error", "subject": "buyer-payment-methods" }
  ]
}
```

## verified-optional-reference

An order may have no buyer yet, `*->0..1`. The configuration's `HasOne(o => o.Buyer).WithMany()` gives the many side, and the reference end's optionality comes from the foreign key it names: `BuyerId` is `int?`, so the end is `0..1` (ESC-DOM-56). The rule combines the chain and the key property, and names the key property as deciding, since its nullability decides the end: `at` is `BuyerId`'s line.

```json case
{ "phase": 4, "kinds": ["relationship.multiplicity"], "traces": ["ESC-DOM-56"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "relationship.multiplicity", "element": "order-buyer", "outcome": "verified", "expected": "*->0..1", "actual": "*->0..1",
      "at": "src/Domain/Order.cs:7" }
  ]
}
```

## contradicted-required-end

The model says every order has a buyer (`*->1`), but the foreign key `BuyerId` is `int?`, so the code lets an order have none. The key's nullability decides, so the claim is contradicted (ESC-DOM-56).

```json case
{ "phase": 4, "base": "verified-optional-reference", "kinds": ["relationship.multiplicity"], "traces": ["ESC-DOM-56"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": true } }
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
    { "kind": "relationship.multiplicity", "element": "order-buyer", "outcome": "contradicted", "expected": "*->1", "actual": "*->0..1",
      "at": "src/Domain/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/relationship.multiplicity", "severity": "error", "subject": "order-buyer", "messageContains": "code `*->0..1`" }
  ]
}
```

## verified-from-navigation-property

No configuration names the relationship and `Order` has no foreign key property, only the navigation. The find falls back to the `Buyer` property (ESC-DOM-54), and with no foreign key the end's optionality is the property's own: `Buyer?` in a nullable context is `0..1` (ESC-DOM-56). `Buyer` has no collection of orders, so nothing states a source end other than many.

```json case
{ "phase": 4, "base": "verified-optional-reference", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["relationship.multiplicity"], "traces": ["ESC-DOM-54", "ESC-DOM-56"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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

```xml file src/Domain/Ordering.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public Buyer? Buyer { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-buyer", "outcome": "verified", "expected": "*->0..1", "actual": "*->0..1",
      "at": "src/Domain/Order.cs:7" }
  ]
}
```

## java-verified-many-to-one

The same claim over JPA. The owning field `product` in `OrderLine.java` is a single `Product` with `@ManyToOne(optional = false)`: many lines share one product, and the product is required, `*->1` (JAV-DOM-23, JAV-DOM-25).

```json case
{ "phase": 4, "kinds": ["relationship.multiplicity"], "traces": ["JAV-DOM-23", "JAV-DOM-25"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
    { "kind": "relationship.multiplicity", "element": "order-line-product", "outcome": "verified", "expected": "*->1", "actual": "*->1",
      "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ]
}
```

## java-contradicted-optional

The field is `@ManyToOne` with no `optional = false`, no `@JoinColumn(nullable = false)` and no `@NotNull`, so the code lets a line have no product: `*->0..1`. The model's `Product` end is `1`, so the claim is contradicted (JAV-DOM-25).

```json case
{ "phase": 4, "base": "java-verified-many-to-one", "kinds": ["relationship.multiplicity"], "traces": ["JAV-DOM-25"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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

    @ManyToOne
    @JoinColumn(name = "product_id")
    private Product product;
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.multiplicity", "element": "order-line-product", "outcome": "contradicted", "expected": "*->1", "actual": "*->0..1",
      "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ],
  "findings": [
    { "rule": "claim/relationship.multiplicity", "severity": "error", "subject": "order-line-product", "messageContains": "code `*->0..1`" }
  ]
}
```

## java-contradicted-annotation

The model makes each line's product its own (`1->1`), which JPA states with `@OneToOne` or `@Embedded`. The code's `@ManyToOne` says many lines share a product, so the annotation doesn't match the multiplicities (JAV-DOM-23).

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
    { "kind": "relationship.multiplicity", "element": "order-line-product", "outcome": "contradicted", "expected": "1->1", "actual": "*->1",
      "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ]
}
```

## java-verified-composition

`Order` owns its lines, `1->*`. The owning field is a `List<OrderLine>` with `@OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)`: the cascade makes the order the lines' owner, so the source end is `1` (JAV-DOM-23, JAV-DOM-24).

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
    @JoinColumn(name = "order_id", nullable = false)
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
    { "kind": "relationship.multiplicity", "element": "order-lines", "outcome": "verified", "expected": "1->*", "actual": "1->*",
      "at": "src/main/java/com/shop/domain/Order.java:17" }
  ]
}
```

## java-contradicted-not-composition

The same field with a plain `@OneToMany`: no cascade, no orphan removal, so nothing makes the order the lines' owner and the annotation reads as `0..1->*`. The model says composition, `1->*`, so the claim is contradicted (JAV-DOM-24).

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
    { "kind": "relationship.multiplicity", "element": "order-lines", "outcome": "contradicted", "expected": "1->*", "actual": "0..1->*",
      "at": "src/main/java/com/shop/domain/Order.java:16" }
  ],
  "findings": [
    { "rule": "claim/relationship.multiplicity", "severity": "error", "subject": "order-lines", "messageContains": "code `0..1->*`" }
  ]
}
```
