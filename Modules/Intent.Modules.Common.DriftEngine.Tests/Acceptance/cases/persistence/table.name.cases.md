# table.name

The Table stereotype's Name on a Domain class, against the table name the code states. `table.name` is `stated: whenSet`: the model names a table only where the code names one (decision 4), so a convention name is never read in its place.

## verified

The model names the table `Orders` and the configuration's `ToTable` says `Orders`.

```json case
{ "phase": 4, "kinds": ["table.name"], "traces": ["ESC-DOM-52"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
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
    public Guid Id { get; set; }
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

        builder.HasKey(x => x.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "verified", "expected": "Orders", "actual": "Orders",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The configuration names another table. The finding names the claim, the element and the line, and says which model property states the value (R11, R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.name"], "traces": ["ESC-DOM-52"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.ToTable("Order");

        builder.HasKey(x => x.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "contradicted", "expected": "Orders", "actual": "Order",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/table.name", "severity": "error", "subject": "order", "messageContains": "the class's Table Name" }
  ]
}
```

## contradicted-not-stated

The configuration has no `ToTable`. EF Core's convention may well give `Orders`, but the model states a table name only where the code does, so the modelled name is contradicted, with the code side "not stated" (decision 4). The fix is in the model: clear the Name if the convention's name is right. This is Common.Drift's `default: "{Entity}"` trick no longer carried over (conversion.md, semantic change 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.name"], "traces": ["ESC-DOM-52"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "contradicted", "expected": "Orders", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/table.name", "severity": "error", "subject": "order", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The model leaves the table name unset and the code says `ToTable("Orders")`. An unset `whenSet` value is no claim, so nothing is checked and nothing is reported: code the model doesn't mention isn't drift (principle 1). The opt-in `unmodelled.value` rule reports it when a team turns it on ([optin/unmodelled.value](../optin/unmodelled.value.cases.md)).

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.name"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "table.name", "element": "order", "absent": true }
  ],
  "findings": [
    { "rule": "claim/table.name", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## out-of-scope

The only configuration for `Order` sits in a folder a scope row excludes. The engine never opens it, but its path matches the find's named step, so the claim is can't verify, `outOfScope`, naming the file and the row (R25), rather than "not stated".

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["table.name"], "cases": ["scope/shadow"], "requirements": ["R5", "R22", "R25"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Infrastructure" }
        ] } } }]
    }
  }
}
```

```csharp file src/Legacy/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Legacy;

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
    { "kind": "table.name", "element": "order", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/table.name", "severity": "error", "subject": "order", "messageContains": "src/Legacy/OrderConfiguration.cs" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Legacy/**"] }
}
```

## no-rule

Only the core and Domain packs are active, so nothing reads a table name: no EF Core pack, no house pack. Every modelled table name is can't verify, `noRule`, one error per claim (conversion.md, semantic change 6), never a silent skip (principle 2).

```json case
{ "phase": 1, "base": "verified", "kinds": ["table.name"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/table.name", "severity": "error", "subject": "order", "messageContains": "no rule" }
  ]
}
```

## blocked-when-type-missing

`Order` itself isn't in the code. `type.exists` is contradicted, and the table name, which depends on it, is can't verify, `blocked`: one finding for the missing type counts it, rather than one per claim (R5). Its configuration still names it, but the code's own types live in `Shop.Domain`, where the model places it, so it isn't a library's type the code references (structure/type.exists#library-type).

```json case
{ "phase": 4, "base": "verified", "without": ["src/Domain/Order.cs"], "kinds": ["table.name", "type.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Buyer.cs
namespace Shop.Domain;

public class Buyer
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "contradicted" },
    { "kind": "table.name", "element": "order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "isn't in the code" }
  ]
}
```

## java-verified

The same claim over JPA: `@Table(name = "Orders")` on the entity.

```json case
{ "phase": 4, "kinds": ["table.name"], "traces": ["JAV-DOM-07"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
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
@Table(name = "Orders")
public class Order {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "verified", "expected": "Orders", "actual": "Orders",
      "at": "src/main/java/com/shop/domain/Order.java:9" }
  ]
}
```
