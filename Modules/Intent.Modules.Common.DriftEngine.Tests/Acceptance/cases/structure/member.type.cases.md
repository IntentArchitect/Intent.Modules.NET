# member.type

A modelled attribute, DTO field, Domain Event property, component property, event emitter, Model Definition property or Proto Message field has the type its type reference states. The `type` comparer maps the model's type through the language profile's `types` and `collections`, then compares it with the member's type ref (rules.md, Language profiles): model `guid` is C# `Guid` and Java `UUID`, and model `OrderItem[]` is any collection form the profile lists. A model type that is a modelled element matches the code type that element is located as. Nullability isn't part of it: `expected` shows the model's type with its `?`, but a nullability-only difference is `member.nullable`'s one finding, never a second from `member.type` (claim-catalog.md, Notes on kinds). It is a `stated: always` kind; a member the code lacks blocks its type claim (R5).

## verified

Each attribute's type maps through the C# profile: `guid` to `Guid`, `decimal` to `decimal`, and a collection of `string` to `List<string>`. `BuyerId`'s `Guid?` matches its nullable `guid` on the type alone; that both sides are nullable is `member.nullable`'s claim.

```json case
{ "phase": 1, "kinds": ["member.type"], "traces": ["ESC-DOM-34"], "requirements": ["R2", "R10", "R41", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "guid", "nullable": true } },
              { "id": "order-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } },
              { "id": "order-tags", "type": "Attribute", "name": "Tags", "typeRef": { "type": "string", "collection": true } }
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

</Project>
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; private set; }

    public Guid? BuyerId { get; private set; }

    public decimal Total { get; private set; }

    public List<string> Tags { get; private set; } = [];
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-id", "outcome": "verified", "expected": "guid", "actual": "Guid", "at": "src/Domain/Order.cs:5" },
    { "kind": "member.type", "element": "order-buyer-id", "outcome": "verified", "expected": "guid?", "actual": "Guid?", "at": "src/Domain/Order.cs:7" },
    { "kind": "member.type", "element": "order-total", "outcome": "verified", "expected": "decimal", "actual": "decimal", "at": "src/Domain/Order.cs:9" },
    { "kind": "member.type", "element": "order-tags", "outcome": "verified", "expected": "string[]", "actual": "List<string>", "at": "src/Domain/Order.cs:11" }
  ],
  "findings": [
    { "rule": "claim/member.type", "count": 0 }
  ]
}
```

## contradicted

`Total` is a `double` in the code. `double` isn't one of the C# profile's names for `decimal`, so the claim is contradicted, and the finding shows both sides (R14).

```json case
{ "phase": 1, "base": "verified", "kinds": ["member.type"], "traces": ["ESC-DOM-34"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; private set; }

    public Guid? BuyerId { get; private set; }

    public double Total { get; private set; }

    public List<string> Tags { get; private set; } = [];
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-total", "outcome": "contradicted", "expected": "decimal", "actual": "double", "at": "src/Domain/Order.cs:9" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "order-total", "messageContains": "code `double`" }
  ]
}
```

## contradicted-collection

The model says `Tags` is a collection of `string`; the code has one `string`. The collection flag is part of the type, so the claim is contradicted.

```json case
{ "phase": 1, "base": "verified", "kinds": ["member.type"], "traces": ["ESC-DOM-34"], "requirements": ["R1", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; private set; }

    public Guid? BuyerId { get; private set; }

    public decimal Total { get; private set; }

    public string Tags { get; private set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-tags", "outcome": "contradicted", "expected": "string[]", "actual": "string", "at": "src/Domain/Order.cs:11" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "order-tags" }
  ]
}
```

## blocked

`Order` has no `Total`. Its `member.exists` claim is contradicted, and its type claim waits on that one: can't verify, `blocked`. A blocked claim isn't a finding of its own; the member's finding counts it (R5).

