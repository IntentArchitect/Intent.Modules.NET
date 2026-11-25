# column.type

The Column stereotype's Type on an attribute of a persisted class, against the database type the code states: EF Core's `HasColumnType` or `[Column(TypeName = "…")]`. `column.type` is `stated: whenSet`, so the type a provider maps a CLR type to by default is never read in its place (decision 4), and it compares with `ignoreCase`, because SQL type names don't depend on case (R10).

## verified

`Order.OrderDate` is modelled as a `datetime2` column, and the configuration says `HasColumnType("datetime2")` (ESC-DOM-46).

```json case
{ "phase": 4, "kinds": ["column.type"], "traces": ["ESC-DOM-46"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-order-date", "type": "Attribute", "name": "OrderDate", "typeRef": { "type": "datetime" },
                "stereotypes": { "Column": { "Type": "datetime2" } } }
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
        builder.Property(o => o.OrderDate).HasColumnType("datetime2");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "order-order-date", "outcome": "verified", "expected": "datetime2", "actual": "datetime2",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## verified-ignore-case

The model writes the type in capitals and the code in lower case. `column.type`'s `ignoreCase` comparer verifies it (R10).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.type"], "cases": ["engine/comparers"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-order-date", "type": "Attribute", "name": "OrderDate", "typeRef": { "type": "datetime" },
                "stereotypes": { "Column": { "Type": "DATETIME2" } } }
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
    { "kind": "column.type", "element": "order-order-date", "outcome": "verified", "expected": "DATETIME2", "actual": "datetime2",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The configuration says `datetime`, a different SQL Server type with less precision. The finding names the attribute, both values and the line, and says the Column stereotype's Type states the model's side (R11, R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.type"], "traces": ["ESC-DOM-46"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.OrderDate).HasColumnType("datetime");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "order-order-date", "outcome": "contradicted", "expected": "datetime2", "actual": "datetime",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.type", "severity": "error", "subject": "order-order-date", "messageContains": "the attribute's Column Type" }
  ]
}
```

## contradicted-not-stated

The configuration doesn't state a column type. SQL Server's provider maps `DateTime` to `datetime2` by default, but that default isn't the code stating it, so the modelled `datetime2` is contradicted, "not stated" (decision 4). ESC-DOM-46 checked both ways; this is the model-to-code way.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.type"], "traces": ["ESC-DOM-46"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.OrderDate).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "order-order-date", "outcome": "contradicted", "expected": "datetime2", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.type", "severity": "error", "subject": "order-order-date", "messageContains": "doesn't state" }
  ]
}
```

## data-annotation-verified

The same type stated with `[Column(TypeName = "datetime2")]` on the property instead of the fluent API.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.type"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    [Column(TypeName = "datetime2")]
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
        builder.HasKey(o => o.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "order-order-date", "outcome": "verified", "expected": "datetime2", "actual": "datetime2",
      "at": "src/Domain/Order.cs:9" }
  ]
}
```

## verified-by-ef6-properties-convention

No configuration of `Order` states a column type; the EF6 context's `modelBuilder.Properties<DateTime>().Configure(x => x.HasColumnType("datetime2"))` states it for every `DateTime` property it maps, a `DateTime?` included.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["column.type"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public DateTime? OrderDate { get; private set; }
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
        modelBuilder.Properties<DateTime>().Configure(x => x.HasColumnType("datetime2"));
    }

    public virtual DbSet<Order> Orders { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "order-order-date", "outcome": "verified", "expected": "datetime2", "actual": "datetime2",
      "at": "src/Infrastructure/OrderingContext.cs:11" }
  ]
}
```

## not-claimed-when-unset

The attribute has no Column stereotype, and the code says `HasColumnType("datetime2")`. An unset `whenSet` value is no claim, so nothing is checked or reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.type"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "order-order-date", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.type", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## unreadable

The configuration takes the column type from its constructor, so one configuration serves two database providers. `HasColumnType(_dateColumnType)` states a type the rule can't read without running the code, so the claim is can't verify, `unreadable`, naming the line (R5), rather than "not stated" or a guess.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.type"], "cases": ["engine/unreadable"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    private readonly string _dateColumnType;

    public OrderConfiguration(string dateColumnType)
    {
        _dateColumnType = dateColumnType;
    }

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.OrderDate).HasColumnType(_dateColumnType);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "order-order-date", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:18" }
  ],
  "findings": [
    { "rule": "claim/column.type", "severity": "error", "subject": "order-order-date", "messageContains": "can't read" }
  ]
}
```
