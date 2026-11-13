# table.schema

The schema a Domain class's table lives in: the Table or View stereotype's Schema, else the nearest folder's or the package's Schema stereotype Name. The code states it in `ToTable`'s schema argument, or in the `HasDefaultSchema` of the DbContext that maps the class. eShopCustom compared the package's Schema with `HasDefaultSchema` once per package; the engine claims it once per class with Table or View (claim-catalog.md). `table.schema` is `stated: whenSet`: a provider's default schema (`dbo`, `public`) is never read in its place (decision 4).

## verified

The package's Schema is `ordering`, and the DbContext that applies `Order`'s configuration calls `HasDefaultSchema("ordering")`.

```json case
{ "phase": 4, "kinds": ["table.schema"], "traces": ["ESC-DOM-09"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "stereotypes": { "Schema": { "Name": "ordering" } },
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
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
        builder.HasKey(x => x.Id);
    }
}
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ordering");
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.schema", "element": "order", "outcome": "verified", "expected": "ordering", "actual": "ordering",
      "at": "src/Infrastructure/OrderingContext.cs:12" }
  ]
}
```

## contradicted

The context sets another default schema. The finding names the class, both values and the line, and says the package's Schema stereotype states the model's side (R11, R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.schema"], "traces": ["ESC-DOM-09"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.schema", "element": "order", "outcome": "contradicted", "expected": "ordering", "actual": "orders",
      "at": "src/Infrastructure/OrderingContext.cs:12" }
  ],
  "findings": [
    { "rule": "claim/table.schema", "severity": "error", "subject": "order", "messageContains": "Schema stereotype" }
  ]
}
```

## contradicted-not-stated

The context applies `Order`'s configuration but never calls `HasDefaultSchema`, and `ToTable` names no schema, so the table lands in the provider's default schema. The model states `ordering` and the code states nothing: contradicted, with the code side "not stated" (decision 4). The fix is in the model (clear the Schema) or the code (add `HasDefaultSchema`).

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.schema"], "traces": ["ESC-DOM-09"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.schema", "element": "order", "outcome": "contradicted", "expected": "ordering", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/table.schema", "severity": "error", "subject": "order", "messageContains": "doesn't state" }
  ]
}
```

## contradicted-no-dbcontext

The package has a Schema but no DbContext is in the code at all, the case eShopCustom reported as "a Schema with no DbContext". `Order` and its configuration are found, so the claim isn't blocked: nothing in the code states a schema, and the modelled one is contradicted, not stated.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/OrderingContext.cs"], "kinds": ["table.schema"], "traces": ["ESC-DOM-09"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "table.schema", "element": "order", "outcome": "contradicted", "expected": "ordering", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/table.schema", "severity": "error", "subject": "order", "messageContains": "doesn't state" }
  ]
}
```

## table-stereotype-schema-verified

The Table stereotype's own Schema comes before the package's, on both sides: the model's value is `sales`, and `ToTable`'s schema argument is read before the context's `HasDefaultSchema("ordering")`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.schema"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "stereotypes": { "Schema": { "Name": "ordering" } },
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": { "Schema": "sales" } },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
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
        builder.ToTable("Orders", "sales");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.schema", "element": "order", "outcome": "verified", "expected": "sales", "actual": "sales",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## not-claimed-when-unset

Neither the package nor the Table stereotype has a Schema, and the context calls `HasDefaultSchema("ordering")`. An unset `whenSet` value is no claim, so nothing is checked or reported (principle 1). The opt-in `unmodelled.value` rule would report the code's schema if a team turned it on.

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.schema"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
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
    { "kind": "table.schema", "element": "order", "absent": true }
  ],
  "findings": [
    { "rule": "claim/table.schema", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## unreadable

The context takes its schema from configuration at run time, so `HasDefaultSchema(_schema)` states a schema the rule can't read without running the code. The recipe's read case for a non-literal argument is marked `unreadable`: can't verify, naming the line (R5), rather than a guess or "not stated".

```json case
{ "phase": 4, "base": "verified", "kinds": ["table.schema"], "cases": ["engine/unreadable"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    private readonly string _schema;

    public OrderingContext(DbContextOptions<OrderingContext> options, IConfiguration configuration) : base(options)
    {
        _schema = configuration["Ordering:Schema"];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_schema);
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.schema", "element": "order", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Infrastructure/OrderingContext.cs:18" }
  ],
  "findings": [
    { "rule": "claim/table.schema", "severity": "error", "subject": "order", "messageContains": "can't read" }
  ]
}
```
