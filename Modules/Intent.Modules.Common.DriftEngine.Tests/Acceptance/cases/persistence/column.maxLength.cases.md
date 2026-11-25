# column.maxLength

The maximum length of a column: the Text Constraints stereotype's MaxLength on a `string` attribute of a persisted class, or, in eShopCustom, the Enum Storage stereotype's Max Length on an enum attribute stored as text. EF Core states it with `HasMaxLength`, `[MaxLength]` or `[StringLength]`; JPA with `@Column(length = …)`. `column.maxLength` is `stated: whenSet` and compared as a number: an unbounded `nvarchar(max)` or JPA's default of 255 is never read in its place (decision 4).

## verified

`Order.Description` is modelled with a MaxLength of 200, and the configuration says `HasMaxLength(200)` (ESC-DOM-49).

```json case
{ "phase": 4, "kinds": ["column.maxLength"], "traces": ["ESC-DOM-49"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" },
                "stereotypes": { "Text Constraints": { "MaxLength": 200 } } }
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

    public string Description { get; private set; }
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
        builder.Property(o => o.Description).HasMaxLength(200);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## verified-max-length-attribute

The same length stated with the `[MaxLength]` data annotation, one of the three forms ESC-DOM-49 accepts.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.maxLength"], "traces": ["ESC-DOM-49"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    [MaxLength(200)]
    public string Description { get; private set; }
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
        builder.HasKey(o => o.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Domain/Order.cs:9" }
  ]
}
```

## contradicted-string-length

`[StringLength(100)]`, the third form, states a different length. The finding names the attribute, both values and the line, and says the Text Constraints stereotype's MaxLength states the model's side (R11, R14).

```json case
{ "phase": 4, "base": "verified-max-length-attribute", "kinds": ["column.maxLength"], "traces": ["ESC-DOM-49"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    [StringLength(100)]
    public string Description { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "contradicted", "expected": "200", "actual": "100",
      "at": "src/Domain/Order.cs:9" }
  ],
  "findings": [
    { "rule": "claim/column.maxLength", "severity": "error", "subject": "order-description", "messageContains": "MaxLength" }
  ]
}
```

## contradicted-not-stated

The model says 200, and the code states no maximum length in any of the three forms, so the column is as long as the provider makes it. The modelled length is contradicted, "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.maxLength"], "traces": ["ESC-DOM-49"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.Description).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "contradicted", "expected": "200", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.maxLength", "severity": "error", "subject": "order-description", "messageContains": "doesn't state" }
  ]
}
```

## missing-property-not-configured

`Description` isn't in the code at all, and the configuration states no length for it either. Both are reported in the one run, each a fix of its own: the property is missing from `Order`, and `OrderConfiguration`, which EF Core reads by the property's name, has no `HasMaxLength(200)`. The length is against the configuration class, since there's no property to point at (user decision, 2026-11-24: separate findings, so a missing property and its configuration take one trip).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.maxLength", "member.exists"], "requirements": ["R4", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.ToTable("Orders");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-description", "outcome": "contradicted" },
    { "kind": "column.maxLength", "element": "order-description", "outcome": "contradicted", "expected": "200", "actual": null,
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-description", "messageContains": "isn't in the code" },
    { "rule": "claim/column.maxLength", "severity": "error", "subject": "order-description", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The attribute has no MaxLength, and the code says `HasMaxLength(200)`. An unset `whenSet` value is no claim, so nothing is checked or reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.maxLength"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
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
    { "kind": "column.maxLength", "element": "order-description", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.maxLength", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## verified-constant

The configuration takes the length from a constant in another class. `HasMaxLength(OrderConstraints.DescriptionMaxLength)` states the constant's value, `200`, which the code declares, so the claim is verified as a literal would be.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.maxLength"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.Description).HasMaxLength(OrderConstraints.DescriptionMaxLength);
    }
}
```

```csharp file src/Domain/OrderConstraints.cs
namespace Shop.Domain;

public static class OrderConstraints
{
    public const int DescriptionMaxLength = 200;
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## unreadable

The configuration takes the length from options bound at run time. `HasMaxLength(limits.Value.DescriptionMaxLength)` states a length, but not as a literal or a constant the code declares, so the claim is can't verify, `unreadable`, naming the line (R5), the outcome claims-and-findings.md's example message describes.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.maxLength"], "cases": ["engine/unreadable"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderConfiguration(Microsoft.Extensions.Options.IOptions<OrderLimits> limits) : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Description).HasMaxLength(limits.Value.DescriptionMaxLength);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.maxLength", "severity": "error", "subject": "order-description", "messageContains": "can't read" }
  ]
}
```

## verified-by-ef6-properties-convention

No configuration of `Order` states a length; the EF6 context's `modelBuilder.Properties<string>().Configure(x => x.HasMaxLength(200))` states it for every string property it maps.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["column.maxLength"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using System.Data.Entity;
using Shop.Domain;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Properties<string>().Configure(x => x.HasMaxLength(200));
    }

    public virtual DbSet<Order> Orders { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Infrastructure/OrderingContext.cs:10" }
  ]
}
```

## verified-by-ef-core-configure-conventions

EF Core's form of the same convention: `ConfigureConventions` gives every string property the length with `HaveMaxLength(200)`.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["column.maxLength"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().AreUnicode(false).HaveMaxLength(200);
    }

    public DbSet<Order> Orders => Set<Order>();
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Infrastructure/OrderingContext.cs:10" }
  ]
}
```

