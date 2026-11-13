# column.mapped

Whether a modelled attribute of a persisted class is mapped to a column: always `true` in the model, since an attribute of a persisted class is persisted. EF Core states otherwise with `Ignore(x => x.Attr)` or `[NotMapped]`. eShopCustom checked it as `persistence.ignored` (ESC-DOM-39). `column.mapped` is `stated: always`: EF Core maps a public property with a getter and a setter unless the code ignores it, so a property nothing ignores really is mapped, and the reading from silence is honest (R3).

## verified-from-silence

eShopCustom's `Order` configuration ignores its `DomainEvents`, which the model doesn't have, and nothing ignores `Description`. `Description` is mapped, read from silence; the ignored `DomainEvents` is code the model doesn't mention, so it isn't drift (principle 1).

```json case
{ "phase": 4, "kinds": ["column.mapped"], "cases": ["engine/silence"], "traces": ["ESC-DOM-39"], "requirements": ["R2", "R3", "R44"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public string Description { get; private set; }

    public List<object> DomainEvents { get; } = new();
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
        builder.Ignore(o => o.DomainEvents);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.mapped", "element": "order-description", "outcome": "verified", "expected": "true", "actual": "true", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/column.mapped", "count": 0 }
  ]
}
```

## contradicted

The configuration also ignores `Description`, which the model persists. The claim is contradicted at the `Ignore` line, the drift `persistence.ignored` caught.

```json case
{ "phase": 4, "base": "verified-from-silence", "kinds": ["column.mapped"], "traces": ["ESC-DOM-39"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Ignore(o => o.DomainEvents);
        builder.Ignore(o => o.Description);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.mapped", "element": "order-description", "outcome": "contradicted", "expected": "true", "actual": "false",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:12" }
  ],
  "findings": [
    { "rule": "claim/column.mapped", "severity": "error", "subject": "order-description" }
  ]
}
```

## contradicted-not-mapped-attribute

The same drift through the data annotation: `[NotMapped]` on the property.

```json case
{ "phase": 4, "base": "verified-from-silence", "kinds": ["column.mapped"], "requirements": ["R1"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    [NotMapped]
    public string Description { get; private set; }

    public List<object> DomainEvents { get; } = new();
}
```

```json expect
{
  "claims": [
    { "kind": "column.mapped", "element": "order-description", "outcome": "contradicted", "expected": "true", "actual": "false",
      "at": "src/Domain/Order.cs:9" }
  ]
}
```

## ambiguous

A second configuration for `Order`, in another namespace, is also in scope. The find ends at two configurations where one is expected, so the claim is can't verify, `ambiguous`, naming both. Silence isn't read from either: the outcome order decides ambiguity before silence.

```json case
{ "phase": 4, "base": "verified-from-silence", "kinds": ["column.mapped"], "cases": ["engine/ambiguous"], "requirements": ["R5", "R43"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Reporting/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Reporting;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Ignore(o => o.Description);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.mapped", "element": "order-description", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/column.mapped", "severity": "error", "subject": "order-description", "messageContains": "src/Infrastructure/Reporting/OrderConfiguration.cs" }
  ]
}
```
