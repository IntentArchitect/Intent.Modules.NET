# layer.dependencies

A Layer's May Depend On associations state which other Layers its code may import from. The model's value is the set of Layers it may depend on; the code's is the set of Layers its files import from. The `within` comparer verifies when every Layer the code imports from is one the model allows, and a contradiction names each offending import and its line (claims-and-findings.md, Comparers).

No single find reaches this, so the house pack's rule is a query (R47), as rules.md's example gives it: each import's file is owned by the Layer whose Path is the longest prefix of it, the import is resolved to the files that declare what it names, and those files are owned in turn. An import whose target no Layer's file declares, such as a library namespace, drops out, and so does an import within one Layer. For C#, a `using` names a namespace, so `resolve` maps it to the files declaring that namespace; a Java `import` names a class.

## verified

eShopCustom's Ordering service: Infrastructure may depend on Domain, and its `OrderingContext` imports Domain's namespace. Its `Microsoft.EntityFrameworkCore` import names a namespace no in-scope file declares, so it is ignored. Code the Domain Layer doesn't import is not a claim this case is about.

```json case
{ "phase": 3, "kinds": ["layer.dependencies"], "traces": ["ESC-ARC-08"], "requirements": ["R2", "R10", "R45", "R46"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["Codebase Structure"],
  "kinds": [
    { "kind": "layer.dependencies", "label": "layer dependencies", "valueType": "set", "comparer": "within", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Codebase Structure": {
      "entries": [
        {
          "kind": "layer.dependencies",
          "for": { "element": "Layer" },
          "value": "map(associations('May Depend On'), it.element.name)",
          "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
          "own": ["Layer"],
          "source": "the Layer's May Depend On associations",
          "fixHint": "Remove the import, or add a May Depend On from the Layer to the one it imports from."
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
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Domain" } } },
          { "id": "infrastructure-layer", "type": "Layer", "name": "Infrastructure",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Infrastructure" } } }
        ]
      }],
      "associations": [
        { "id": "infrastructure-may-depend-on-domain", "type": "May Depend On",
          "source": { "element": "infrastructure-layer" }, "target": { "element": "domain-layer" } }
      ]
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

```csharp file src/Ordering.Infrastructure/OrderingContext.cs
using eShop.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace eShop.Ordering.Infrastructure;

