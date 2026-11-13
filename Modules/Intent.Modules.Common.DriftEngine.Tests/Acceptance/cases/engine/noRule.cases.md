# engine/noRule

A claim exists because the model states it; whether anything can check it depends on the active packs. Vocabulary and rules ship separately: the Domain pack says what the Table stereotype states, and the EF Core pack says where EF Core code states it. With no active rule for a claim's kind on its element, the claim is can't verify, `noRule` (outcome step 1), an error like every can't verify, so a model never looks checked when nothing read it (R5).

## vocabulary-without-rule

`Order`'s Table stereotype names its table and schema, and the code configures both. Only the core and Domain packs are active, so the table claims exist but no rule reads them: each is can't verify, `noRule`, and its finding says no rule in the active packs reads it. Listing `intent.efcore` in the model's Drift Check Packs is the fix. The structure claims, which the core pack reads, are verified as usual.

```json case
{ "phase": 1, "cases": ["engine/noRule"], "requirements": ["R1", "R5"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": { "Name": "Orders", "Schema": "sales" } },
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

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }
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

        builder.HasKey(x => x.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "cantVerify", "reason": "noRule" },
    { "kind": "table.schema", "element": "order", "outcome": "cantVerify", "reason": "noRule" },
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "claim/table.schema", "severity": "error", "subject": "order", "messageContains": "no rule" }
  ]
}
```
