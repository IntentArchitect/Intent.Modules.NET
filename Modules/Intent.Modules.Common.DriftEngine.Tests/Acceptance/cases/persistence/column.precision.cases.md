# column.precision

The Decimal Constraints stereotype's Precision on a `decimal` attribute of a persisted class, against the precision the code states: EF Core's `HasPrecision(p, s)`, or JPA's `@Column(precision = …)`. `column.precision` is `stated: whenSet` and compared as a number: a provider's default precision (`decimal(18,2)` in SQL Server) is never read in its place (decision 4).

## verified

`OrderItem.UnitPrice` is modelled with a precision of 18, and the configuration says `HasPrecision(18, 2)`.

```json case
{ "phase": 4, "kinds": ["column.precision"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.precision", "element": "order-item-unit-price", "outcome": "verified", "expected": "18", "actual": "18",
      "at": "src/Infrastructure/Persistence/OrderItemConfiguration.cs:11" }
  ]
}
```

## contradicted-not-stated

The configuration states no precision, so SQL Server's default applies. That default happens to be 18, but it isn't the code stating it, so the modelled precision is contradicted, "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.precision"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(i => i.UnitPrice).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.precision", "element": "order-item-unit-price", "outcome": "contradicted", "expected": "18", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.precision", "severity": "error", "subject": "order-item-unit-price", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The attribute has no Decimal Constraints, and the code says `HasPrecision(18, 2)`. An unset `whenSet` value is no claim, so nothing is checked or reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.precision"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-item-unit-price", "type": "Attribute", "name": "UnitPrice", "typeRef": { "type": "decimal" } }
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
    { "kind": "column.precision", "element": "order-item-unit-price", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.precision", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## java-verified

Over JPA, the Java spike's prices carry `@Column(precision = …, scale = …)` (JAV-DOM-17).

```json case
{ "phase": 4, "kinds": ["column.precision"], "traces": ["JAV-DOM-17"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
    { "kind": "column.precision", "element": "order-item-unit-price", "outcome": "verified", "expected": "18", "actual": "18",
      "at": "src/main/java/com/shop/domain/OrderItem.java:14" }
  ]
}
```

## java-contradicted

`@Column(precision = 16, …)`, as the Java spike's `price` column has it, against a modelled 18.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["column.precision"], "traces": ["JAV-DOM-17"], "requirements": ["R1", "R11", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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

    @Column(precision = 16, scale = 2)
    private BigDecimal unitPrice;
}
```

```json expect
{
  "claims": [
    { "kind": "column.precision", "element": "order-item-unit-price", "outcome": "contradicted", "expected": "18", "actual": "16",
      "at": "src/main/java/com/shop/domain/OrderItem.java:14" }
  ],
  "findings": [
    { "rule": "claim/column.precision", "severity": "error", "subject": "order-item-unit-price", "messageContains": "Decimal Constraints" }
  ]
}
```

## no-rule

The Java reader's pack is active but the JPA pack isn't, so nothing reads a column's precision from Java code. The modelled precision is can't verify, `noRule`, one error for the claim, never a silent skip (principle 2).

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["column.precision"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "column.precision", "element": "order-item-unit-price", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/column.precision", "severity": "error", "subject": "order-item-unit-price", "messageContains": "no rule" }
  ]
}
```
