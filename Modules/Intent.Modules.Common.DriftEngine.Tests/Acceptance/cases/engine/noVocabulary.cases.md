# engine/noVocabulary

Nothing modelled is skipped silently (R6, R7). For each designer with content, the engine works out every model feature it holds (an element's specialization, each applied stereotype, each stereotype property set away from its default, each association's specialization, each type reference) and which features the active packs' vocabulary reads. A feature nothing reads is an `engine/noVocabulary` error, once per element and feature, in the designer's `<Designer> (DriftEngine)` Area with the element as its subject. A designer with content and no vocabulary at all is reported once instead. A pack can declare a feature non-checkable, with a reason, and it is then not reported (R32). A non-claim only explains what nothing reads: a pack that reads the feature overrides another's non-claim for it, so its claims are checked and what it leaves unread is reported.

eShopCustom's Domain, Services and Front End modules, and the Java spike's Domain check, each failed the run on model content they couldn't compare (`unsupported.model`), so the model could never claim more than the check verified. The engine keeps that rule for every designer, with the list of what can be compared coming from packs rather than from each module's code.

## unread-stereotype

`Order` carries the Cosmos DB module's `Container` stereotype, with its properties at their defaults. No active pack says what it states in code, so it is reported on `Order`, naming the stereotype. The rest of `Order` is still checked: its type is verified.

```json case
{ "phase": 1, "cases": ["engine/noVocabulary"], "traces": ["ESC-DOM-06"], "requirements": ["R6"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Container": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "string" } }
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
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "engine/noVocabulary", "severity": "error", "subject": "order", "messageContains": "Container" }
  ]
}
```

## declared-non-checkable

The same model, with a house pack that declares the `Container` stereotype non-checkable and says why. Nothing is reported about it, and the coverage listing shows the non-claim with its reason, so a reviewer still sees what isn't checked.

```json case
{ "phase": 2, "base": "unread-stereotype", "cases": ["engine/noVocabulary", "scope/coverage"], "requirements": ["R6", "R15", "R32"], "packs": ["intent.core", "intent.domain", "house.shop"], "language": "csharp" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "nonClaims": [
        { "for": { "stereotype": "Container" }, "reason": "Cosmos DB container settings are applied at deployment, not in code" }
      ]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "engine/noVocabulary", "subject": "order", "count": 0 },
    { "rule": "scope/coverage", "severity": "info", "messageContains": "Cosmos DB container settings are applied at deployment, not in code" }
  ]
}
```

## claimed-overrides-a-stereotype-non-claim

`Order` carries a house `Placement` stereotype. One house pack declares the whole stereotype non-checkable; another reads its Path as the class's file. Reading it overrides the non-claim: the file is checked, the non-claim isn't listed, and Placement's Region, which nothing reads, is reported.

```json case
{ "phase": 2, "cases": ["engine/noVocabulary", "scope/coverage"], "requirements": ["R6", "R32"], "packs": ["intent.core", "intent.domain", "house.legacy", "house.shop"], "language": "csharp" }
```

```json pack house.legacy
{
  "id": "house.legacy",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "nonClaims": [
        { "for": { "stereotype": "Placement" }, "reason": "Placement told the old generator where to write a class" }
      ]
    }
  }
}
```

```json pack house.shop
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        { "kind": "type.location", "for": { "element": "Class", "where": "hasStereotype('Placement')" }, "value": "stereotype('Placement').Path", "dependsOn": "type.exists", "source": "the class's Placement Path" }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Placement": { "Path": "src/Domain/Order.cs", "Region": "EU" } },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "string" } }
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
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
```

```json expect
{
  "claims": [
    { "kind": "type.location", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "engine/noVocabulary", "severity": "error", "subject": "order", "messageContains": "Region" },
    { "rule": "scope/coverage", "messageContains": "old generator", "count": 0 }
  ]
}
```

## claimed-overrides-a-designer-non-claim

The model of `unread-stereotype`, with a house pack that declares the whole Domain designer non-checkable. The Domain pack reads the designer, so the non-claim isn't used: `Order` is checked, its unread `Container` stereotype is still reported, and the non-claim isn't listed.

