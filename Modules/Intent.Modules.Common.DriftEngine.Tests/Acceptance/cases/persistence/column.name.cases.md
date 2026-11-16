# column.name

The Column stereotype's Name on an attribute of a persisted class, against the column name the code states: EF Core's `HasColumnName` or `[Column("…")]`, or JPA's `@Column(name = "…")`. `column.name` is `stated: whenSet`: the column name a framework derives from the property's name (EF Core's property name, Spring's snake case) is never read in its place (decision 4).

## verified

eShopCustom's `Order` keeps its payment method's id in a property named `PaymentId`, mapped to the column `PaymentMethodId` (ESC-DOM-45).

```json case
{ "phase": 4, "kinds": ["column.name"], "traces": ["ESC-DOM-45"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.PaymentId).HasColumnName("PaymentMethodId");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.name", "element": "order-payment-id", "outcome": "verified", "expected": "PaymentMethodId", "actual": "PaymentMethodId",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

`HasColumnName` names another column. The finding names the attribute, both values and the line, and says the Column stereotype's Name states the model's side (R11, R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.name"], "traces": ["ESC-DOM-45"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.PaymentId).HasColumnName("PaymentMethod");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.name", "element": "order-payment-id", "outcome": "contradicted", "expected": "PaymentMethodId", "actual": "PaymentMethod",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.name", "severity": "error", "subject": "order-payment-id", "messageContains": "the attribute's Column Name" }
  ]
}
```

## contradicted-not-stated

The model names the column `PaymentId`, the attribute's own name, and the code has no `HasColumnName` and no `[Column]`. EF Core's convention gives exactly `PaymentId`, but the model states a column name only where the code does, so the claim is contradicted, "not stated" (decision 4). The fix is to clear the Column stereotype's Name.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.name"], "traces": ["ESC-DOM-45"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-payment-id", "type": "Attribute", "name": "PaymentId", "typeRef": { "type": "int" },
                "stereotypes": { "Column": { "Name": "PaymentId" } } }
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
    { "kind": "column.name", "element": "order-payment-id", "outcome": "contradicted", "expected": "PaymentId", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.name", "severity": "error", "subject": "order-payment-id", "messageContains": "doesn't state" }
  ]
}
```

## data-annotation-verified

The same column name stated with the `[Column]` data annotation on the property instead of the fluent API.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.name"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    [Column("PaymentMethodId")]
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
        builder.HasKey(o => o.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.name", "element": "order-payment-id", "outcome": "verified", "expected": "PaymentMethodId", "actual": "PaymentMethodId",
      "at": "src/Domain/Order.cs:9" }
  ]
}
```

## not-claimed-when-unset

The attribute has no Column stereotype, and the code says `HasColumnName("PaymentMethodId")`. An unset `whenSet` value is no claim, so nothing is checked or reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.name"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-payment-id", "type": "Attribute", "name": "PaymentId", "typeRef": { "type": "int" } }
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
    { "kind": "column.name", "element": "order-payment-id", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.name", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## blocked-when-member-missing

`Order` has no `PaymentId` property at all. `member.exists` is contradicted, and the column name, which depends on it, is can't verify, `blocked`: only the missing member's finding is reported, not a second one about its column (R5).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.name", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-payment-id", "outcome": "contradicted" },
    { "kind": "column.name", "element": "order-payment-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-payment-id" },
    { "rule": "claim/column.name", "count": 0 }
  ]
}
```

## java-verified

Over JPA the column name is `@Column(name = …)` on the field (JAV-DOM-15).

```json case
{ "phase": 4, "kinds": ["column.name"], "traces": ["JAV-DOM-15"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-payment-id", "type": "Attribute", "name": "PaymentId", "typeRef": { "type": "long" },
                "stereotypes": { "Column": { "Name": "payment_method_id" } } }
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

    @Column(name = "payment_method_id")
    private Long paymentId;
}
```

```json expect
{
  "claims": [
    { "kind": "column.name", "element": "order-payment-id", "outcome": "verified", "expected": "payment_method_id", "actual": "payment_method_id",
      "at": "src/main/java/com/shop/domain/Order.java:13" }
  ]
}
```

## java-contradicted

`@Column(name = …)` names another column.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["column.name"], "traces": ["JAV-DOM-15"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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

    @Column(name = "payment_method")
    private Long paymentId;
}
```

```json expect
{
  "claims": [
    { "kind": "column.name", "element": "order-payment-id", "outcome": "contradicted", "expected": "payment_method_id", "actual": "payment_method",
      "at": "src/main/java/com/shop/domain/Order.java:13" }
  ]
}
```

## java-contradicted-not-stated

