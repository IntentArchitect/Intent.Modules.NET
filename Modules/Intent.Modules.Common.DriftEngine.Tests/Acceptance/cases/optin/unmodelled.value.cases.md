# optin/unmodelled.value

The opt-in rule that reports an explicit value the code states for a `whenSet` kind that the model leaves unset, such as a `ToTable` with no modelled table name (rules.md, "Opt-in must be modelled rules"; decision 6). By decision 4 the model states a value only where the code states it, and an unset `whenSet` value is no claim, so by default code that states more than the model is no drift (principle 1, decision 10). Common.Drift stated empty model values so these were caught; a team that wants that back turns this rule on.

The rule reports only what the code states explicitly: a framework's convention is silence, never a value. A finding's subject is the file and the line of the statement the kind's rule read.

## off-by-default

`Order` has the Table stereotype with no Name, and `Description` has no MaxLength. The configuration says `ToTable("Orders")` and `HasMaxLength(200)`. Neither value is a claim, and no `Opt-in Rules` row turns the rule on, so nothing is reported.

```json case
{ "phase": 4, "kinds": ["table.name", "column.maxLength"], "cases": ["optin/unmodelled.value"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    public Guid Id { get; set; }

    public string Description { get; set; }
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

        builder.Property(x => x.Description).HasMaxLength(200);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "absent": true },
    { "kind": "column.maxLength", "element": "order-description", "absent": true }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## on

The same model and code, with the rule on. The code states a table name and a max length the model leaves unset, so each is reported where the code states it, an error by default. Still neither is a claim: the rule reports code the model doesn't describe, and the fix is to model the value or to except it. eShopCustom's Domain module caught the table name by comparing it with an empty Name (ESC-DOM-52's code-to-model half).

```json case
{ "phase": 4, "base": "off-by-default", "kinds": ["table.name", "column.maxLength"], "cases": ["optin/unmodelled.value"], "traces": ["ESC-DOM-52"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "absent": true },
    { "kind": "column.maxLength", "element": "order-description", "absent": true }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Infrastructure/Persistence/OrderConfiguration.cs:11", "messageContains": "Orders" },
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Infrastructure/Persistence/OrderConfiguration.cs:15", "messageContains": "200" },
    { "rule": "claim/table.name", "count": 0 },
    { "rule": "claim/column.maxLength", "count": 0 }
  ]
}
```

## modelled-values

The model sets both values. Each is a claim now, verified where the code states it, and code a claim read is located, so the rule reports nothing.

```json case
{ "phase": 4, "base": "on", "kinds": ["table.name", "column.maxLength"], "cases": ["optin/unmodelled.value"], "traces": ["ESC-DOM-52"], "requirements": ["R2", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" },
                "stereotypes": { "Text Constraints": { "MaxLength": 200 } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "verified", "expected": "Orders", "actual": "Orders",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" },
    { "kind": "column.maxLength", "element": "order-description", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:15" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "count": 0 }
  ]
}
```

## convention-is-silence

The rule is on, but the configuration states neither value: EF Core would name the table after the `DbSet` and leave the column unbounded, by convention. A convention is silence, not a value the code states (decision 4), so there is nothing to report.

```json case
{ "phase": 4, "base": "on", "cases": ["optin/unmodelled.value"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
  "findings": [
    { "rule": "optin/unmodelled.value", "count": 0, "note": "Nothing explicit to report" }
  ]
}
```

## excepted-by-type-member

The rule is on, and an `Exceptions` row names `Order.Description`, with a reason. Its max length isn't reported; the table name, which the row doesn't name, still is. The exception matched, so it isn't stale.

```json case
{ "phase": 4, "base": "off-by-default", "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.value", "Match": "Order.Description", "Reason": "The length guards a legacy column and is due to be removed" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "subject": "src/Infrastructure/Persistence/OrderConfiguration.cs:15", "count": 0, "note": "Excepted" },
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Infrastructure/Persistence/OrderConfiguration.cs:11", "messageContains": "Orders" },
    { "rule": "optin/staleException", "count": 0 }
  ]
}
```

## out-of-scope

The rule is on, but the configuration sits in a folder a scope row excludes. Opt-in rules report only in-scope code (R49), and the excluded file is never read (R22), so nothing is reported.

```json case
{ "phase": 4, "base": "off-by-default", "cases": ["optin/unmodelled.value", "scope/gate"], "requirements": ["R22", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/Infrastructure/**", "Scope": "Out", "Reason": "The persistence layer is checked in a later adoption step" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "count": 0 }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Infrastructure/**"] }
}
```

## key-generation-where-it-applies

Key generation is a Primary Key's Data source, so it is probed only on a primary key, and a generation that is EF Core's own for the key is what the Default Data source means. `Order.Id`, the one `int` key, is generated on add by convention, and `ValueGeneratedOnAdd()` only says so. `Line.Id` isn't a key, as `HasKey` names `OrderId` and `LineNo`: its identity can't be modelled, so it is no drift. `Ticket.Code`, a `Guid` key EF Core would generate, is `ValueGeneratedNever()`: the code supplies it and the model leaves its Data source Default, which is reported.

```json case
{ "phase": 4, "kinds": ["key.generated"], "cases": ["optin/unmodelled.value"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": { "Data source": "Default" } } }
            ] },
          { "id": "line", "type": "Class", "name": "Line",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "line-order-id", "type": "Attribute", "name": "OrderId", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": { "Data source": "Default" } } },
              { "id": "line-line-no", "type": "Attribute", "name": "LineNo", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": { "Data source": "Default" } } },
              { "id": "line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] },
          { "id": "ticket", "type": "Class", "name": "Ticket",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "ticket-code", "type": "Attribute", "name": "Code", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": { "Data source": "Default" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/Entities.cs
namespace Shop.Domain;

public partial class Order
{
    public int Id { get; set; }
}

public partial class Line
{
    public int OrderId { get; set; }

    public int LineNo { get; set; }

    public int Id { get; set; }
}

public partial class Ticket
{
    public Guid Code { get; set; }
}
```

```csharp file src/Data/ShopContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Data;

public class ShopContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Line>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.LineNo });
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).ValueGeneratedNever();
        });
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Data/ShopContext.cs:24", "messageContains": "`Ticket.Code` key generation: the code states `User supplied`" },
    { "rule": "optin/unmodelled.value", "messageContains": "Order.Id", "count": 0, "note": "EF Core's own generation for an int key" },
    { "rule": "optin/unmodelled.value", "messageContains": "Line.Id", "count": 0, "note": "Not a key: its identity can't be modelled" }
  ]
}
```

## store-type-stated-by-constraints

A database-first configuration writes every column's store type. `Description`'s `varchar(50)` is what its Text Constraints state, VARCHAR and 50, and `Total`'s `decimal(18, 2)` what its Decimal Constraints state, so neither is a column type the model leaves unset. `Discount`'s `decimal(5, 2)` has no Decimal Constraints: the model leaves it unset, and it is reported once, as its column type, not again as a precision and a scale.

```json case
{ "phase": 4, "kinds": ["column.type", "column.precision", "column.scale"], "cases": ["optin/unmodelled.value"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" },
                "stereotypes": { "Text Constraints": { "SQL Data Type": "VARCHAR", "MaxLength": 50 } } },
              { "id": "order-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" },
                "stereotypes": { "Decimal Constraints": { "Precision": 18, "Scale": 2 } } },
              { "id": "order-discount", "type": "Attribute", "name": "Discount", "typeRef": { "type": "decimal" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public partial class Order
{
    public int Id { get; set; }

    public string Description { get; set; }

    public decimal Total { get; set; }

    public decimal Discount { get; set; }
}
```

```csharp file src/Data/ShopContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Data;

public class ShopContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(50).IsUnicode(false).HasColumnType("varchar(50)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
        });
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Data/ShopContext.cs:18", "messageContains": "`Order.Discount` column type: the code states `decimal(5, 2)`" },
    { "rule": "optin/unmodelled.value", "messageContains": "Order.Discount", "count": 1, "note": "One finding, not three" },
    { "rule": "optin/unmodelled.value", "messageContains": "Order.Description", "count": 0 },
    { "rule": "optin/unmodelled.value", "messageContains": "Order.Total", "count": 0 }
  ]
}
```