## property-overrides-the-convention

The context gives every string 50, and `Description`'s own `[MaxLength(200)]` overrides it, as a property's configuration does in EF6 and EF Core, so the column is 200 long.

```json case
{ "phase": 4, "base": "verified-by-ef6-properties-convention", "kinds": ["column.maxLength"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    [MaxLength(200)]
    public string Description { get; private set; }
}
```

```csharp file src/Infrastructure/OrderingContext.cs
using System.Data.Entity;
using Shop.Domain;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Properties<string>().Configure(x => x.HasMaxLength(50));
    }

    public virtual DbSet<Order> Orders { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Domain/Order.cs:9" }
  ]
}
```

## unbounded-attribute-overrides-the-convention

`Description`'s `[MaxLength]` with no length makes its column unbounded, which the context's convention for strings doesn't change, so the model's 200 is contradicted, not stated.

```json case
{ "phase": 4, "base": "verified-by-ef6-properties-convention", "kinds": ["column.maxLength"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    [MaxLength]
    public string Description { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "contradicted", "expected": "200", "actual": null, "fromSilence": false }
  ]
}
```

## convention-contradicted

The context's convention gives every string 50, and nothing on `Description` overrides it, so the model's 200 is contradicted, at the convention.

```json case
{ "phase": 4, "base": "property-overrides-the-convention", "kinds": ["column.maxLength"], "requirements": ["R1"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public string Description { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "contradicted", "expected": "200", "actual": "50",
      "at": "src/Infrastructure/OrderingContext.cs:10" }
  ]
}
```

## enum-verified

eShopCustom stores `Order.OrderStatus` as text, with the Enum Storage stereotype from its own module: As `String`, Max Length 30. ESC-DOM-49 checks that length against `HasMaxLength(30)` too. No stock pack knows the module's stereotype, so a house pack gives the vocabulary: a `column.maxLength` entry for attributes with Enum Storage, valued from its Max Length. The stock EF Core rule reads the length as it does for a string.

```json case
{ "phase": 4, "kinds": ["column.maxLength"], "traces": ["ESC-DOM-49"], "requirements": ["R2", "R29"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
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
          "kind": "column.enumStorage",
          "for": { "element": "Attribute", "where": "parent.hasStereotype('Table') && hasStereotype('Enum Storage')" },
          "value": "stereotype('Enum Storage').As",
          "bindings": { "Entity": "parent.name", "Attribute": "name" },
          "own": ["Attribute"],
          "dependsOn": "member.exists",
          "source": "the Enum Storage stereotype's As"
        },
        {
          "kind": "column.maxLength",
          "for": { "element": "Attribute", "where": "parent.hasStereotype('Table') && hasStereotype('Enum Storage')" },
          "value": "stereotype('Enum Storage')['Max Length']",
          "bindings": { "Entity": "parent.name", "Attribute": "name" },
          "own": ["Attribute"],
          "dependsOn": "member.exists",
          "source": "the Enum Storage stereotype's Max Length"
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
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-submitted", "type": "Enum-Literal", "name": "Submitted", "value": 1 },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid", "value": 2 },
              { "id": "order-status-shipped", "type": "Enum-Literal", "name": "Shipped", "value": 3 }
            ] },
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-order-status", "type": "Attribute", "name": "OrderStatus", "typeRef": { "element": "order-status" },
                "stereotypes": { "Enum Storage": { "As": "String", "Max Length": 30 } } }
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

```csharp file src/Domain/OrderStatus.cs
namespace Shop.Domain;

public enum OrderStatus
{
    Submitted = 1,
    Paid = 2,
    Shipped = 3
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public OrderStatus OrderStatus { get; private set; }
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
        builder.Property(o => o.OrderStatus).HasConversion<string>().HasMaxLength(30);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-order-status", "outcome": "verified", "expected": "30", "actual": "30",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## java-verified

Over JPA the length is `@Column(length = …)` (JAV-DOM-16).

```json case
{ "phase": 4, "kinds": ["column.maxLength"], "traces": ["JAV-DOM-16"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" },
                "stereotypes": { "Text Constraints": { "MaxLength": 200 } } }
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

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @Column(length = 200)
    private String description;
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/main/java/com/shop/domain/Order.java:13" }
  ]
}
```

## java-contradicted

`@Column(length = 100)` states a different length.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["column.maxLength"], "traces": ["JAV-DOM-16"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @Column(length = 100)
    private String description;
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "contradicted", "expected": "200", "actual": "100",
      "at": "src/main/java/com/shop/domain/Order.java:13" }
  ]
}
```

## java-contradicted-not-stated

The model says 255, and the field's `@Column` has no `length`. JPA's default length is 255, but a default isn't the code stating it, so the modelled length is contradicted, "not stated" (decision 4). The fix is to clear the MaxLength, or to write `length = 255`.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["column.maxLength"], "traces": ["JAV-DOM-16"], "requirements": ["R4", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" },
                "stereotypes": { "Text Constraints": { "MaxLength": 255 } } }
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

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @Column(nullable = false)
    private String description;
}
```

```json expect
{
  "claims": [
    { "kind": "column.maxLength", "element": "order-description", "outcome": "contradicted", "expected": "255", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.maxLength", "severity": "error", "subject": "order-description", "messageContains": "doesn't state" }
  ]
}
```