```json case
{ "phase": 2, "base": "unread-stereotype", "cases": ["engine/noVocabulary", "scope/coverage"], "requirements": ["R7", "R32"], "packs": ["intent.core", "intent.domain", "house.legacy"], "language": "csharp" }
```

```json pack
{
  "id": "house.legacy",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "nonClaims": [
        { "reason": "The old generator owned the Domain designer" }
      ]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "engine/noVocabulary", "severity": "error", "subject": "order", "messageContains": "Container" },
    { "rule": "scope/coverage", "messageContains": "old generator", "count": 0 }
  ]
}
```

## unread-stereotype-property

The Column stereotype is read: its Name and Type are claims. Its Collation isn't: no kind states it, and JPA's `@Column` has no collation to compare it with. The property is set, so it is reported on the attribute, naming it. The team removes it from the model, or a pack declares it non-checkable.

The Java spike's Domain check required removing the stereotype properties it couldn't verify (Column Type, Order and Collation among them); Column Type is now the `column.type` claim, and the rest are this finding.

```json case
{ "phase": 4, "cases": ["engine/noVocabulary"], "traces": ["JAV-DOM-08"], "requirements": ["R6"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
              { "id": "customer-id", "type": "Attribute", "name": "id", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": {} } },
              { "id": "customer-email", "type": "Attribute", "name": "email", "typeRef": { "type": "string" },
                "stereotypes": { "Column": { "Collation": "Latin1_General_CI_AS" } } }
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

```java file src/main/java/com/shop/domain/Customer.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Customer {

    @Id
    private UUID id;

    private String email;

    public UUID getId() {
        return id;
    }

    public String getEmail() {
        return email;
    }
}
```

```json expect
{
  "findings": [
    { "rule": "engine/noVocabulary", "severity": "error", "subject": "customer-email", "messageContains": "Collation" }
  ]
}
```

## unread-stereotype-property-services

`GetOrder`'s Http Settings set Return Type Mediatype to `application/json`. Its Verb and Route are claims; no kind states the media type, so the property is reported on the operation. The Services pack still claims everything else on the operation.

eShopCustom's Services module listed Return Type Mediatype among the model content it couldn't compare, as a model-side Unsupported error.

```json case
{ "phase": 1, "cases": ["engine/noVocabulary"], "traces": ["ESC-SVC-08"], "requirements": ["R6"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "get-order", "type": "Operation", "name": "GetOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{orderId}", "Return Type Mediatype": "application/json" } },
                "children": [
                  { "id": "get-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] }
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

```csharp file src/Shop.Api/Apis/OrdersApi.cs
namespace Shop.Api.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet("{orderId:int}", GetOrderAsync);

        return api;
    }

    public static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        var order = await services.Queries.GetOrderAsync(orderId);
        return order is null ? TypedResults.NotFound() : TypedResults.Ok(order);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "engine/noVocabulary", "severity": "error", "subject": "get-order", "messageContains": "Return Type Mediatype" }
  ]
}
```

## designer-without-vocabulary

The application models its Blazor front end in the User Interface designer, but the active packs give that designer no vocabulary: phase 1's stock packs cover Domain and Services only. The designer is reported once, with its element count, rather than once per element and feature.

eShopCustom's Front End module failed the run on any model content it couldn't compare. A whole designer no pack reads is the largest case of that.

```json case
{ "phase": 1, "cases": ["engine/noVocabulary"], "traces": ["ESC-FE-07"], "requirements": ["R7"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "Shop.WebApp",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "Pages",
            "children": [
              { "id": "order-history", "type": "Component", "name": "OrderHistory",
                "stereotypes": { "Page": { "Route": "/orders", "Title": "Order history" } } }
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

```razor file src/WebApp/Components/Pages/OrderHistory.razor
@page "/orders"

<PageTitle>Order history</PageTitle>

<h1>Order history</h1>
```

```json expect
{
  "findings": [
    { "rule": "engine/noVocabulary", "severity": "error", "messageContains": "User Interface" },
    { "rule": "engine/noVocabulary", "subject": "order-history", "count": 0, "note": "One finding for the designer, not one per element" }
  ]
}
```
