# engine/readerFailed

Readers read files; they never build, restore or load the codebase (R18), so there is no workspace to fail to load. What can fail is reading one file. A file its reader fails on, such as a project file that won't parse, gives `readerFailed` for each claim that needed it, plus one `engine/readerFailed` error for the file, in the `DriftEngine` Area under Code (R40). A file with syntax errors is different: it is still read, and it gets no finding of its own. Nodes inside an error's span are `incomplete`, and so is a node containing a missing token the parser inserted; a reading taken from an incomplete node gives `readerFailed` with the reader's diagnostic, rather than a value that may be wrong (code-reader.md, Failures). A file that isn't text can't be written as a fixture, so these cases show reader failures with code that doesn't parse.

eShopCustom's Domain, Services, Front End and Architecture modules each reported the reader failing to start, or a file failing to parse, as one error (`domain.read`, `services.read`, `frontend.read`, `architecture.read`).

## syntax-error-in-read-code

Someone is part-way through editing `OrderConfiguration`: the `ToTable` call has lost its closing parenthesis. The parser inserts the missing `)` as a zero-width token, so the call contains it and is `incomplete`, even though nothing after `"Orders"` is skipped. The file is still read, but the call the table name rule reads is incomplete, so the claim is can't verify, `readerFailed`, naming the file. The file itself gets no `engine/readerFailed` finding: the claim it fails is what's reported. `Order` itself is in a clean file and is verified: one broken file doesn't stop the rest of the run.

```json case
{ "phase": 4, "kinds": ["table.name"], "cases": ["engine/readerFailed"], "traces": ["ESC-DOM-05", "ESC-FE-06"], "requirements": ["R5", "R40"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.ToTable("Orders";

        builder.HasKey(x => x.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "cantVerify", "reason": "readerFailed" },
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "claim/table.name", "severity": "error", "subject": "order", "messageContains": "OrderConfiguration.cs" },
    { "rule": "engine/readerFailed", "count": 0, "note": "A file with syntax errors gets no finding of its own." }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs", "src/Infrastructure/Persistence/OrderConfiguration.cs"] }
}
```

## project-file-unparseable

A bad merge left the domain project's `.csproj` with an `<ItemGroup>` that never closes. A project file that won't parse is a file its reader fails on, so its settings, the nullable context among them, are unknown. That is one `engine/readerFailed` error, for the project file, however many claims its files carry. The `.cs` files are still read: `Customer` is verified. Its `Name` property's `member.nullable` claim needed the project's settings, so it is can't verify, `readerFailed`, naming the project file, not a guess and not `unresolved`: a file the project governs does have a nullable context, the reader just couldn't read it.

```json case
{ "phase": 1, "cases": ["engine/readerFailed"], "traces": ["ESC-SVC-07", "ESC-ARC-01"], "requirements": ["R5", "R40"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "customer", "type": "Class", "name": "Customer",
            "children": [
              { "id": "customer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "customer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } }
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

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
  <ItemGroup>

</Project>
```

```csharp file src/Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "customer", "outcome": "verified", "at": "src/Domain/Customer.cs:3" },
    { "kind": "member.nullable", "element": "customer-name", "outcome": "cantVerify", "reason": "readerFailed" }
  ],
  "findings": [
    { "rule": "engine/readerFailed", "severity": "error", "messageContains": "Shop.Domain.csproj" },
    { "rule": "claim/member.nullable", "severity": "error", "subject": "customer-name", "messageContains": "Shop.Domain.csproj" }
  ],
  "filesRead": { "include": ["src/Domain/Customer.cs"] }
}
```
