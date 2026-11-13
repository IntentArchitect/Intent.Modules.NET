# table.kind

Whether a Domain class maps to a table or a view: `View` when the class has the View stereotype, `Table` when it has the Table stereotype. The code states it with `ToView` or `ToTable`. `table.kind` is `stated: always`: an EF Core entity that isn't mapped to a view is mapped to a table, so a configuration with no `ToView` really does state `Table`, and the reading from silence is honest (principle 3). A conversion-only kind: Common.Drift's Domain recipes check it, and no eShopCustom or Java check does (claim-catalog.md).

## verified-view

The model's `OrderSummary` has the View stereotype, and its configuration maps it with `ToView`.

```json case
{ "phase": 4, "kinds": ["table.kind"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-summary", "type": "Class", "name": "OrderSummary",
            "stereotypes": { "View": { "Name": "OrderSummaries" } },
            "children": [
              { "id": "order-summary-order-number", "type": "Attribute", "name": "OrderNumber", "typeRef": { "type": "int" } },
              { "id": "order-summary-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } }
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

```csharp file src/Domain/OrderSummary.cs
namespace Shop.Domain;

public class OrderSummary
{
    public int OrderNumber { get; private set; }
    public decimal Total { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/OrderSummaryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderSummaryConfiguration : IEntityTypeConfiguration<OrderSummary>
{
    public void Configure(EntityTypeBuilder<OrderSummary> builder)
    {
        builder.ToView("OrderSummaries");
        builder.HasNoKey();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.kind", "element": "order-summary", "outcome": "verified", "expected": "View", "actual": "View",
      "at": "src/Infrastructure/Persistence/OrderSummaryConfiguration.cs:11" }
  ]
}
```

## contradicted

The model says view, and the configuration maps the class to a table with `ToTable`. The finding names both values and the line, and says the View stereotype states the model's side (R11, R14).

```json case
{ "phase": 4, "base": "verified-view", "kinds": ["table.kind"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderSummaryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderSummaryConfiguration : IEntityTypeConfiguration<OrderSummary>
{
    public void Configure(EntityTypeBuilder<OrderSummary> builder)
    {
        builder.ToTable("OrderSummaries");
        builder.HasNoKey();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.kind", "element": "order-summary", "outcome": "contradicted", "expected": "View", "actual": "Table",
      "at": "src/Infrastructure/Persistence/OrderSummaryConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/table.kind", "severity": "error", "subject": "order-summary", "messageContains": "View" }
  ]
}
```

## contradicted-from-silence

The model says view, and the configuration neither calls `ToView` nor `ToTable`. EF Core maps the entity to a table, and the code's silence states exactly that, so the `always` kind reads `Table` from silence (R3) and the claim is contradicted. This is the difference from a `whenSet` kind: silence here is a statement, not a gap.

```json case
{ "phase": 4, "base": "verified-view", "kinds": ["table.kind"], "cases": ["engine/silence"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderSummaryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderSummaryConfiguration : IEntityTypeConfiguration<OrderSummary>
{
    public void Configure(EntityTypeBuilder<OrderSummary> builder)
    {
        builder.HasNoKey();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.kind", "element": "order-summary", "outcome": "contradicted", "expected": "View", "actual": "Table", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/table.kind", "severity": "error", "subject": "order-summary" }
  ]
}
```

## verified-from-silence

The model's `Order` has the Table stereotype, and its configuration says nothing about tables or views. The silence states `Table`, so the claim is verified from silence (R3).

```json case
{ "phase": 4, "kinds": ["table.kind"], "cases": ["engine/silence"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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

```json expect
{
  "claims": [
    { "kind": "table.kind", "element": "order", "outcome": "verified", "expected": "Table", "actual": "Table", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/table.kind", "count": 0 }
  ]
}
```

## ambiguous

Two configurations for `OrderSummary`, one in `Persistence` and a second in `Reporting`, both in scope. The find ends at two places where one is expected, so the claim is can't verify, `ambiguous`, and the finding names both candidates rather than picking one (R43).

```json case
{ "phase": 4, "base": "verified-view", "kinds": ["table.kind"], "cases": ["engine/ambiguous"], "requirements": ["R5", "R43"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Reporting/OrderSummaryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Reporting;

public class OrderSummaryConfiguration : IEntityTypeConfiguration<OrderSummary>
{
    public void Configure(EntityTypeBuilder<OrderSummary> builder)
    {
        builder.ToTable("OrderSummaryReport");
        builder.HasNoKey();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.kind", "element": "order-summary", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/table.kind", "severity": "error", "subject": "order-summary", "messageContains": "src/Infrastructure/Reporting/OrderSummaryConfiguration.cs" }
  ]
}
```
