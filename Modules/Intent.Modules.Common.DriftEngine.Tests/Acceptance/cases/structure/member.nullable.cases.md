# member.nullable

A modelled attribute, DTO field or Domain Event property is nullable exactly when its type reference says so. The code side is the member's type ref's `nullable`: `true`, `false` or `unknown` (code-reader.md, Type references). A value type's nullability is always known (`int` or `int?`); a C# reference type's is known only inside a nullable context, from the project's `<Nullable>` or a `#nullable` directive; a Java type's comes from the Java profile (primitives, boxed types and nullability annotations). An `unknown` reading is never guessed: the claim is can't verify, `unresolved` (rules.md, What is compared; R35). Values are compared as canonical text, `true` or `false`.

## value-types

`Units` is an `int` and `Discount` a `decimal?`. A value type states its nullability in any context, so no project file is needed.

```json case
{ "phase": 1, "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "children": [
              { "id": "order-item-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } },
              { "id": "order-item-discount", "type": "Attribute", "name": "Discount", "typeRef": { "type": "decimal", "nullable": true } }
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

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

public class OrderItem
{
    public int Units { get; private set; }

    public decimal? Discount { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-item-units", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Domain/OrderItem.cs:5" },
    { "kind": "member.nullable", "element": "order-item-discount", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Domain/OrderItem.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "count": 0 }
  ]
}
```

## value-type-contradicted

`Discount` is a plain `decimal`, which can't be null, against the model's nullable attribute: contradicted, `attribute.nullability` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "value-types", "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

public class OrderItem
{
    public int Units { get; private set; }

