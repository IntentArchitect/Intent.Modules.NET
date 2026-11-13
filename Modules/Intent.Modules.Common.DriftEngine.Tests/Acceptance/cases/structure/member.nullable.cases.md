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

The project turns nullable reference types off and the file has no `#nullable` directive, so `string Name` and `string Email` say nothing about nullability: the reading is `unknown`, and both claims are can't verify, `unresolved`, rather than verified or contradicted by a guess (R35). With nullable reference types off, ESC-DOM-35 takes nullability from `[Required]` or `.IsRequired()`. That is EF Core's mapping, which `column.required` claims from the persistence packs; `member.nullable` doesn't borrow it.

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
