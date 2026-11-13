# key.generated

How a primary key attribute gets its values: the Primary Key stereotype's `Data source`, `Auto-generated` or `User supplied`. `Default`, the property's default, counts as unset, so it makes no claim. EF Core states it with `ValueGeneratedNever()` (`User supplied`) or `ValueGeneratedOnAdd()` (`Auto-generated`); JPA with `@GeneratedValue` (`Auto-generated`). `key.generated` is `stated: whenSet`: the value EF Core or JPA would choose by convention is never read in its place (decision 4).

## verified

eShopCustom's `CardType` has a key the application supplies (an enumeration's values), modelled as `User supplied`, and its configuration says `ValueGeneratedNever()` (ESC-DOM-47).

```json case
{ "phase": 4, "kinds": ["key.generated"], "traces": ["ESC-DOM-47"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "card-type", "type": "Class", "name": "CardType",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "card-type-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" },
                "stereotypes": { "Primary Key": { "Data source": "User supplied" } } },
              { "id": "card-type-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } }
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

```csharp file src/Domain/CardType.cs
namespace Shop.Domain;

public class CardType
{
    public int Id { get; private set; }
    public string Name { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/CardTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class CardTypeConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> builder)
    {
        builder.Property(ct => ct.Id).ValueGeneratedNever();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key.generated", "element": "card-type-id", "outcome": "verified", "expected": "User supplied", "actual": "User supplied",
      "at": "src/Infrastructure/Persistence/CardTypeConfiguration.cs:11" }
  ]
}
```

## contradicted

The configuration says `ValueGeneratedOnAdd()`: the database generates the key the model says the application supplies.

```json case
{ "phase": 4, "base": "verified", "kinds": ["key.generated"], "traces": ["ESC-DOM-47"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/CardTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class CardTypeConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> builder)
    {
        builder.Property(ct => ct.Id).ValueGeneratedOnAdd();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key.generated", "element": "card-type-id", "outcome": "contradicted", "expected": "User supplied", "actual": "Auto-generated",
      "at": "src/Infrastructure/Persistence/CardTypeConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/key.generated", "severity": "error", "subject": "card-type-id", "messageContains": "Data source" }
  ]
}
```

## contradicted-not-stated

The model says `User supplied` and the configuration says nothing about value generation. EF Core's convention for an `int` key is to generate it, but the point is that the code doesn't state the modelled value either way: contradicted, with the code side "not stated" (decision 4). ESC-DOM-47 checked both ways; this is the model-to-code way.

```json case
{ "phase": 4, "base": "verified", "kinds": ["key.generated"], "traces": ["ESC-DOM-47"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/CardTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class CardTypeConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> builder)
    {
        builder.HasKey(ct => ct.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key.generated", "element": "card-type-id", "outcome": "contradicted", "expected": "User supplied", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/key.generated", "severity": "error", "subject": "card-type-id", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

`Data source` is `Default`, the property's default, which counts as unset, and the configuration says `ValueGeneratedNever()`. An unset `whenSet` value is no claim, so nothing is checked or reported (principle 1). ESC-DOM-47's code-to-model way is the opt-in `unmodelled.value` rule's job.

```json case
{ "phase": 4, "base": "verified", "kinds": ["key.generated"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "card-type", "type": "Class", "name": "CardType",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "card-type-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" },
                "stereotypes": { "Primary Key": { "Data source": "Default" } } },
              { "id": "card-type-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } }
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
    { "kind": "key.generated", "element": "card-type-id", "absent": true }
  ],
  "findings": [
    { "rule": "claim/key.generated", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## ambiguous

The configuration configures the key property twice, once `ValueGeneratedNever()` and once `ValueGeneratedOnAdd()`. EF Core would take the last, but the find ends at two chains where one is expected, so the claim is can't verify, `ambiguous`, naming both lines, rather than the engine guessing which one wins.

```json case
{ "phase": 4, "base": "verified", "kinds": ["key.generated"], "cases": ["engine/ambiguous"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/CardTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class CardTypeConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> builder)
    {
        builder.Property(ct => ct.Id).ValueGeneratedNever();
        builder.Property(ct => ct.Id).ValueGeneratedOnAdd();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key.generated", "element": "card-type-id", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/key.generated", "severity": "error", "subject": "card-type-id", "messageContains": "CardTypeConfiguration.cs:12" }
  ]
}
```

## java-verified

Over JPA, `Auto-generated` is `@GeneratedValue` on the key field (JAV-DOM-18's second half).

```json case
{ "phase": 4, "kinds": ["key.generated"], "traces": ["JAV-DOM-18"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": { "Data source": "Auto-generated" } } }
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
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Order {
    @Id
    @GeneratedValue
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "key.generated", "element": "order-id", "outcome": "verified", "expected": "Auto-generated", "actual": "Auto-generated",
      "at": "src/main/java/com/shop/domain/Order.java:11" }
  ]
}
```

## java-contradicted

The model says `User supplied`, and the key field has `@GeneratedValue`.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["key.generated"], "traces": ["JAV-DOM-18"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": { "Data source": "User supplied" } } }
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

```json expect
{
  "claims": [
    { "kind": "key.generated", "element": "order-id", "outcome": "contradicted", "expected": "User supplied", "actual": "Auto-generated",
      "at": "src/main/java/com/shop/domain/Order.java:11" }
  ]
}
```

## java-contradicted-not-stated

The model says `Auto-generated`, and the key field has no `@GeneratedValue`: the code doesn't state the modelled value, so it is contradicted, "not stated" (decision 4).

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["key.generated"], "traces": ["JAV-DOM-18"], "requirements": ["R4", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
}
```

```json expect
{
  "claims": [
    { "kind": "key.generated", "element": "order-id", "outcome": "contradicted", "expected": "Auto-generated", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/key.generated", "severity": "error", "subject": "order-id", "messageContains": "doesn't state" }
  ]
}
```
