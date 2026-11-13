# entity.persisted

Whether a persisted Domain class (one with the Table or View stereotype) really reaches the database: its EF Core configuration is applied in the DbContext's `OnModelCreating`, by `ApplyConfiguration(new XConfiguration())` or by `ApplyConfigurationsFromAssembly` over the assembly that holds it. eShopCustom checked this as `persistence.applied` (ESC-DOM-10). `entity.persisted` is `stated: always`: a configuration nothing applies is a statement that the class isn't mapped, so a `false` reading from silence is honest (principle 3).

## verified

`OrderingContext` applies `OrderConfiguration` explicitly.

```json case
{ "phase": 4, "kinds": ["entity.persisted"], "traces": ["ESC-DOM-10"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
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

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/OrderingContext.cs:12" }
  ]
}
```

## verified-from-assembly

The context applies every configuration in its own assembly with `ApplyConfigurationsFromAssembly`, the other form ESC-DOM-10 accepts. `OrderConfiguration` is in the same project as the context, so the call applies it.

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted"], "traces": ["ESC-DOM-10"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```xml file src/Infrastructure/Shop.Infrastructure.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingContext).Assembly);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Infrastructure/OrderingContext.cs:11" }
  ]
}
```

## contradicted-not-applied

The context applies another class's configuration and never `OrderConfiguration`, and nothing else maps `Order`. The configuration is dead code and `Order` isn't in EF Core's model. The code's silence states `false`, so the claim is contradicted from silence (R3), the drift eShopCustom's `persistence.applied` caught.

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted"], "cases": ["engine/silence"], "traces": ["ESC-DOM-10"], "requirements": ["R1", "R3", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BuyerConfiguration());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "severity": "error", "subject": "order" }
  ]
}
```

## reader-failed

The call that applies `OrderConfiguration` is missing its closing parenthesis. The C# reader still reads the file, but the node the rule would read lies in the error's span and is `incomplete`, so the claim is can't verify, `readerFailed`, with the reader's diagnostic, rather than a value that may be wrong (R40, code-reader.md).

```json case
{ "phase": 4, "base": "verified", "kinds": ["entity.persisted"], "cases": ["engine/readerFailed"], "requirements": ["R5", "R40"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/OrderingContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public class OrderingContext : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order", "outcome": "cantVerify", "reason": "readerFailed" }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "severity": "error", "subject": "order" }
  ]
}
```