```json case
{ "phase": 1, "base": "verified", "kinds": ["member.type", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; private set; }

    public Guid? BuyerId { get; private set; }

    public List<string> Tags { get; private set; } = [];
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-total", "outcome": "contradicted" },
    { "kind": "member.type", "element": "order-total", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-total" },
    { "rule": "claim/member.type", "count": 0, "note": "a blocked claim is counted in the finding of the claim it waits on" }
  ]
}
```

## shadow-foreign-key

`BuyerId` has no property on `Order`; EF Core maps it as a shadow property. With the EF Core pack, the attribute's type is the shadow property's type argument: `Property<int?>("BuyerId")` states a nullable `int`.

```json case
{ "phase": 4, "kinds": ["member.type"], "traces": ["ESC-DOM-37"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int", "nullable": true },
                "stereotypes": { "Foreign Key": { "Association": "order-buyer" } } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "0..1", "navigable": true } }
      ]
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
    public Buyer? Buyer { get; private set; }
}
```

```csharp file src/Domain/Buyer.cs
namespace Shop.Domain;

public class Buyer
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
        builder.Property<int?>("BuyerId");

        builder.HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey("BuyerId");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-buyer-id", "outcome": "verified", "expected": "int?", "actual": "int?", "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## shadow-foreign-key-contradicted

The shadow property is `Property<long?>`, and `long` isn't one of the C# profile's names for `int`: contradicted.

```json case
{ "phase": 4, "base": "shadow-foreign-key", "kinds": ["member.type"], "traces": ["ESC-DOM-37"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property<long?>("BuyerId");

        builder.HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey("BuyerId");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-buyer-id", "outcome": "contradicted", "expected": "int?", "actual": "long?", "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "order-buyer-id" }
  ]
}
```

## shadow-foreign-key-nullability

The shadow property is `Property<int>`, a non-nullable `int`, while the model's `BuyerId` is nullable. The type is still `int`, so `member.type` is verified; the nullability is `member.nullable`'s, and it is the one finding.

```json case
{ "phase": 4, "base": "shadow-foreign-key", "kinds": ["member.type", "member.nullable"], "traces": ["ESC-DOM-37"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.Property<int>("BuyerId");

        builder.HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey("BuyerId");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-buyer-id", "outcome": "verified", "expected": "int?", "actual": "int", "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" },
    { "kind": "member.nullable", "element": "order-buyer-id", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/member.type", "count": 0 },
    { "rule": "claim/member.nullable", "severity": "error", "subject": "order-buyer-id" }
  ]
}
```

## services-dto

A Services DTO's fields are the record's positional parameters. `Date` is `DateTime` for model `datetime`, and `OrderItems` is a `List<OrderItemDto>` for a collection of the `OrderItemDto` DTO.

```json case
{ "phase": 1, "kinds": ["member.type"], "traces": ["ESC-SVC-46"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-item-dto", "type": "DTO", "name": "OrderItemDto",
            "children": [
              { "id": "order-item-dto-product-name", "type": "DTO-Field", "name": "ProductName", "typeRef": { "type": "string" } }
            ] },
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } },
              { "id": "order-dto-date", "type": "DTO-Field", "name": "Date", "typeRef": { "type": "datetime" } },
              { "id": "order-dto-order-items", "type": "DTO-Field", "name": "OrderItems", "typeRef": { "element": "order-item-dto", "collection": true } }
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

```xml file src/Application/Shop.Application.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/Application/Orders/OrderItemDto.cs
namespace Shop.Application.Orders;

public record OrderItemDto(string ProductName);
```

```csharp file src/Application/Orders/OrderDto.cs
namespace Shop.Application.Orders;

public record OrderDto(int OrderNumber, DateTime Date, List<OrderItemDto> OrderItems);
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-dto-date", "outcome": "verified", "expected": "datetime", "actual": "DateTime", "at": "src/Application/Orders/OrderDto.cs:3" },
    { "kind": "member.type", "element": "order-dto-order-items", "outcome": "verified", "expected": "OrderItemDto[]", "actual": "List<OrderItemDto>", "at": "src/Application/Orders/OrderDto.cs:3" }
  ]
}
```

## services-dto-contradicted

