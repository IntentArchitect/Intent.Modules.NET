# column.required

Whether a column of a persisted class is required (`NOT NULL`): `true` when the attribute's type isn't nullable. EF Core states it with `IsRequired()` or `[Required]`, and also through the property's type: a non-nullable value type is always required, and with nullable reference types on, a `string` without `?` is too. ESC-DOM-35 checked this with EF Core's semantics, and this kind takes its column half. `column.required` is `stated: always`, but the code's silence states a value only where it really decides one: where it depends on a nullable setting the reader can't establish, the claim is can't verify, `unresolved` (R35), never a guess.

## verified

`Order.Description` is a non-nullable `string` in the model, and the configuration says `IsRequired()`.

```json case
{ "phase": 4, "kinds": ["column.required"], "traces": ["ESC-DOM-35"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } },
              { "id": "order-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } }
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

    public int Units { get; private set; }
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
        builder.Property(o => o.Description).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.required", "element": "order-description", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## contradicted

The model makes `Description` nullable, and the configuration makes the column required. The finding names the attribute's nullability as the model's side (R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.required"], "traces": ["ESC-DOM-35"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string", "nullable": true } },
              { "id": "order-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } }
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
    { "kind": "column.required", "element": "order-description", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/column.required", "severity": "error", "subject": "order-description" }
  ]
}
```

## verified-value-type

`Units` is a non-nullable `int` in the model, and the configuration says nothing about it. An `int` property can't hold null, so EF Core's column is required whatever the configuration says: the code states `true` by its type, and the claim is verified with no `IsRequired()` (ESC-DOM-35's "a value type needs `?`").

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.required"], "traces": ["ESC-DOM-35"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "column.required", "element": "order-units", "outcome": "verified", "expected": "true", "actual": "true" }
  ]
}
```

## contradicted-value-type

The model makes `Units` nullable, and the code declares it `int`, which can't be. The column is required, so the claim is contradicted with no configuration involved.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.required"], "traces": ["ESC-DOM-35"], "requirements": ["R1"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } },
              { "id": "order-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int", "nullable": true } }
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
    { "kind": "column.required", "element": "order-units", "outcome": "contradicted", "expected": "false", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/column.required", "severity": "error", "subject": "order-units" }
  ]
}
```

## unresolved

No `IsRequired()` or `[Required]` on `Description`, and no project file or `#nullable` directive says whether nullable reference types are on. With them on, `string` makes the column required; with them off, it doesn't. The C# reader reports the property's nullability as `unknown` outside a known nullable context (code-reader.md), so the claim is can't verify, `unresolved` (R35), rather than either guess.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.required"], "cases": ["engine/unresolved"], "requirements": ["R5", "R35"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
    { "kind": "column.required", "element": "order-description", "outcome": "cantVerify", "reason": "unresolved" }
  ],
  "findings": [
    { "rule": "claim/column.required", "severity": "error", "subject": "order-description" }
  ]
}
```

## verified-nullable-enabled

The same code, with the domain project's file turning nullable reference types on. `string` without `?` is now non-nullable, so EF Core makes the column required, and the claim is verified from the property's type.

```json case
{ "phase": 4, "base": "unresolved", "kinds": ["column.required"], "traces": ["ESC-DOM-35"], "requirements": ["R2", "R35"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

```json expect
{
  "claims": [
    { "kind": "column.required", "element": "order-description", "outcome": "verified", "expected": "true", "actual": "true" }
  ]
}
```
