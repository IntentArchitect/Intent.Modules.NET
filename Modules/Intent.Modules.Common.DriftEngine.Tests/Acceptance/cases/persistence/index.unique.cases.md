# index.unique

Whether each index the Index stereotypes give is unique (the `IsUnique` of its first attribute's Index), against the index's `IsUnique()` in the code. Named and unnamed indexes are both claims (decision 9), told apart by `key`, the index's name or its columns. `index.unique` is `stated: always`: an index with no `IsUnique()` states that it isn't unique, so a non-unique index verifies from silence and a unique one is contradicted by it (ESC-DOM-48, both ways).

## verified

An unnamed unique index on `Buyer.IdentityGuid`, and `HasIndex(b => b.IdentityGuid).IsUnique()` in the configuration (ESC-DOM-48).

```json case
{ "phase": 4, "kinds": ["index.unique"], "traces": ["ESC-DOM-48"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "buyer-identity-guid", "type": "Attribute", "name": "IdentityGuid", "typeRef": { "type": "string" },
                "stereotypes": { "Index": { "IsUnique": true } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }

    public string IdentityGuid { get; private set; } = string.Empty;
}
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasIndex(b => b.IdentityGuid).IsUnique();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "index.unique", "element": "buyer", "key": "IdentityGuid", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ]
}
```

## contradicted-not-unique-in-code

The index is declared, in the string form `HasIndex("IdentityGuid")`, but without `IsUnique()`. The index saying nothing states that it isn't unique, so the modelled unique index is contradicted from silence (ESC-DOM-48). The rule found the index and no read case matched, so the reading is its `default`, located at the index it found: the `HasIndex` chain.

```json case
{ "phase": 4, "base": "verified", "kinds": ["index.unique"], "traces": ["ESC-DOM-48"], "requirements": ["R1", "R3", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasIndex("IdentityGuid");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "index.unique", "element": "buyer", "key": "IdentityGuid", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true,
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/index.unique", "severity": "error", "subject": "buyer", "messageContains": "code `false`" }
  ]
}
```

## verified-not-unique

The model's index isn't unique, and the code's `HasIndex("IdentityGuid")` has no `IsUnique()`. Silence states it, so the claim is verified from silence (R3).

```json case
{ "phase": 4, "base": "contradicted-not-unique-in-code", "kinds": ["index.unique"], "traces": ["ESC-DOM-48"], "requirements": ["R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "buyer-identity-guid", "type": "Attribute", "name": "IdentityGuid", "typeRef": { "type": "string" },
                "stereotypes": { "Index": { "IsUnique": false } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "index.unique", "element": "buyer", "key": "IdentityGuid", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true,
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ]
}
```

## contradicted-unique-in-code

The other way round: the model's index isn't unique, and the code says `IsUnique()`. The claim is contradicted (ESC-DOM-48, code to model).

```json case
{ "phase": 4, "base": "verified", "kinds": ["index.unique"], "traces": ["ESC-DOM-48"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "buyer-identity-guid", "type": "Attribute", "name": "IdentityGuid", "typeRef": { "type": "string" },
                "stereotypes": { "Index": { "IsUnique": false } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "index.unique", "element": "buyer", "key": "IdentityGuid", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/index.unique", "severity": "error", "subject": "buyer", "messageContains": "code `true`" }
  ]
}
```

## cant-verify-ambiguous

The index is declared twice: in `BuyerConfiguration` and again in the context's `OnModelCreating`. The find ends at two places where one is expected, so the claim is can't verify, `ambiguous`, naming both.

```json case
{ "phase": 4, "base": "verified", "kinds": ["index.unique"], "cases": ["engine/ambiguous"], "traces": ["ESC-DOM-48"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class OrderingContext : DbContext
{
    public DbSet<Buyer> Buyers => Set<Buyer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingContext).Assembly);

        modelBuilder.Entity<Buyer>().HasIndex(b => b.IdentityGuid).IsUnique();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "index.unique", "element": "buyer", "key": "IdentityGuid", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/index.unique", "severity": "error", "subject": "buyer", "messageContains": "src/Infrastructure/Persistence/OrderingContext.cs:14" }
  ]
}
```