The record's `Date` is a `string`: the DTO-Field's type is contradicted, `*.field.type` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "services-dto", "kinds": ["member.type"], "traces": ["ESC-SVC-46"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Application/Orders/OrderDto.cs
namespace Shop.Application.Orders;

public record OrderDto(int OrderNumber, string Date, List<OrderItemDto> OrderItems);
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "order-dto-date", "outcome": "contradicted", "expected": "datetime", "actual": "string", "at": "src/Application/Orders/OrderDto.cs:3" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "order-dto-date" }
  ]
}
```

## binary-is-a-byte-array

The C# profile writes the model's `binary` as `byte[]`, which is also an array's form. A field of type `binary` isn't a collection, so `byte[]` matches it as written, before the array is read as a collection of `byte`. basislink5's first Software Factory run reported a file upload's `byte[]` as contradicting `binary`.

```json case
{ "phase": 1, "kinds": ["member.type"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "upload-dto", "type": "DTO", "name": "UploadDto",
            "children": [
              { "id": "upload-dto-file-data", "type": "DTO-Field", "name": "FileData", "typeRef": { "type": "binary" } },
              { "id": "upload-dto-chunks", "type": "DTO-Field", "name": "Chunks", "typeRef": { "type": "byte", "collection": true } }
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

```csharp file src/Application/Uploads/UploadDto.cs
namespace Shop.Application.Uploads;

public class UploadDto
{
    public byte[] FileData { get; set; }
    public byte[] Chunks { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "upload-dto-file-data", "outcome": "verified", "expected": "binary", "actual": "byte[]", "at": "src/Application/Uploads/UploadDto.cs:5" },
    { "kind": "member.type", "element": "upload-dto-chunks", "outcome": "verified", "expected": "byte[]", "actual": "byte[]", "at": "src/Application/Uploads/UploadDto.cs:6" }
  ]
}
```

## component-members

A Component's Property is a `[Parameter]` property and its Event Emitter a `[Parameter] EventCallback<T>`. The component's project turns nullable reference types on, so `string?` states a nullable `string`. An Event Emitter's type is the `T` of `EventCallback<T>`, so `EventCallback<int?>` is a nullable `int`.

```json case
{ "phase": 6, "kinds": ["member.type"], "traces": ["ESC-FE-17", "ESC-FE-23"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.WebAppComponents",
        "elements": [
          { "id": "catalog-search", "type": "Component", "name": "CatalogSearch",
            "children": [
              { "id": "catalog-search-search-text", "type": "Property", "name": "SearchText", "typeRef": { "type": "string", "nullable": true } },
              { "id": "catalog-search-brand-ids", "type": "Property", "name": "BrandIds", "typeRef": { "type": "int", "collection": true } },
              { "id": "catalog-search-brand-changed", "type": "Event Emitter", "name": "BrandChanged", "typeRef": { "type": "int", "nullable": true } }
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

```xml file src/WebAppComponents/WebAppComponents.csproj
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

```razor file src/WebAppComponents/Catalog/CatalogSearch.razor
<input @bind="searchText" placeholder="Search the catalog" />

@code {
    string? searchText;

    [Parameter] public string? SearchText { get; set; }

    [Parameter] public IEnumerable<int> BrandIds { get; set; } = [];

    [Parameter] public EventCallback<int?> BrandChanged { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "catalog-search-search-text", "outcome": "verified", "expected": "string?", "actual": "string?", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:6" },
    { "kind": "member.type", "element": "catalog-search-brand-ids", "outcome": "verified", "expected": "int[]", "actual": "IEnumerable<int>", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:8" },
    { "kind": "member.type", "element": "catalog-search-brand-changed", "outcome": "verified", "expected": "int?", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:10" }
  ]
}
```

## component-property-contradicted

`BrandIds` is a collection of `string`, against the model's collection of `int`: contradicted, `property.type` in eShopCustom's terms. `SearchText` is now a non-nullable `string` against the model's nullable one, and its type claim is still verified: nullability isn't part of `member.type`.

```json case
{ "phase": 6, "base": "component-members", "kinds": ["member.type"], "traces": ["ESC-FE-17"], "requirements": ["R1", "R42"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebAppComponents/Catalog/CatalogSearch.razor
<input @bind="searchText" placeholder="Search the catalog" />

@code {
    string? searchText;

    [Parameter] public string SearchText { get; set; } = "";

    [Parameter] public IEnumerable<string> BrandIds { get; set; } = [];

    [Parameter] public EventCallback<int?> BrandChanged { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "catalog-search-search-text", "outcome": "verified", "expected": "string?", "actual": "string", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:6" },
    { "kind": "member.type", "element": "catalog-search-brand-ids", "outcome": "contradicted", "expected": "int[]", "actual": "IEnumerable<string>", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:8" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "catalog-search-brand-ids" },
    { "rule": "claim/member.type", "subject": "catalog-search-search-text", "count": 0 }
  ]
}
```

## model-definition

A Model Definition's Properties are the record's positional parameters: `decimal` for `Price`, and `string?` for the nullable `PictureUrl`.

```json case
{ "phase": 6, "kinds": ["member.type"], "traces": ["ESC-FE-43"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.WebAppComponents",
        "elements": [
          { "id": "catalog-item", "type": "Model Definition", "name": "CatalogItem",
            "children": [
              { "id": "catalog-item-id", "type": "Property", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "catalog-item-price", "type": "Property", "name": "Price", "typeRef": { "type": "decimal" } },
              { "id": "catalog-item-picture-url", "type": "Property", "name": "PictureUrl", "typeRef": { "type": "string", "nullable": true } }
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

```xml file src/WebAppComponents/WebAppComponents.csproj
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/WebAppComponents/Catalog/CatalogItem.cs
namespace Shop.WebAppComponents.Catalog;

public record CatalogItem(int Id, decimal Price, string? PictureUrl);
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "catalog-item-price", "outcome": "verified", "expected": "decimal", "actual": "decimal", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:3" },
    { "kind": "member.type", "element": "catalog-item-picture-url", "outcome": "verified", "expected": "string?", "actual": "string?", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:3" }
  ]
}
```

## java-field-type

The same claim over Java fields, through the Java profile: `guid` is `UUID`, `decimal` is `BigDecimal`, a collection of `StockMovement` is a `List<StockMovement>`, and `datetime` is `LocalDateTime`. `DiscontinuedOn` is nullable in the model; `member.type` compares only the type and collection, and whether the unannotated `LocalDateTime` is nullable is `member.nullable`'s question (its reading is `unknown` there).

```json case
{ "phase": 4, "kinds": ["member.type"], "traces": ["JAV-DOM-12"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "stock-movement", "type": "Class", "name": "StockMovement",
            "children": [
              { "id": "stock-movement-quantity", "type": "Attribute", "name": "Quantity", "typeRef": { "type": "int" } }
            ] },
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "children": [
              { "id": "stock-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "guid" } },
              { "id": "stock-item-unit-cost", "type": "Attribute", "name": "UnitCost", "typeRef": { "type": "decimal" } },
              { "id": "stock-item-movements", "type": "Attribute", "name": "Movements", "typeRef": { "element": "stock-movement", "collection": true } },
              { "id": "stock-item-discontinued-on", "type": "Attribute", "name": "DiscontinuedOn", "typeRef": { "type": "datetime", "nullable": true } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/StockMovement.java
package com.shop.domain;

public class StockMovement {
    private int quantity;
}
```

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class StockItem {
    private UUID productId;
    private BigDecimal unitCost;
    private List<StockMovement> movements = new ArrayList<>();
    private LocalDateTime discontinuedOn;
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "stock-item-product-id", "outcome": "verified", "expected": "guid", "actual": "UUID", "at": "src/main/java/com/shop/domain/StockItem.java:10" },
    { "kind": "member.type", "element": "stock-item-unit-cost", "outcome": "verified", "expected": "decimal", "actual": "BigDecimal", "at": "src/main/java/com/shop/domain/StockItem.java:11" },
    { "kind": "member.type", "element": "stock-item-movements", "outcome": "verified", "expected": "StockMovement[]", "actual": "List<StockMovement>", "at": "src/main/java/com/shop/domain/StockItem.java:12" },
    { "kind": "member.type", "element": "stock-item-discontinued-on", "outcome": "verified", "expected": "datetime?", "actual": "LocalDateTime", "at": "src/main/java/com/shop/domain/StockItem.java:13" }
  ]
}
```

## java-field-type-contradicted

`unitCost` is a `double`, which the Java profile doesn't list for `decimal`: contradicted.

```json case
{ "phase": 4, "base": "java-field-type", "kinds": ["member.type"], "traces": ["JAV-DOM-12"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class StockItem {
    private UUID productId;
    private double unitCost;
    private List<StockMovement> movements = new ArrayList<>();
    private LocalDateTime discontinuedOn;
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "stock-item-unit-cost", "outcome": "contradicted", "expected": "decimal", "actual": "double", "at": "src/main/java/com/shop/domain/StockItem.java:11" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "stock-item-unit-cost" }
  ]
}
```

## java-record-boxed

A Services DTO in Java is a record, and its fields are the record's components. The Java profile maps `int` to `int` and its boxed `Integer`, so `Integer` matches the model's `int`. A boxed type is nullable, which matches the model's nullable `ReorderLevel` as a `member.nullable` claim.

```json case
{ "phase": 4, "kinds": ["member.type", "member.nullable"], "traces": ["JAV-SVC-19"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "set-reorder-level-input", "type": "DTO", "name": "SetReorderLevelInput",
            "children": [
              { "id": "set-reorder-level-input-product-id", "type": "DTO-Field", "name": "ProductId", "typeRef": { "type": "guid" } },
              { "id": "set-reorder-level-input-reorder-level", "type": "DTO-Field", "name": "ReorderLevel", "typeRef": { "type": "int", "nullable": true } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/application/SetReorderLevelInput.java
package com.eshop.backend.application;

import java.util.UUID;

public record SetReorderLevelInput(UUID productId, Integer reorderLevel) {
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "set-reorder-level-input-reorder-level", "outcome": "verified", "expected": "int?", "actual": "Integer", "at": "src/main/java/com/eshop/backend/application/SetReorderLevelInput.java:5" },
    { "kind": "member.nullable", "element": "set-reorder-level-input-reorder-level", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/eshop/backend/application/SetReorderLevelInput.java:5" }
  ]
}
```

## java-record-primitive

The component is a primitive `int`, which can't be null, against the model's nullable `ReorderLevel`. The type is still `int`, so `member.type` is verified, and the one finding is `member.nullable`'s: a primitive is never nullable.

```json case
{ "phase": 4, "base": "java-record-boxed", "kinds": ["member.type", "member.nullable"], "traces": ["JAV-SVC-19"], "requirements": ["R1", "R42"], "packs": ["intent.core", "intent.services", "intent.java"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/application/SetReorderLevelInput.java
package com.eshop.backend.application;

import java.util.UUID;

public record SetReorderLevelInput(UUID productId, int reorderLevel) {
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "set-reorder-level-input-reorder-level", "outcome": "verified", "expected": "int?", "actual": "int", "at": "src/main/java/com/eshop/backend/application/SetReorderLevelInput.java:5" },
    { "kind": "member.nullable", "element": "set-reorder-level-input-reorder-level", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/main/java/com/eshop/backend/application/SetReorderLevelInput.java:5" }
  ],
  "findings": [
    { "rule": "claim/member.type", "count": 0 },
    { "rule": "claim/member.nullable", "severity": "error", "subject": "set-reorder-level-input-reorder-level" }
  ]
}
```

## java-record-contradicted

The component is a `String`, which the Java profile doesn't list for `int`: contradicted.

```json case
{ "phase": 4, "base": "java-record-boxed", "kinds": ["member.type"], "traces": ["JAV-SVC-19"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.services", "intent.java"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/application/SetReorderLevelInput.java
package com.eshop.backend.application;

import java.util.UUID;

public record SetReorderLevelInput(UUID productId, String reorderLevel) {
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "set-reorder-level-input-reorder-level", "outcome": "contradicted", "expected": "int?", "actual": "String", "at": "src/main/java/com/eshop/backend/application/SetReorderLevelInput.java:5" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "set-reorder-level-input-reorder-level" }
  ]
}
```

## proto-message-fields

A DTO with eShopCustom's Proto Message stereotype is a `message` in the service's `.proto` file, and its DTO-Fields are the message's fields. The Protobuf reader keeps the `.proto`'s names; the gRPC pack's protoc naming makes the model's `ProductId` the field `product_id`, and its Protobuf profile maps `int` to `int32`. `Items` is a collection of the `BasketItem` DTO, so the field must be `repeated` and of the message `BasketItem` is located as. The house pack says where the messages are, since Proto Message is eShopCustom's own stereotype.

```json case
{ "phase": "later", "kinds": ["member.type"], "traces": ["ESC-SVC-83"], "requirements": ["R2", "R28", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.grpc", "house.eshop"], "language": "protobuf" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.grpc", "version": "^1.0.0" }],
  "languages": ["csharp", "protobuf"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "DTO": { "where": "hasStereotype('Proto Message')", "typeKinds": ["message"], "locations": ["src/**/Proto/*.proto"], "note": "eShopCustom: a Proto Message DTO is a message in its service's .proto file" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-item", "type": "DTO", "name": "BasketItem", "stereotypes": { "Proto Message": {} },
            "children": [
              { "id": "basket-item-product-id", "type": "DTO-Field", "name": "ProductId", "typeRef": { "type": "int" } },
              { "id": "basket-item-quantity", "type": "DTO-Field", "name": "Quantity", "typeRef": { "type": "int" } }
            ] },
          { "id": "customer-basket-response", "type": "DTO", "name": "CustomerBasketResponse", "stereotypes": { "Proto Message": {} },
            "children": [
              { "id": "customer-basket-response-items", "type": "DTO-Field", "name": "Items", "typeRef": { "element": "basket-item", "collection": true } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```proto file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

message CustomerBasketResponse {
    repeated BasketItem items = 1;
}

message BasketItem {
    int32 product_id = 2;
    int32 quantity = 6;
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "basket-item-product-id", "outcome": "verified", "expected": "int", "actual": "int32", "at": "src/Basket.API/Proto/basket.proto:12" },
    { "kind": "member.type", "element": "basket-item-quantity", "outcome": "verified", "expected": "int", "actual": "int32", "at": "src/Basket.API/Proto/basket.proto:13" },
    { "kind": "member.type", "element": "customer-basket-response-items", "outcome": "verified", "expected": "BasketItem[]", "at": "src/Basket.API/Proto/basket.proto:8" }
  ],
  "findings": [
    { "rule": "claim/member.type", "count": 0 }
  ]
}
```

## proto-message-fields-contradicted

`product_id` is a `string`, which the Protobuf profile doesn't list for `int`, and `items` isn't `repeated`, so it is one `BasketItem` where the model has a collection: both contradicted, `.field.type` in eShopCustom's terms.

```json case
{ "phase": "later", "base": "proto-message-fields", "kinds": ["member.type"], "traces": ["ESC-SVC-83"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.services", "intent.grpc", "house.eshop"], "language": "protobuf" }
```

```proto file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

message CustomerBasketResponse {
    BasketItem items = 1;
}

message BasketItem {
    string product_id = 2;
    int32 quantity = 6;
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "basket-item-product-id", "outcome": "contradicted", "expected": "int", "actual": "string", "at": "src/Basket.API/Proto/basket.proto:12" },
    { "kind": "member.type", "element": "customer-basket-response-items", "outcome": "contradicted", "expected": "BasketItem[]", "actual": "BasketItem", "at": "src/Basket.API/Proto/basket.proto:8" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "basket-item-product-id" },
    { "rule": "claim/member.type", "severity": "error", "subject": "customer-basket-response-items" }
  ]
}
```