    public decimal Discount { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-item-discount", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/Domain/OrderItem.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "order-item-discount" }
  ]
}
```

## reference-nullable-context

The project turns nullable reference types on, so `string` states a non-nullable `string` and `string?` a nullable one.

```json case
{ "phase": 1, "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R2", "R35", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "children": [
              { "id": "buyer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } },
              { "id": "buyer-email", "type": "Attribute", "name": "Email", "typeRef": { "type": "string", "nullable": true } }
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

```csharp file src/Domain/Buyer.cs
namespace Shop.Domain;

public class Buyer
{
    public string Name { get; private set; } = "";

    public string? Email { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "buyer-name", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Domain/Buyer.cs:5" },
    { "kind": "member.nullable", "element": "buyer-email", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Domain/Buyer.cs:7" }
  ]
}
```

## reference-nullable-directive

The project doesn't set `<Nullable>`, but the file opens with `#nullable enable`, which makes its reference types state their nullability. `Email` is verified. `Name` is `string?`, nullable, against the model's non-nullable attribute: contradicted.

```json case
{ "phase": 1, "base": "reference-nullable-context", "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R1", "R2", "R35"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

</Project>
```

```csharp file src/Domain/Buyer.cs
#nullable enable

namespace Shop.Domain;

public class Buyer
{
    public string? Name { get; private set; }

    public string? Email { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "buyer-name", "outcome": "contradicted", "expected": "false", "actual": "true", "at": "src/Domain/Buyer.cs:7" },
    { "kind": "member.nullable", "element": "buyer-email", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Domain/Buyer.cs:9" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "buyer-name" }
  ]
}
```

## unresolved-nullable-disabled

The project turns nullable reference types off and the file has no `#nullable` directive, so `string Name` and `string Email` say nothing about nullability: the reading is `unknown`, and both claims are can't verify, `unresolved`, rather than verified or contradicted by a guess (R35). With nullable reference types off, ESC-DOM-35 takes nullability from `[Required]` or `.IsRequired()`. That is EF Core's mapping, which the EF Core extension adds to `member.nullable` where EF Core is in the code (`ef-required-nullable-disabled`); here it isn't.

```json case
{ "phase": 1, "base": "reference-nullable-context", "kinds": ["member.nullable"], "cases": ["engine/unresolved", "engine/notStated"], "traces": ["ESC-DOM-35"], "requirements": ["R1", "R5", "R35"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/Domain/Buyer.cs
namespace Shop.Domain;

public class Buyer
{
    public string Name { get; private set; }

    public string Email { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "buyer-name", "outcome": "cantVerify", "reason": "unresolved" },
    { "kind": "member.nullable", "element": "buyer-email", "outcome": "cantVerify", "reason": "unresolved" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "subject": "buyer-name", "count": 0 },
    { "rule": "claim/member.nullable", "subject": "buyer-email", "count": 0 },
    { "rule": "engine/notStated", "severity": "info", "messageContains": "Not checked: 2 nullability claims (Domain 2)" }
  ]
}
```

## services-dto

A DTO-Field's nullability against the record's positional parameter: `DateTime?` for the nullable `ShippedOn`, `int` for `OrderNumber`.

```json case
{ "phase": 1, "kinds": ["member.nullable"], "traces": ["ESC-SVC-46"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } },
              { "id": "order-summary-shipped-on", "type": "DTO-Field", "name": "ShippedOn", "typeRef": { "type": "datetime", "nullable": true } }
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

```csharp file src/Ordering.API/Application/Queries/OrderSummary.cs
namespace eShop.Ordering.API.Application.Queries;

public record OrderSummary(int OrderNumber, DateTime? ShippedOn);
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-summary-order-number", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Ordering.API/Application/Queries/OrderSummary.cs:3" },
    { "kind": "member.nullable", "element": "order-summary-shipped-on", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Ordering.API/Application/Queries/OrderSummary.cs:3" }
  ]
}
```

## services-dto-contradicted

`ShippedOn` is a plain `DateTime`: the DTO-Field's nullability is contradicted, `*.field.nullability` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "services-dto", "kinds": ["member.nullable"], "traces": ["ESC-SVC-46"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Queries/OrderSummary.cs
namespace eShop.Ordering.API.Application.Queries;

public record OrderSummary(int OrderNumber, DateTime ShippedOn);
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-summary-shipped-on", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/Ordering.API/Application/Queries/OrderSummary.cs:3" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "order-summary-shipped-on" }
  ]
}
```

## java-primitive-boxed-annotated

The Java profile says how a Java field states nullability: a primitive `int` can't be null, a boxed `Integer` can, and a reference type annotated `@NotNull` can't.

```json case
{ "phase": 4, "kinds": ["member.nullable"], "traces": ["JAV-DOM-14"], "requirements": ["R2", "R33", "R35", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "children": [
              { "id": "stock-item-sku", "type": "Attribute", "name": "Sku", "typeRef": { "type": "string" } },
              { "id": "stock-item-quantity-on-hand", "type": "Attribute", "name": "QuantityOnHand", "typeRef": { "type": "int" } },
              { "id": "stock-item-reorder-level", "type": "Attribute", "name": "ReorderLevel", "typeRef": { "type": "int", "nullable": true } }
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

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import jakarta.validation.constraints.NotNull;

public class StockItem {
    @NotNull
    private String sku;
    private int quantityOnHand;
    private Integer reorderLevel;
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "stock-item-sku", "outcome": "verified", "expected": "false", "actual": "false" },
    { "kind": "member.nullable", "element": "stock-item-quantity-on-hand", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/main/java/com/shop/domain/StockItem.java:8" },
    { "kind": "member.nullable", "element": "stock-item-reorder-level", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/shop/domain/StockItem.java:9" }
  ]
}
```

## java-primitive-contradicted

`reorderLevel` is a primitive `int`, which can't hold the model's nullable value: contradicted.

```json case
{ "phase": 4, "base": "java-primitive-boxed-annotated", "kinds": ["member.nullable"], "traces": ["JAV-DOM-14"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import jakarta.validation.constraints.NotNull;

public class StockItem {
    @NotNull
    private String sku;
    private int quantityOnHand;
    private int reorderLevel;
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "stock-item-reorder-level", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/main/java/com/shop/domain/StockItem.java:9" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "stock-item-reorder-level" }
  ]
}
```

## ef-required-nullable-disabled

The same project with EF Core: nullable reference types are off, so the property's type doesn't say, and EF Core decides the column. `.IsRequired()` makes `Name`'s required, and `Email`, configured with nothing, is optional, as EF Core maps a reference type without nullable reference types. Both follow the model (ESC-DOM-35). A required column is the attribute's own nullability: the Domain designer has nothing else to state it with.

```json case
{ "phase": 4, "base": "unresolved-nullable-disabled", "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.Property(b => b.Name).IsRequired();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "buyer-name", "outcome": "verified", "expected": "false", "actual": "false",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" },
    { "kind": "member.nullable", "element": "buyer-email", "outcome": "verified", "expected": "true", "actual": "true" }
  ]
}
```

## ef-unconfigured-required-nullable-disabled

The model says `Name` is required, and the configuration says nothing about it. With nullable reference types off, the `string` type can't make the column required, so only `.IsRequired()` or `[Required]` can, and without either EF Core maps it nullable: the model is contradicted. The finding is against `BuyerConfiguration`, the code the model is checked against, not the property, and names the missing configuration, not the type, which says nothing (user decision, 2026-11-24: "The Model says its required, so EF should say its required"). eShop's Ordering had reported a `Test` added as `string` to `Order` on both sides as "model `false`, the code doesn't state it, so it is `true`", which read as if the code said the property was nullable. `Email`, modelled optional, still follows the model.

```json case
{ "phase": 4, "base": "ef-required-nullable-disabled", "kinds": ["member.nullable"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.ToTable("Buyers");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "buyer-name", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:7" },
    { "kind": "member.nullable", "element": "buyer-email", "outcome": "verified", "expected": "true", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "buyer-name",
      "messageContains": "`Buyer.Name` is required in the model, but EF Core maps its column as nullable: nothing configures `IsRequired()` or `[Required]` for it" },
    { "rule": "claim/member.nullable", "subject": "buyer-email", "count": 0 }
  ]
}
```

## ef-missing-required-property

`Name` isn't in the code at all. It is reported missing, and in the same run its configuration is checked by its name: the model says required, a `string` outside a nullable context can't say so once it is added, and `BuyerConfiguration` has no `IsRequired()`, so that is a finding of its own, against the configuration (user decision, 2026-11-24: separate findings, so a missing property and its configuration take one trip). A property the model leaves optional, or whose type will state it, needs nothing configured and waits on the property.

```json case
{ "phase": 4, "base": "ef-unconfigured-required-nullable-disabled", "kinds": ["member.nullable", "member.exists"], "requirements": ["R2", "R3", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Buyer.cs
namespace Shop.Domain;

public class Buyer
{
    public string Email { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "buyer-name", "outcome": "contradicted" },
    { "kind": "member.nullable", "element": "buyer-name", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:7" },
    { "kind": "member.nullable", "element": "buyer-email", "outcome": "verified", "expected": "true", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "buyer-name", "messageContains": "isn't in the code" },
    { "rule": "claim/member.nullable", "severity": "error", "subject": "buyer-name",
      "messageContains": "`Buyer.Name` is required in the model, but EF Core maps its column as nullable" }
  ]
}
```

## required-column-on-nullable-property

Nullable reference types are on, and `Description` is a `string?` the model calls nullable, but its configuration says `.IsRequired()`: EF Core's scaffolding writes this for a `NOT NULL` column with a default, so an insert can leave it out. The model has one nullability for the attribute and states it in the property's type, which agrees; it can't state a required column on a nullable property, so nothing is claimed about the column and there's no drift.

```json case
{ "phase": 4, "base": "reference-nullable-context", "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R2", "R4"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string", "nullable": true } }
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
#nullable enable
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }

    public string? Description { get; private set; }
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
        builder.Property(o => o.Description).IsRequired().HasDefaultValueSql("''");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-description", "outcome": "verified", "expected": "true", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "count": 0 }
  ]
}
```

## association-reference-optional

A navigable reference end is optional when its multiplicity is `0..1` (`association.nullability`): an order may have no buyer, and `Order.Buyer` is `Buyer?` in a nullable context.

```json case
{ "phase": 1, "kinds": ["member.nullable"], "traces": ["ESC-DOM-56"], "requirements": ["R2", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "0..1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Domain/Ordering.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public Buyer? Buyer { get; private set; }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-buyer", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Domain/Order.cs:7" }
  ]
}
```

## association-reference-required

The model says every order has a buyer (`1`), but `Order.Buyer` is `Buyer?`, so the code lets an order have none. The finding says so as the end's multiplicity.

```json case
{ "phase": 1, "base": "association-reference-optional", "kinds": ["member.nullable"], "traces": ["ESC-DOM-56"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": true } }
      ]
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
    { "kind": "member.nullable", "element": "order-buyer", "outcome": "contradicted", "expected": "false", "actual": "true", "at": "src/Domain/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "order-buyer",
      "messageContains": "`Order.Buyer` multiplicity: model `1`, code `0..1`." }
  ]
}
```

## java-many-to-one-optional

Over JPA, a single end is required when its relationship says so: `@ManyToOne(optional = false)`, `@JoinColumn(nullable = false)` or `@NotNull`. Here the field is a plain `@ManyToOne`, so a line may have no product, where the model's `Product` end is `1`. The finding names the missing mapping.

```json case
{ "phase": 4, "kinds": ["member.nullable"], "traces": ["JAV-DOM-25"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "product", "type": "Class", "name": "Product",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "product-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-line-product", "type": "Association",
          "source": { "element": "order-line", "name": "OrderLines", "multiplicity": "*", "navigable": false },
          "target": { "element": "product", "name": "Product", "multiplicity": "1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/OrderLine.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import java.util.UUID;

@Entity
public class OrderLine {
    @Id
    private UUID id;

    @ManyToOne
    @JoinColumn(name = "product_id")
    private Product product;
}
```

```java file src/main/java/com/shop/domain/Product.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Product {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-line-product", "outcome": "contradicted", "expected": "false", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "order-line-product",
      "messageContains": "`OrderLine.Product` is required in the model, but JPA maps it as nullable" }
  ]
}
```

## java-many-to-one-required

`optional = false` makes the product required, as the model's `1` says.

```json case
{ "phase": 4, "base": "java-many-to-one-optional", "kinds": ["member.nullable"], "traces": ["JAV-DOM-25"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/OrderLine.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import java.util.UUID;

@Entity
public class OrderLine {
    @Id
    private UUID id;

    @ManyToOne(optional = false)
    @JoinColumn(name = "product_id")
    private Product product;
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-line-product", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ]
}
```

## ef-navigation-required-nullable-disabled

Nullable reference types are off, so `Order.Buyer`'s type can't say whether an order may have no buyer. EF Core decides it from the relationship: no `IsRequired`, so the foreign key `HasForeignKey` names, an `int` that can't be null, makes it required. The model's `1` agrees.

```json case
{ "phase": 4, "kinds": ["member.nullable"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int" } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Domain/Ordering.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public int BuyerId { get; private set; }

    public Buyer Buyer { get; private set; }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }
}
```

```csharp file src/Domain/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Domain.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-buyer", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Domain/Order.cs:7" }
  ]
}
```

## ef-navigation-optional-contradicted

The same relationship, with the foreign key an `int?`: EF Core makes the navigation optional, and the model says every order has a buyer.

```json case
{ "phase": 4, "base": "ef-navigation-required-nullable-disabled", "kinds": ["member.nullable"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public int? BuyerId { get; private set; }

    public Buyer Buyer { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-buyer", "outcome": "contradicted", "expected": "false", "actual": "true", "at": "src/Domain/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "order-buyer",
      "messageContains": "`Order.Buyer` multiplicity: model `1`, code `0..1`." }
  ]
}
```

## ef-navigation-is-required

The chain's own `IsRequired(false)` decides, whatever the foreign key's type: the navigation is optional.

```json case
{ "phase": 4, "base": "ef-navigation-required-nullable-disabled", "kinds": ["member.nullable"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Domain.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o => o.BuyerId).IsRequired(false);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-buyer", "outcome": "contradicted", "expected": "false", "actual": "true", "at": "src/Domain/Persistence/OrderConfiguration.cs:10" }
  ]
}
```

## ef6-has-required

An EF6 context configures the relationship with `HasRequired(o => o.Buyer)`, EF6's form of `HasOne(o => o.Buyer).IsRequired()`. It decides whatever the foreign key's type, so with `BuyerId` an `int?` the navigation is still required, and the model's `1` agrees. A client's EF6 data layer configures its relationships this way.

```json case
{ "phase": 4, "base": "ef-navigation-optional-contradicted", "without": ["src/Domain/Persistence/OrderConfiguration.cs"], "kinds": ["member.nullable"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Persistence/OrderingContext.cs
using System.Data.Entity;

namespace Ordering.Domain.Persistence;

public class OrderingContext : DbContext
{
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasRequired(o => o.Buyer);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-buyer", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Domain/Persistence/OrderingContext.cs:10" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "subject": "order-buyer", "count": 0 }
  ]
}
```

## ef6-configured-from-both-ends

The EF6 context configures the relationship from both ends: `Buyer`'s `HasMany(b => b.Orders).WithRequired(o => o.Buyer)`, which a client's context then restates from `Order` with `HasRequired(o => o.Buyer)`. Both say the navigation is required, so they are one statement, not two places to choose from; chains that disagree would be ambiguous.

```json case
{ "phase": 4, "base": "ef6-has-required", "kinds": ["member.nullable"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }

    public ICollection<Order> Orders { get; private set; }
}
```

```csharp file src/Domain/Persistence/OrderingContext.cs
using System.Data.Entity;

namespace Ordering.Domain.Persistence;

public class OrderingContext : DbContext
{
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Buyer>()
            .HasMany(b => b.Orders)
            .WithRequired(o => o.Buyer);

        modelBuilder.Entity<Order>()
            .HasRequired(o => o.Buyer);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "order-buyer", "outcome": "verified", "expected": "false", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "subject": "order-buyer", "count": 0 }
  ]
}
```

## python-annotations

A Python annotation states whether a field can be `None`, as a type checker reads it: `int | None` (or `Optional[int]`) can, and any other type, `str` or `int`, can't.

```json case
{ "phase": 4, "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R2", "R33", "R35", "R42"], "packs": ["intent.core", "intent.domain", "intent.python"], "language": "python" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "children": [
              { "id": "stock-item-sku", "type": "Attribute", "name": "Sku", "typeRef": { "type": "string" } },
              { "id": "stock-item-quantity-on-hand", "type": "Attribute", "name": "QuantityOnHand", "typeRef": { "type": "int" } },
              { "id": "stock-item-reorder-level", "type": "Attribute", "name": "ReorderLevel", "typeRef": { "type": "int", "nullable": true } }
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

```python file src/shop/domain/stock_item.py
from dataclasses import dataclass


@dataclass
class StockItem:
    sku: str
    quantity_on_hand: int
    reorder_level: int | None = None
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "stock-item-sku", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/shop/domain/stock_item.py:6" },
    { "kind": "member.nullable", "element": "stock-item-quantity-on-hand", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/shop/domain/stock_item.py:7" },
    { "kind": "member.nullable", "element": "stock-item-reorder-level", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/shop/domain/stock_item.py:8" }
  ]
}
```

## python-annotation-contradicted

`reorder_level` is annotated `int`, which can't be `None`, where the model's is nullable: contradicted.

```json case
{ "phase": 4, "base": "python-annotations", "kinds": ["member.nullable"], "traces": ["ESC-DOM-35"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.domain", "intent.python"], "language": "python" }
```

```python file src/shop/domain/stock_item.py
from dataclasses import dataclass


@dataclass
class StockItem:
    sku: str
    quantity_on_hand: int
    reorder_level: int = 0
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "stock-item-reorder-level", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/shop/domain/stock_item.py:8" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "stock-item-reorder-level" }
  ]
}
```
