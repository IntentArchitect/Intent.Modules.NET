# column.scale

The Decimal Constraints stereotype's Scale on a `decimal` attribute of a persisted class, against the scale the code states: the second argument of EF Core's `HasPrecision(p, s)`, or JPA's `@Column(scale = …)`. `column.scale` is `stated: whenSet` and compared as a number: a provider's default scale is never read in its place (decision 4).

## verified

`OrderItem.UnitPrice` is modelled with a scale of 2, and the configuration says `HasPrecision(18, 2)`.

```json case
{ "phase": 4, "kinds": ["column.scale"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-item-unit-price", "type": "Attribute", "name": "UnitPrice", "typeRef": { "type": "decimal" },
                "stereotypes": { "Decimal Constraints": { "Precision": 18, "Scale": 2 } } }
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
    public int Id { get; private set; }

    public decimal UnitPrice { get; private set; }
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
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.scale", "element": "order-item-unit-price", "outcome": "verified", "expected": "2", "actual": "2",
      "at": "src/Infrastructure/Persistence/OrderItemConfiguration.cs:11" }
  ]
}
```

## contradicted-not-stated

`HasPrecision(18)` states the precision and no scale, so the provider's default scale applies. The modelled scale is contradicted, "not stated" (decision 4), while the precision claim beside it is verified.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.scale", "column.precision"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(i => i.UnitPrice).HasPrecision(18);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.scale", "element": "order-item-unit-price", "outcome": "contradicted", "expected": "2", "actual": null, "fromSilence": false },
    { "kind": "column.precision", "element": "order-item-unit-price", "outcome": "verified", "expected": "18", "actual": "18",
      "at": "src/Infrastructure/Persistence/OrderItemConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.scale", "severity": "error", "subject": "order-item-unit-price", "messageContains": "doesn't state" },
    { "rule": "claim/column.precision", "count": 0 }
  ]
}
```

## not-claimed-when-unset

The Decimal Constraints set only the Precision, and the code says `HasPrecision(18, 2)`. The unset Scale is no claim, so nothing is checked or reported about it (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.scale"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-item-unit-price", "type": "Attribute", "name": "UnitPrice", "typeRef": { "type": "decimal" },
                "stereotypes": { "Decimal Constraints": { "Precision": 18 } } }
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
    { "kind": "column.scale", "element": "order-item-unit-price", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.scale", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## unreadable

The configuration takes the scale from its constructor, so each deployment's currency rules can set it. `HasPrecision(18, _moneyScale)` states a scale the rule can't read without running the code: can't verify, `unreadable`, naming the line (R5).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.scale"], "cases": ["engine/unreadable"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderItemConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    private readonly int _moneyScale;

    public OrderItemConfiguration(int moneyScale)
    {
        _moneyScale = moneyScale;
    }

    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(i => i.UnitPrice).HasPrecision(18, _moneyScale);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.scale", "element": "order-item-unit-price", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Infrastructure/Persistence/OrderItemConfiguration.cs:18" }
  ],
  "findings": [
    { "rule": "claim/column.scale", "severity": "error", "subject": "order-item-unit-price", "messageContains": "can't read" }
  ]
}
```

## java-verified

Over JPA, `@Column(scale = …)` (JAV-DOM-17).

```json case
{ "phase": 4, "kinds": ["column.scale"], "traces": ["JAV-DOM-17"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-item-unit-price", "type": "Attribute", "name": "UnitPrice", "typeRef": { "type": "decimal" },
                "stereotypes": { "Decimal Constraints": { "Precision": 18, "Scale": 2 } } }
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

```java file src/main/java/com/shop/domain/OrderItem.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.math.BigDecimal;
import java.util.UUID;

@Entity
public class OrderItem {
    @Id
    private UUID id;

    @Column(precision = 18, scale = 2)
    private BigDecimal unitPrice;
}
```

```json expect
{
  "claims": [
    { "kind": "column.scale", "element": "order-item-unit-price", "outcome": "verified", "expected": "2", "actual": "2",
      "at": "src/main/java/com/shop/domain/OrderItem.java:14" }
  ]
}
```

## java-contradicted

`@Column(scale = 4)` against a modelled scale of 2.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["column.scale"], "traces": ["JAV-DOM-17"], "requirements": ["R1", "R11", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/OrderItem.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.math.BigDecimal;
import java.util.UUID;

@Entity
public class OrderItem {
    @Id
    private UUID id;

    @Column(precision = 18, scale = 4)
    private BigDecimal unitPrice;
}
```

```json expect
{
  "claims": [
    { "kind": "column.scale", "element": "order-item-unit-price", "outcome": "contradicted", "expected": "2", "actual": "4",
      "at": "src/main/java/com/shop/domain/OrderItem.java:14" }
  ],
  "findings": [
    { "rule": "claim/column.scale", "severity": "error", "subject": "order-item-unit-price", "messageContains": "Decimal Constraints" }
  ]
}
```