The field has no `@Column(name = …)`. Spring's naming strategy would give `payment_id`, but a convention never stands in for a `whenSet` value, so the modelled `payment_method_id` is contradicted, "not stated" (decision 4).

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["column.name"], "traces": ["JAV-DOM-15"], "requirements": ["R4", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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

    private Long paymentId;
}
```

```json expect
{
  "claims": [
    { "kind": "column.name", "element": "order-payment-id", "outcome": "contradicted", "expected": "payment_method_id", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.name", "severity": "error", "subject": "order-payment-id", "messageContains": "doesn't state" }
  ]
}
```

## backing-field-verified

eShopCustom's `PaymentMethod` keeps its card number in a private field with no property, and its configuration maps the field by name: `Property("_cardNumber").HasColumnName("CardNumber")` (ESC-DOM-42). Three pieces make that checkable, all from the module's vocabulary, brought inline as a house pack (packs.md, "Who ships what"):

- a `member.backingField` entry, so the attribute, which has no property of its own name, is located as the field its Backing Field names (rules.md, "Locating an element's code"), and `member.exists`, which `column.name` depends on, finds it;
- an extending entry that binds `{BackingField}` onto the stock `intent.domain/column.name` claims of attributes with Backing Field (vocabulary.md, "Extending another pack's entry"); the stock entry still decides the claim and its value, the Column stereotype's Name;
- a rule for those attributes only, replacing EF Core's, that finds the chain by the field's name and reads `HasColumnName`.

`member.backingField` is phase `later`, so this case is too.

```json case
{ "phase": "later", "kinds": ["column.name", "member.exists"], "traces": ["ESC-DOM-42"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
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
          "kind": "member.backingField",
          "for": { "element": "Attribute", "where": "hasStereotype('Backing Field')" },
          "value": "stereotype('Backing Field').Field",
          "bindings": { "Entity": "parent.name", "Attribute": "name" },
          "own": ["Attribute"],
          "dependsOn": "parent:type.exists",
          "source": "the Backing Field stereotype's Field"
        },
        {
          "extends": "intent.domain/column.name",
          "for": { "element": "Attribute", "where": "hasStereotype('Backing Field')" },
          "bindings": { "BackingField": "stereotype('Backing Field').Field" }
        }
      ]
    }
  },
  "rules": {
    "Domain": {
      "facts": {
        "column.name": {
          "for": { "element": "Attribute", "where": "hasStereotype('Backing Field')" },
          "replaces": "intent.efcore/column.name",
          "find": "class:{Entity}Configuration > chain:Property(\"{BackingField}\")",
          "read": [{ "match": "\\.HasColumnName\\(\"([^\"]+)\"\\)" }],
          "note": "A field mapped by name: the column name is on the field's chain."
        }
      }
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
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "payment-method-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "payment-method-card-number", "type": "Attribute", "name": "CardNumber", "typeRef": { "type": "string" },
                "stereotypes": { "Column": { "Name": "CardNumber" }, "Backing Field": { "Field": "_cardNumber" } } }
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

```csharp file src/Domain/PaymentMethod.cs
namespace Shop.Domain;

public class PaymentMethod
{
    public int Id { get; private set; }

    private string _cardNumber;
}
```

```csharp file src/Infrastructure/Persistence/PaymentMethodConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.Property("_cardNumber").HasColumnName("CardNumber").HasMaxLength(25).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "payment-method-card-number", "outcome": "verified", "at": "src/Domain/PaymentMethod.cs:7" },
    { "kind": "column.name", "element": "payment-method-card-number", "outcome": "verified", "expected": "CardNumber", "actual": "CardNumber",
      "at": "src/Infrastructure/Persistence/PaymentMethodConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "config/vocabularyConflict", "count": 0 },
    { "rule": "config/ruleConflict", "count": 0 }
  ]
}
```

## backing-field-contradicted-not-stated

The field is mapped, but its chain has no `HasColumnName`, so EF Core names the column after the field, `_cardNumber`. That is a convention, and the modelled `CardNumber` is contradicted, "not stated" (decision 4), the drift ESC-DOM-42 caught. Its "or the field name" half was that convention, which a `whenSet` value no longer takes.

```json case
{ "phase": "later", "base": "backing-field-verified", "kinds": ["column.name"], "traces": ["ESC-DOM-42"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/PaymentMethodConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.Property("_cardNumber").HasMaxLength(25).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.name", "element": "payment-method-card-number", "outcome": "contradicted", "expected": "CardNumber", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.name", "severity": "error", "subject": "payment-method-card-number", "messageContains": "doesn't state" }
  ]
}
```
