# engine/silence

Code that says nothing says something only for some kinds (R3, R4). A kind `stated: always` is stated by the code even by silence: a class with no `abstract` modifier says it isn't abstract. A rule's `default` (found, but no read case matched) or `absent` (nothing found), or a structure rule's reading of a missing modifier, is then compared like any value, and the reading is marked as from silence so recipe validation can count it apart. A kind `stated: whenSet` never takes a value from silence: the model states such a value only where the code states it explicitly (decision 4), so code that says nothing contradicts it, and a recipe giving one a `default` or `absent` is refused as a configuration error.

## always-verified-from-silence

The model says `Order` isn't abstract, and its class has no `abstract` modifier. The claim is verified, from silence.

```json case
{ "phase": 1, "kinds": ["type.abstract"], "cases": ["engine/silence"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
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

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "order", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true }
  ]
}
```

## always-contradicted-from-silence

The model makes `Payment` abstract, the base of its card and bank transfer payments, but its class has lost the modifier. Silence states `false`, and the claim is contradicted, from silence.

```json case
{ "phase": 1, "kinds": ["type.abstract"], "cases": ["engine/silence"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "payment", "type": "Class", "name": "Payment", "abstract": true,
            "children": [
              { "id": "payment-amount", "type": "Attribute", "name": "Amount", "typeRef": { "type": "decimal" } }
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

```csharp file src/Domain/Payments/Payment.cs
namespace Shop.Domain.Payments;

public class Payment
{
    public decimal Amount { get; protected set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "payment", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/type.abstract", "severity": "error", "subject": "payment" }
  ]
}
```

## when-set-never-from-silence

The model gives each `OrderStatus` literal a value. The code gives `Draft` and `Submitted` theirs, but leaves `Paid` to C#'s numbering. C# would make it `2`, the model's value, but the code doesn't state it, so the claim is contradicted with the code side "not stated", and not from silence. `Draft` and `Submitted` are verified from what the code states.

```json case
{ "phase": 1, "kinds": ["enum.literal.value"], "cases": ["engine/silence"], "requirements": ["R2", "R4"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-status-draft", "type": "Enum-Literal", "name": "Draft", "value": "0" },
              { "id": "order-status-submitted", "type": "Enum-Literal", "name": "Submitted", "value": "1" },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid", "value": "2" }
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
    Draft = 0,
    Submitted = 1,
    Paid,
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literal.value", "element": "order-status-draft", "outcome": "verified", "fromSilence": false, "at": "src/Domain/OrderStatus.cs:5" },
    { "kind": "enum.literal.value", "element": "order-status-submitted", "outcome": "verified", "fromSilence": false, "at": "src/Domain/OrderStatus.cs:6" },
    { "kind": "enum.literal.value", "element": "order-status-paid", "outcome": "contradicted", "expected": "2", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/enum.literal.value", "severity": "error", "subject": "order-status-paid", "messageContains": "doesn't state" }
  ]
}
```

## silence-refused-for-when-set

A house pack's recipe for table names falls back to the class name when there is no `ToTable`: EF Core's convention, as a `default`. `table.name` is `whenSet`, so the recipe is refused with a `config/silenceForWhenSet` error naming it, before anything is compared. A convention's name may well be right, but the model states a table name only where the code does.

```json case
{ "phase": 1, "cases": ["engine/silence"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "house.shop"], "language": "csharp" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "rules": {
    "Domain": {
      "facts": {
        "table.name": {
          "find": "class:{Entity}Configuration > method:Configure",
          "read": [{ "match": "ToTable\\(\"([^\"]+)\"" }],
          "default": "{Entity}",
          "note": "No ToTable: EF Core names the table after the class."
        }
      }
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
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": { "Name": "Order" } },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": {} } }
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
        builder.HasKey(x => x.Id);
    }
}
```

```json expect
{
  "config": [
    { "rule": "config/silenceForWhenSet", "severity": "error", "messageContains": "table.name" }
  ]
}
```