public class OrderingContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.dependencies", "element": "infrastructure-layer", "outcome": "verified",
      "expected": "Domain", "actual": "Domain", "at": "src/Ordering.Infrastructure/OrderingContext.cs:1" }
  ],
  "findings": [
    { "rule": "claim/layer.dependencies", "subject": "infrastructure-layer", "count": 0 }
  ]
}
```

## contradicted-planted-import

eShopCustom's planted break: `Order.cs` in the Domain Layer gains a `using` of Infrastructure's namespace, and Domain has no May Depend On to Infrastructure. Infrastructure is not within Domain's allowed set, which is empty, so the claim is contradicted, and the finding names the import's file and line. Infrastructure's own claim still verifies.

```json case
{ "phase": 3, "base": "verified", "kinds": ["layer.dependencies"], "traces": ["ESC-ARC-08"], "requirements": ["R10", "R11", "R14", "R46"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
using eShop.Ordering.Infrastructure;

namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.dependencies", "element": "domain-layer", "outcome": "contradicted",
      "actual": "Infrastructure", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:1" },
    { "kind": "layer.dependencies", "element": "infrastructure-layer", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/layer.dependencies", "severity": "error", "subject": "domain-layer",
      "messageContains": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:1" }
  ]
}
```

## contradicted-every-import-form

The Domain Layer imports Infrastructure three ways: a `using static` of a type, an alias naming a type, and a `global using` of the namespace. Each resolves to `OrderingContext.cs`, so each is an offending import, and the finding names every one with its line. Domain's `global using` of its own SeedWork namespace stays within the Layer and is not named.

```json case
{ "phase": 3, "base": "verified", "kinds": ["layer.dependencies"], "traces": ["ESC-ARC-08"], "requirements": ["R10", "R11", "R46"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
using static eShop.Ordering.Infrastructure.OrderingContext;
using Context = eShop.Ordering.Infrastructure.OrderingContext;

namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Ordering.Domain/GlobalUsings.cs
global using eShop.Ordering.Domain.SeedWork;
global using eShop.Ordering.Infrastructure;
```

```csharp file src/Ordering.Domain/SeedWork/Entity.cs
namespace eShop.Ordering.Domain.SeedWork;

public abstract class Entity
{
    public int Id { get; protected set; }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.dependencies", "element": "domain-layer", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/layer.dependencies", "subject": "domain-layer", "messageContains": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:1" },
    { "rule": "claim/layer.dependencies", "subject": "domain-layer", "messageContains": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:2" },
    { "rule": "claim/layer.dependencies", "subject": "domain-layer", "messageContains": "src/Ordering.Domain/GlobalUsings.cs:2" },
    { "rule": "claim/layer.dependencies", "subject": "domain-layer", "messageContains": "src/Ordering.Domain/GlobalUsings.cs:1", "count": 0,
      "note": "An import within the Layer is never an offending one" }
  ]
}
```

## contradicted-longest-path-wins

The Application Layer's Path lies inside the API Layer's. A command handler under `src/Ordering.API/Application` belongs to Application, the longer Path, and Application may depend only on Domain. Its import of Infrastructure is contradicted even though API may depend on Infrastructure; were the shorter Path to win, the import would pass. Its import of Domain is allowed and not named.

```json case
{ "phase": 3, "base": "verified", "kinds": ["layer.dependencies"], "traces": ["ESC-ARC-08"], "requirements": ["R11", "R46"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
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
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Infrastructure" } } },
          { "id": "api-layer", "type": "Layer", "name": "API",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.API" } } },
          { "id": "application-layer", "type": "Layer", "name": "Application",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.API/Application" } } }
        ]
      }],
      "associations": [
        { "id": "infrastructure-may-depend-on-domain", "type": "May Depend On",
          "source": { "element": "infrastructure-layer" }, "target": { "element": "domain-layer" } },
        { "id": "api-may-depend-on-application", "type": "May Depend On",
          "source": { "element": "api-layer" }, "target": { "element": "application-layer" } },
        { "id": "api-may-depend-on-domain", "type": "May Depend On",
          "source": { "element": "api-layer" }, "target": { "element": "domain-layer" } },
        { "id": "api-may-depend-on-infrastructure", "type": "May Depend On",
          "source": { "element": "api-layer" }, "target": { "element": "infrastructure-layer" } },
        { "id": "application-may-depend-on-domain", "type": "May Depend On",
          "source": { "element": "application-layer" }, "target": { "element": "domain-layer" } }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Application/Commands/CreateOrderCommandHandler.cs
using eShop.Ordering.Domain.AggregatesModel.OrderAggregate;
using eShop.Ordering.Infrastructure;

namespace eShop.Ordering.API.Application.Commands;

public class CreateOrderCommandHandler(OrderingContext context)
{
    public async Task<bool> Handle(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Add(order);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.dependencies", "element": "application-layer", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/layer.dependencies", "severity": "error", "subject": "application-layer",
      "messageContains": "src/Ordering.API/Application/Commands/CreateOrderCommandHandler.cs:2" },
    { "rule": "claim/layer.dependencies", "subject": "application-layer",
      "messageContains": "src/Ordering.API/Application/Commands/CreateOrderCommandHandler.cs:1", "count": 0 },
    { "rule": "claim/layer.dependencies", "subject": "api-layer", "messageContains": "CreateOrderCommandHandler.cs", "count": 0 }
  ]
}
```

## no-rule

Phase 2's house pack states the Layers' dependencies, but its rule, a query, arrives with phase 3. With no active rule reading the kind, each claim is can't verify, `noRule`, reported as an error rather than skipped (P2).

```json case
{ "phase": 2, "base": "verified", "kinds": ["layer.dependencies"], "cases": ["engine/noRule"], "traces": ["ESC-ARC-08"], "requirements": ["R5", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["Codebase Structure"],
  "kinds": [
    { "kind": "layer.dependencies", "label": "layer dependencies", "valueType": "set", "comparer": "within", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Codebase Structure": {
      "entries": [
        {
          "kind": "layer.dependencies",
          "for": { "element": "Layer" },
          "value": "map(associations('May Depend On'), it.element.name)",
          "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
          "own": ["Layer"],
          "source": "the Layer's May Depend On associations"
        }
      ]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.dependencies", "element": "infrastructure-layer", "outcome": "cantVerify", "reason": "noRule" },
    { "kind": "layer.dependencies", "element": "domain-layer", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/layer.dependencies", "severity": "error", "subject": "infrastructure-layer", "messageContains": "no rule in the active packs reads it" }
  ]
}
```

## java-planted-import

The Java spike's planted break: `Order.java` in the domain package imports the application layer's `CatalogService`, and Domain may depend on nothing. Application's `CatalogService` imports a domain class, which its May Depend On allows. The `java.util.List` import names a library class no in-scope file declares, so it is ignored. The query is eShopCustom's, unchanged; only the reader differs, and a Java import resolves to the class's file.

```json case
{ "phase": 4, "kinds": ["layer.dependencies"], "traces": ["JAV-ARC-02"], "requirements": ["R2", "R33", "R46"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "java" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Codebase Structure"],
  "kinds": [
    { "kind": "layer.dependencies", "label": "layer dependencies", "valueType": "set", "comparer": "within", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Codebase Structure": {
      "entries": [
        {
          "kind": "layer.dependencies",
          "for": { "element": "Layer" },
          "value": "map(associations('May Depend On'), it.element.name)",
          "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
          "own": ["Layer"],
          "source": "the Layer's May Depend On associations"
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
          { "id": "application-layer", "type": "Layer", "name": "Application",
            "stereotypes": { "Layer Settings": { "Path": "src/main/java/com/shop/application" } } }
        ]
      }],
      "associations": [
        { "id": "application-may-depend-on-domain", "type": "May Depend On",
          "source": { "element": "application-layer" }, "target": { "element": "domain-layer" } }
      ]
    }
  }
}
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;
import java.util.List;
import com.shop.application.CatalogService;
public class Order {}
```

```java file src/main/java/com/shop/domain/Product.java
package com.shop.domain;

public class Product {
    private String name;
}
```

```java file src/main/java/com/shop/application/CatalogService.java
package com.shop.application;

import com.shop.domain.Product;

public class CatalogService {
    public Product find(String name) {
        return null;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "layer.dependencies", "element": "domain-layer", "outcome": "contradicted",
      "actual": "Application", "at": "src/main/java/com/shop/domain/Order.java:3" },
    { "kind": "layer.dependencies", "element": "application-layer", "outcome": "verified",
      "expected": "Domain", "actual": "Domain", "at": "src/main/java/com/shop/application/CatalogService.java:3" }
  ],
  "findings": [
    { "rule": "claim/layer.dependencies", "severity": "error", "subject": "domain-layer", "messageContains": "src/main/java/com/shop/domain/Order.java:3" },
    { "rule": "claim/layer.dependencies", "subject": "domain-layer", "messageContains": "Order.java:2", "count": 0 }
  ]
}
```
