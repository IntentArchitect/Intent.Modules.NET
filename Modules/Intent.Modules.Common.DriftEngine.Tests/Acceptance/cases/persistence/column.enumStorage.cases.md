# column.enumStorage

How an enum attribute of a persisted class is stored: eShopCustom's own Enum Storage stereotype's As, `String` or `Number`. EF Core states it with `HasConversion<string>()` (`String`) or `HasConversion<int>()` (`Number`). eShopCustom checked it in ESC-DOM-51. No stock pack knows the module's stereotype, so these cases give its vocabulary in a house pack, as the codebase would; the stock EF Core pack reads the conversion. `column.enumStorage` is `stated: whenSet`: EF Core stores an enum as its number by default, but that default is never read in place of a modelled value (decision 4).

## verified

eShopCustom stores `Order.OrderStatus` as text: Enum Storage As `String`, and the configuration says `HasConversion<string>()` (ESC-DOM-51).

```json case
{ "phase": 4, "kinds": ["column.enumStorage"], "traces": ["ESC-DOM-51"], "requirements": ["R2", "R29"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
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
          "kind": "column.enumStorage",
          "for": { "element": "Attribute", "where": "parent.hasStereotype('Table') && hasStereotype('Enum Storage')" },
          "value": "stereotype('Enum Storage').As",
          "bindings": { "Entity": "parent.name", "Attribute": "name" },
          "own": ["Attribute"],
          "dependsOn": "member.exists",
          "source": "the Enum Storage stereotype's As"
        },
        {
          "kind": "column.maxLength",
          "for": { "element": "Attribute", "where": "parent.hasStereotype('Table') && hasStereotype('Enum Storage')" },
          "value": "stereotype('Enum Storage')['Max Length']",
          "bindings": { "Entity": "parent.name", "Attribute": "name" },
          "own": ["Attribute"],
          "dependsOn": "member.exists",
          "source": "the Enum Storage stereotype's Max Length"
        }
      ]
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
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-submitted", "type": "Enum-Literal", "name": "Submitted", "value": 1 },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid", "value": 2 },
              { "id": "order-status-shipped", "type": "Enum-Literal", "name": "Shipped", "value": 3 }
            ] },
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-order-status", "type": "Attribute", "name": "OrderStatus", "typeRef": { "element": "order-status" },
                "stereotypes": { "Enum Storage": { "As": "String" } } }
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

```csharp file src/Domain/OrderStatus.cs
namespace Shop.Domain;

public enum OrderStatus
{
    Submitted = 1,
    Paid = 2,
    Shipped = 3
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public OrderStatus OrderStatus { get; private set; }
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
        builder.Property(o => o.OrderStatus).HasConversion<string>();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.enumStorage", "element": "order-order-status", "outcome": "verified", "expected": "String", "actual": "String",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The configuration converts the enum to `int`: stored as a number, where the model says text. The finding names the Enum Storage stereotype's As as the model's side (R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.enumStorage"], "traces": ["ESC-DOM-51"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
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
        builder.Property(o => o.OrderStatus).HasConversion<int>();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.enumStorage", "element": "order-order-status", "outcome": "contradicted", "expected": "String", "actual": "Number",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.enumStorage", "severity": "error", "subject": "order-order-status", "messageContains": "Enum Storage" }
  ]
}
```

## contradicted-not-stated

The model says `String`, and the configuration has no conversion, so EF Core stores the enum's number. The code doesn't state the modelled storage, so the claim is contradicted, "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.enumStorage"], "traces": ["ESC-DOM-51"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
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
        builder.Property(o => o.OrderStatus).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.enumStorage", "element": "order-order-status", "outcome": "contradicted", "expected": "String", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/column.enumStorage", "severity": "error", "subject": "order-order-status", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The attribute has no Enum Storage stereotype, and the configuration says `HasConversion<string>()`. Nothing in the model states the storage, so there is no claim and nothing is reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.enumStorage"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-submitted", "type": "Enum-Literal", "name": "Submitted", "value": 1 },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid", "value": 2 },
              { "id": "order-status-shipped", "type": "Enum-Literal", "name": "Shipped", "value": 3 }
            ] },
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-order-status", "type": "Attribute", "name": "OrderStatus", "typeRef": { "element": "order-status" } }
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
    { "kind": "column.enumStorage", "element": "order-order-status", "absent": true }
  ],
  "findings": [
    { "rule": "claim/column.enumStorage", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## unreadable

The configuration converts with a custom `ValueConverter`. `HasConversion(OrderStatusConverter.Instance)` states a storage, but which one is in the converter's code, not in a form the rule reads, so the claim is can't verify, `unreadable`, naming the line (R5).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.enumStorage"], "cases": ["engine/unreadable"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.eshopcustom"], "language": "csharp" }
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
        builder.Property(o => o.OrderStatus).HasConversion(OrderStatusConverter.Instance);
    }
}
```

```csharp file src/Infrastructure/Persistence/OrderStatusConverter.cs
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderStatusConverter : ValueConverter<OrderStatus, string>
{
    public static readonly OrderStatusConverter Instance = new();

    private OrderStatusConverter() : base(s => s.ToString().ToUpperInvariant(), s => Enum.Parse<OrderStatus>(s, true)) { }
}
```

```json expect
{
  "claims": [
    { "kind": "column.enumStorage", "element": "order-order-status", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.enumStorage", "severity": "error", "subject": "order-order-status", "messageContains": "can't read" }
  ]
}
```
