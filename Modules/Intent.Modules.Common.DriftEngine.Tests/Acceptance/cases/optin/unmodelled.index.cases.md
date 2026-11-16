# optin/unmodelled.index

The EF Core extension's opt-in rule that reports an index the configuration declares, a `HasIndex` in checked code, that no index claim located: one no Index element or Index stereotype describes (rules.md, "Opt-in must be modelled rules"). Common.Drift's `own` entry for indexes caught these. It is off until the Root Folder's `Drift Check` lists it; an index is named by its database name, else by its entity and columns.

## on

`Buyer`'s `IX_Buyers_IdentityGuid` is modelled, as the Index stereotype's UniqueKey, and located by its claims. `IX_Buyers_Name` isn't modelled, and neither is the unnamed index on `Email`: each is reported where it is declared.

```json case
{ "phase": 4, "kinds": ["index.columns"], "cases": ["optin/unmodelled.index"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
                "stereotypes": { "Index": { "UniqueKey": "IX_Buyers_IdentityGuid", "Order": 1, "IsUnique": true } } },
              { "id": "buyer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } },
              { "id": "buyer-email", "type": "Attribute", "name": "Email", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.index" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }

    public string IdentityGuid { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }
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
        builder.HasIndex(b => b.IdentityGuid).HasDatabaseName("IX_Buyers_IdentityGuid").IsUnique();
        builder.HasIndex(b => b.Name, "IX_Buyers_Name");
        builder.HasIndex(b => b.Email);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.index", "severity": "error", "subject": "src/Infrastructure/Persistence/BuyerConfiguration.cs:12",
      "messageContains": "The index `IX_Buyers_Name` on `Buyer` isn't in the model." },
    { "rule": "optin/unmodelled.index", "severity": "error", "subject": "src/Infrastructure/Persistence/BuyerConfiguration.cs:13", "messageContains": "`Buyer(Email)`" },
    { "rule": "optin/unmodelled.index", "subject": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11", "count": 0, "note": "Modelled, so located" }
  ]
}
```

## off-by-default

The same model and code with no `Opt-in Rules` row: nothing is reported.

```json case
{ "phase": 4, "base": "on", "kinds": ["index.columns"], "cases": ["optin/unmodelled.index"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
                "stereotypes": { "Index": { "UniqueKey": "IX_Buyers_IdentityGuid", "Order": 1, "IsUnique": true } } },
              { "id": "buyer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } },
              { "id": "buyer-email", "type": "Attribute", "name": "Email", "typeRef": { "type": "string" } }
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
  "findings": [
    { "rule": "optin/unmodelled.index", "count": 0 }
  ]
}
```
