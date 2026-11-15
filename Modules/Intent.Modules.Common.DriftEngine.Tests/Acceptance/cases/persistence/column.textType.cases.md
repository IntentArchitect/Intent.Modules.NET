# column.textType

The SQL text type of a `string` attribute of a persisted class: the Text Constraints stereotype's `SQL Data Type` (`VARCHAR`, `NVARCHAR`, `TEXT` or `NTEXT`), and `DEFAULT` when it isn't modelled. EF Core states it with `IsUnicode(false)` (`VARCHAR`), `IsUnicode()` (`NVARCHAR`) or a `HasColumnType` naming a text type. `column.textType` is `stated: always`: a string property with neither leaves the text type to the provider, which is exactly what the model's `DEFAULT` says, so silence reads `DEFAULT` honestly (R3). A conversion-only kind: Common.Drift's Domain recipes check it, and no eShopCustom or Java check does (claim-catalog.md).

## verified

`Order.Description` is modelled `VARCHAR`, and the configuration says `IsUnicode(false)`.

```json case
{ "phase": 4, "kinds": ["column.textType"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
                "stereotypes": { "Text Constraints": { "SQL Data Type": "VARCHAR" } } }
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
        builder.Property(o => o.Description).IsUnicode(false);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.textType", "element": "order-description", "outcome": "verified", "expected": "VARCHAR", "actual": "VARCHAR",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## verified-column-type

`NVARCHAR` stated through the column type: `HasColumnType("nvarchar(200)")` names the text type with a length, and the text type is read from it.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.textType"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
                "stereotypes": { "Text Constraints": { "SQL Data Type": "NVARCHAR" } } }
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
        builder.Property(o => o.Description).HasColumnType("nvarchar(200)");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.textType", "element": "order-description", "outcome": "verified", "expected": "NVARCHAR", "actual": "NVARCHAR",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The model says `VARCHAR`, and the configuration says `IsUnicode()`, which is `NVARCHAR`. The finding names the Text Constraints stereotype's SQL Data Type as the model's side (R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.textType"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.Description).IsUnicode();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.textType", "element": "order-description", "outcome": "contradicted", "expected": "VARCHAR", "actual": "NVARCHAR",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.textType", "severity": "error", "subject": "order-description", "messageContains": "SQL Data Type" }
  ]
}
```

## contradicted-from-silence

The model says `VARCHAR`, and the configuration says nothing about the text type, which leaves it to the provider: `DEFAULT`, read from silence (R3), and not the modelled value.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.textType"], "cases": ["engine/silence"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.textType", "element": "order-description", "outcome": "contradicted", "expected": "VARCHAR", "actual": "DEFAULT", "fromSilence": true }
  ]
}
```

## verified-from-silence

No Text Constraints on the attribute, so the model says `DEFAULT`, and the configuration says nothing about the text type: verified from silence (R3).

```json case
{ "phase": 4, "base": "contradicted-from-silence", "kinds": ["column.textType"], "cases": ["engine/silence"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.textType", "element": "order-description", "outcome": "verified", "expected": "DEFAULT", "actual": "DEFAULT", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/column.textType", "count": 0 }
  ]
}
```

## contradicted-unmodelled

No Text Constraints on the attribute, so the model says `DEFAULT`, and the configuration says `IsUnicode(false)`. Because the kind is `always`, the model's silence is a statement too, and the code's `VARCHAR` contradicts it: the model should say `VARCHAR`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.textType"], "requirements": ["R1"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.textType", "element": "order-description", "outcome": "contradicted", "expected": "DEFAULT", "actual": "VARCHAR",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## not-claimed-with-column-type

No Text Constraints on the attribute, but its Column stereotype's Type is `char(1)`, a store type that settles its text type, which `column.type` checks. The model doesn't leave the text type to the provider, so there is no `DEFAULT` claim against the configuration's `IsUnicode(false)`, which states the same `char(1)` with `IsFixedLength()`.

```json case
{ "phase": 4, "base": "contradicted-unmodelled", "kinds": ["column.textType"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
                "stereotypes": { "Column": { "Type": "char(1)" } } }
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
        builder.Property(o => o.Description).HasMaxLength(1).IsUnicode(false).IsFixedLength();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.textType", "element": "order-description", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.textType", "count": 0 }
  ]
}
```

## out-of-scope

The only configuration for `Order` sits in a folder a scope row excludes. The engine never opens it, but its path matches the find's named step, so the claim is can't verify, `outOfScope`, naming the file and the row (R25). Shadow lookup comes before silence in the outcome order, so an `always` kind doesn't read `DEFAULT` from code it was never allowed to see.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Infrastructure/Persistence/OrderConfiguration.cs"], "kinds": ["column.textType"], "cases": ["scope/shadow"], "requirements": ["R5", "R22", "R25"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
                "stereotypes": { "Text Constraints": { "SQL Data Type": "VARCHAR" } } }
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
        builder.Property(o => o.Description).IsUnicode(false);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.textType", "element": "order-description", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/column.textType", "severity": "error", "subject": "order-description", "messageContains": "src/Legacy/OrderConfiguration.cs" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Legacy/**"] }
}
```
