# column.default

The Default Constraint stereotype's Value on an attribute of a persisted class, against the database default the code states: EF Core's `HasDefaultValueSql` (when the stereotype's `Treat as SQL Expression` is on) or `HasDefaultValue`. `column.default` is `stated: whenSet`: a column the code gives no default has none to compare, and the CLR default of the property's type is never read in its place (decision 4). A conversion-only kind: Common.Drift's Domain recipes check it, and no eShopCustom or Java check does (claim-catalog.md).

## verified

`Order.OrderDate` defaults to the database's UTC clock, and the configuration says `HasDefaultValueSql("getutcdate()")`.

```json case
{ "phase": 4, "kinds": ["column.default"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
                "stereotypes": { "Default Constraint": { "Value": "getutcdate()", "Treat as SQL Expression": true } } }
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
        builder.Property(o => o.OrderDate).HasDefaultValueSql("getutcdate()");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.default", "element": "order-order-date", "outcome": "verified", "expected": "getutcdate()", "actual": "getutcdate()",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The configuration's default is another SQL expression. The finding names the attribute, both values and the line, and says the Default Constraint stereotype's Value states the model's side (R11, R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.default"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property(o => o.OrderDate).HasDefaultValueSql("sysutcdatetime()");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.default", "element": "order-order-date", "outcome": "contradicted", "expected": "getutcdate()", "actual": "sysutcdatetime()",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.default", "severity": "error", "subject": "order-order-date", "messageContains": "Default Constraint" }
  ]
}
```

## contradicted-not-stated

The model gives the column a default, and the configuration gives it none. The modelled default is contradicted, "not stated" (decision 4): the column has no default in the database.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.default"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.default", "element": "order-order-date", "outcome": "contradicted", "expected": "getutcdate()", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.default", "severity": "error", "subject": "order-order-date", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The attribute has no Default Constraint, and the code says `HasDefaultValueSql("getutcdate()")`. An unset `whenSet` value is no claim, so nothing is checked or reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.default"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.default", "element": "order-order-date", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.default", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## no-rule

Only the core and Domain packs are active, so nothing reads a column default. The modelled default is can't verify, `noRule`, one error for the claim (conversion.md, semantic change 6), never a silent skip (principle 2).

```json case
{ "phase": 1, "base": "verified", "kinds": ["column.default"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "column.default", "element": "order-order-date", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/column.default", "severity": "error", "subject": "order-order-date", "messageContains": "no rule" }
  ]
}
```
