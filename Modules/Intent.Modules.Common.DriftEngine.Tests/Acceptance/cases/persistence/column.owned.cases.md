# column.owned

Whether an attribute of a persisted class whose type is a Value Object is mapped as a value: `true` in the model for every such attribute. EF Core states it with `OwnsOne(x => x.Attr)`, a complex type (`ComplexProperty`) or a value converter (`Property(x => x.Attr).HasConversion(...)`): a Value Object type states only that the attribute is a value, and the designer has no construct choosing among them, so any of them verifies (user decision, 2026-11-26). eShopCustom checked it as `persistence.owned` (ESC-DOM-38), which asked for `OwnsOne` alone. `column.owned` is `stated: always`: EF Core never maps a class as a value unless the code says so, so a configuration with none of them really states `false`, and the reading from silence is honest (R3).

## verified

eShopCustom's `Order` keeps its shipping address as an `Address` value object, and the configuration says `OwnsOne(o => o.Address)`.

```json case
{ "phase": 4, "kinds": ["column.owned"], "traces": ["ESC-DOM-38"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "address", "type": "Value Object", "name": "Address",
            "children": [
              { "id": "address-street", "type": "Attribute", "name": "Street", "typeRef": { "type": "string" } },
              { "id": "address-city", "type": "Attribute", "name": "City", "typeRef": { "type": "string" } }
            ] },
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-address", "type": "Attribute", "name": "Address", "typeRef": { "element": "address" } }
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

```csharp file src/Domain/Address.cs
namespace Shop.Domain;

public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public Address Address { get; private set; }
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
        builder.OwnsOne(o => o.Address);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.owned", "element": "order-address", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted-from-silence

The configuration doesn't own `Address`. EF Core would then treat `Address` as an entity of its own, which it isn't, so the code states `false`, read from silence (R3), and the claim is contradicted: the drift `persistence.owned` caught.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.owned"], "cases": ["engine/silence"], "traces": ["ESC-DOM-38"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.owned", "element": "order-address", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/column.owned", "severity": "error", "subject": "order-address" }
  ]
}
```

## verified-complex-property

The configuration maps `Address` with `ComplexProperty`, EF Core 8's complex type: not an owned type, but a value all the same, which is what the model states.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.owned"], "requirements": ["R2", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.ComplexProperty(o => o.Address);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.owned", "element": "order-address", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## verified-value-conversion

A strongly typed identifier kept in one column: `Order.CustomerId` is the Value Object `CustomerId`, which the configuration stores through a value converter, `HasConversion(v => v.Value, v => new CustomerId(v))`. The value object is mapped as a value, so the claim holds.

```json case
{ "phase": 4, "kinds": ["column.owned"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "customer-id", "type": "Value Object", "name": "CustomerId",
            "children": [
              { "id": "customer-id-value", "type": "Attribute", "name": "Value", "typeRef": { "type": "guid" } }
            ] },
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-customer-id", "type": "Attribute", "name": "CustomerId", "typeRef": { "element": "customer-id" } }
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

```csharp file src/Domain/CustomerId.cs
namespace Shop.Domain;

public readonly struct CustomerId
{
    public CustomerId(Guid value) => Value = value;

    public Guid Value { get; }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public CustomerId CustomerId { get; private set; }
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
        builder.Property(o => o.CustomerId)
            .HasConversion(v => v.Value, v => new CustomerId(v));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.owned", "element": "order-customer-id", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:12" }
  ]
}
```

## blocked-when-member-missing

`Order` has no `Address` property. `member.exists` is contradicted, and whether it is owned, which depends on it, is can't verify, `blocked`, with no finding of its own (R5).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.owned", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-address", "outcome": "contradicted" },
    { "kind": "column.owned", "element": "order-address", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-address" },
    { "rule": "claim/column.owned", "count": 0 }
  ]
}
```
