# layer.populated

A Layer, from eShopCustom's own Architecture module, states that its Layer Settings Path holds code. The module adds the Layer element and its Layer Settings stereotype to the Codebase Structure designer's Root Folder, and no stock pack knows them, so a house pack gives the kind, its vocabulary and its rule. That is phase 2's point: a module's own designer is checked with no change to the engine (R28).

The kind is `stated: always`. The house pack's recipe finds the files under the Path and, with `matches: "any"`, reads `true` from the first of them in path and line order rather than calling several files `ambiguous` (rules.md, Recipes, Additions): a Layer holding many files is populated by any of them. Its `absent` gives `false` when the find finds nothing, so a Layer whose Path holds no file is contradicted from silence.

## verified

The Domain Layer's Path holds three C# files. The find ends at all three, and `matches: "any"` reads the first in path order, `Buyer.cs`, whose read case gives `true`.

```json case
{ "phase": 2, "kinds": ["layer.populated"], "traces": ["ESC-ARC-03"], "requirements": ["R2", "R28", "R30", "R51"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["Codebase Structure"],
  "kinds": [
    { "kind": "layer.populated", "label": "layer contents", "valueType": "bool", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Codebase Structure": {
      "entries": [
        {
          "kind": "layer.populated",
          "for": { "element": "Layer" },
          "value": "true",
          "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
          "own": ["Layer"],
          "source": "the Layer and its Layer Settings Path"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{
        "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Domain" } } }
        ]
      }]
    }
  }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs
namespace eShop.Ordering.Domain.AggregatesModel.BuyerAggregate;

public class Buyer
{
    public string IdentityGuid { get; private set; }
}
```

```csharp file src/Ordering.Domain/SeedWork/Entity.cs
namespace eShop.Ordering.Domain.SeedWork;

public abstract class Entity
{
    public virtual int Id { get; protected set; }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "verified",
      "expected": "true", "actual": "true", "at": "src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs:1" }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "subject": "domain-layer", "count": 0 }
  ]
}
```

## contradicted-empty

eShopCustom's `architecture.layer.empty`: the model adds an Infrastructure Layer whose Path holds only a project file, no C# file. The find finds nothing, so the recipe's `absent` gives `false` from silence, and the modelled Layer is contradicted. The finding quotes the recipe's `unstated` text, naming the Path.

```json case
{ "phase": 2, "base": "verified", "kinds": ["layer.populated"], "cases": ["engine/silence"], "traces": ["ESC-ARC-03"], "requirements": ["R3", "R11", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{
        "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Domain" } } },
          { "id": "infrastructure-layer", "type": "Layer", "name": "Infrastructure",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Infrastructure" } } }
        ]
      }]
    }
  }
}
```

```xml file src/Ordering.Infrastructure/Ordering.Infrastructure.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <RootNamespace>eShop.Ordering.Infrastructure</RootNamespace>
  </PropertyGroup>
</Project>
```

```json expect
{
  "claims": [
    { "kind": "layer.populated", "element": "infrastructure-layer", "outcome": "contradicted",
      "expected": "true", "actual": "false", "fromSilence": true },
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "severity": "error", "subject": "infrastructure-layer", "messageContains": "src/Ordering.Infrastructure" },
    { "rule": "claim/layer.populated", "subject": "domain-layer", "count": 0 }
  ]
}
```

## cant-verify-out-of-scope

eShopCustom's `architecture.layer.outside-scope`: the Domain Layer's folder is excluded by the Root Folder's second scope row. The find finds nothing in scope, but its `file:` step matches the excluded `Order.cs` by path, so the claim is can't verify, `outOfScope`, naming that row (R25). The shadow lookup comes before the recipe's `absent`, so an excluded Layer is never reported as empty, and the excluded file is never read.

```json case
{ "phase": 2, "base": "verified", "kinds": ["layer.populated"], "cases": ["scope/shadow"], "traces": ["ESC-ARC-03", "ESC-ARC-04"], "requirements": ["R5", "R22", "R25"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{
        "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Ordering.Domain/**", "Scope": "Out", "Reason": "Being moved into src/Ordering.Core" }
        ] } },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Domain" } } }
        ]
      }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "severity": "error", "subject": "domain-layer", "messageContains": "src/Ordering.Domain/**" }
  ],
  "filesRead": { "exclude": [
    "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs",
    "src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs",
    "src/Ordering.Domain/SeedWork/Entity.cs"
  ] }
}
```

## java-contradicted-empty

The Java spike's Architecture check: a Layer whose Path holds no source file is a stale model. Its house pack finds Java, TypeScript or Vue files under the Path, trying each in turn, and reads any of them (`matches: "any"`). The Domain Layer holds two Java classes and verifies from the first in path order; the Infrastructure Layer's package folder holds nothing, so it is contradicted from silence.

```json case
{ "phase": 4, "kinds": ["layer.populated"], "cases": ["engine/silence"], "traces": ["JAV-ARC-03"], "requirements": ["R3", "R28", "R51"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "java" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["java", "typescript"],
  "designers": ["Codebase Structure"],
  "kinds": [
    { "kind": "layer.populated", "label": "layer contents", "valueType": "bool", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Codebase Structure": {
      "entries": [
        {
          "kind": "layer.populated",
          "for": { "element": "Layer" },
          "value": "true",
          "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
          "own": ["Layer"],
          "source": "the Layer and its Layer Settings Path"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{
        "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/main/java/com/shop/domain" } } },
          { "id": "infrastructure-layer", "type": "Layer", "name": "Infrastructure",
            "stereotypes": { "Layer Settings": { "Path": "src/main/java/com/shop/infrastructure" } } }
        ]
      }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/Product.java
package com.shop.domain;

public class Product {
    private String name;
}
```

```java file src/main/java/com/shop/domain/Category.java
package com.shop.domain;

public class Category {
    private String name;
}
```

```json expect
{
  "claims": [
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "verified", "at": "src/main/java/com/shop/domain/Category.java:1" },
    { "kind": "layer.populated", "element": "infrastructure-layer", "outcome": "contradicted",
      "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "severity": "error", "subject": "infrastructure-layer", "messageContains": "src/main/java/com/shop/infrastructure" }
  ]
}
```
