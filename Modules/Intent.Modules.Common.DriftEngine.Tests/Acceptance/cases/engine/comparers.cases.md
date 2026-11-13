# engine/comparers

Each claim kind names its comparer, and both sides are trimmed first (R10). One case per comparer, each on a kind that uses it, showing what that comparer accepts or rejects that plain equality wouldn't. The kinds' own case files cover every outcome; these show the comparers side by side. Kinds that no stock pack reads take their rule from a house pack, as they will in a real codebase.

The `version` comparer has no case: its only kind, `proxy.version`, takes its model value from the backend operation a proxy operation is mapped to, which vocabulary can't yet read.

## exact

`exact` compares text ordinally, so case matters. The model names the table `Orders`; the code says `orders`. SQL Server may not care, but the model's statement and the code's differ, so the claim is contradicted.

```json case
{ "phase": 4, "kinds": ["table.name"], "cases": ["engine/comparers"], "requirements": ["R10"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
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
        builder.ToTable("orders");

        builder.HasKey(x => x.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "contradicted", "expected": "Orders", "actual": "orders", "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## ignore-case

`ignoreCase` compares text ignoring case: SQL type names don't depend on it. The model's `nvarchar(200)` and the code's `NVARCHAR(200)` are the same column type.

```json case
{ "phase": 4, "kinds": ["column.type"], "cases": ["engine/comparers"], "requirements": ["R10"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "product", "type": "Class", "name": "Product",
            "children": [
              { "id": "product-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": {} } },
              { "id": "product-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" },
                "stereotypes": { "Column": { "Type": "nvarchar(200)" } } }
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

```csharp file src/Domain/Product.cs
namespace Shop.Domain;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";
}
```

```csharp file src/Infrastructure/Persistence/ProductConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasColumnType("NVARCHAR(200)");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "column.type", "element": "product-name", "outcome": "verified", "expected": "nvarchar(200)", "actual": "NVARCHAR(200)", "at": "src/Infrastructure/Persistence/ProductConfiguration.cs:13" }
  ]
}
```

## number

`number` compares the two sides as numbers, in the invariant culture. The model gives `Paid` the value `2` and the code states `Paid = 2`.

```json case
{ "phase": 1, "kinds": ["enum.literal.value"], "cases": ["engine/comparers"], "requirements": ["R10"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-draft", "type": "Enum-Literal", "name": "Draft" },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid", "value": "2" }
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

```csharp file src/Domain/OrderStatus.cs
namespace Shop.Domain;

public enum OrderStatus
{
    Draft,
    Paid = 2,
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literal.value", "element": "order-status-paid", "outcome": "verified", "expected": "2", "actual": "2", "at": "src/Domain/OrderStatus.cs:6" },
    { "kind": "enum.literal.value", "element": "order-status-draft", "absent": true, "note": "whenSet: no modelled value, no claim" }
  ]
}
```

## list

`list` compares items in order. The model's composite key is `OrderId` then `ProductId`, as its attributes are ordered; the code declares the same two columns the other way round, which is a different key.

```json case
{ "phase": 4, "kinds": ["key"], "cases": ["engine/comparers"], "requirements": ["R10"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "children": [
              { "id": "order-line-order-id", "type": "Attribute", "name": "OrderId", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": {} } },
              { "id": "order-line-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": {} } },
              { "id": "order-line-quantity", "type": "Attribute", "name": "Quantity", "typeRef": { "type": "int" } }
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

```csharp file src/Domain/OrderLine.cs
namespace Shop.Domain;

public class OrderLine
{
    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}
```

```csharp file src/Infrastructure/Persistence/OrderLineConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.HasKey(x => new { x.ProductId, x.OrderId });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "key", "element": "order-line", "outcome": "contradicted", "at": "src/Infrastructure/Persistence/OrderLineConfiguration.cs:11" }
  ]
}
```

## subsequence

`subsequence` verifies when the model's items appear in the code in the model's order, with any others between them. The model lists `Draft, Paid`; the code has `Draft, Submitted, Paid`. An unmodelled literal is not a finding unless an opt-in rule asks for one.

```json case
{ "phase": 1, "kinds": ["enum.literals"], "cases": ["engine/comparers"], "requirements": ["R10", "R44"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-draft", "type": "Enum-Literal", "name": "Draft" },
              { "id": "order-status-paid", "type": "Enum-Literal", "name": "Paid" }
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

```csharp file src/Domain/OrderStatus.cs
namespace Shop.Domain;

public enum OrderStatus
{
    Draft,
    Submitted,
    Paid,
}
```

```json expect
{
  "claims": [
    { "kind": "enum.literals", "element": "order-status", "outcome": "verified" }
  ]
}
```

## set

`set` compares items in any order. The model's Roles are `Admin, Manager`; the controller requires `Manager,Admin`.

```json case
{ "phase": 4, "kinds": ["security.roles"], "cases": ["engine/comparers"], "requirements": ["R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "orders-service", "type": "Service", "name": "OrdersService",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "{id}/cancel" }, "Secured": { "Roles": "Admin, Manager" } },
                "children": [
                  { "id": "cancel-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
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

```csharp file src/Shop.Api/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return NoContent();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security.roles", "element": "cancel-order", "outcome": "verified", "expected": "Admin, Manager", "actual": "Admin, Manager", "at": "src/Shop.Api/Controllers/OrdersController.cs:11" }
  ]
}
```

## subset

`subset` verifies when every model item is in the code, which may have more. The house pack says a Domain Event implies MediatR's `INotification`; the event implements that and one more interface of the codebase's own.

```json case
{ "phase": 2, "kinds": ["type.base"], "cases": ["engine/comparers"], "requirements": ["R10", "R28"], "packs": ["intent.core", "intent.domain", "house.shop"], "language": "csharp" }
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
      "entries": [
        {
          "kind": "type.base",
          "for": { "element": "Domain Event" },
          "value": "'INotification'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the element being a Domain Event",
          "fixHint": "Implement MediatR's INotification."
        }
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
          { "id": "order-paid", "type": "Domain Event", "name": "OrderPaidDomainEvent" }
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

```csharp file src/Domain/Events/OrderPaidDomainEvent.cs
using MediatR;
using Shop.Domain.SeedWork;

namespace Shop.Domain.Events;

public record OrderPaidDomainEvent(Guid OrderId) : INotification, IAuditedEvent;
```

```csharp file src/Domain/SeedWork/IAuditedEvent.cs
namespace Shop.Domain.SeedWork;

public interface IAuditedEvent
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "order-paid", "outcome": "verified" }
  ]
}
```

## within

`within` verifies when every code item is in the model, which may allow more. Infrastructure may depend on Domain and Application, and its code imports only from Domain.

```json case
{ "phase": 3, "kinds": ["layer.dependencies"], "cases": ["engine/comparers"], "requirements": ["R10", "R47"], "packs": ["intent.core", "house.shop"], "language": "csharp" }
```

```json pack
{
  "id": "house.shop",
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
  },
  "rules": {
    "Codebase Structure": {
      "facts": {
        "layer.dependencies": {
          "query": [
            { "from": "code:import" },
            { "ownerOf": { "path": "file", "among": "model:Layer", "by": "stereotype('Layer Settings').Path", "as": "fromLayer" } },
            { "resolve": { "target": "target", "as": "imported" } },
            { "ownerOf": { "path": "imported.file", "among": "model:Layer", "by": "stereotype('Layer Settings').Path", "as": "toLayer" } },
            { "where": "exists(fromLayer) && exists(toLayer) && fromLayer != toLayer" },
            { "reading": { "claim": { "Layer": "fromLayer.name" }, "item": "toLayer.name", "at": "span" } }
          ],
          "compare": "within"
        }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/Domain" } } },
          { "id": "application-layer", "type": "Layer", "name": "Application",
            "stereotypes": { "Layer Settings": { "Path": "src/Application" } } },
          { "id": "infrastructure-layer", "type": "Layer", "name": "Infrastructure",
            "stereotypes": { "Layer Settings": { "Path": "src/Infrastructure" } } }
        ] }],
      "associations": [
        { "id": "application-may-depend-on-domain", "type": "May Depend On",
          "source": { "element": "application-layer" }, "target": { "element": "domain-layer" } },
        { "id": "infrastructure-may-depend-on-domain", "type": "May Depend On",
          "source": { "element": "infrastructure-layer" }, "target": { "element": "domain-layer" } },
        { "id": "infrastructure-may-depend-on-application", "type": "May Depend On",
          "source": { "element": "infrastructure-layer" }, "target": { "element": "application-layer" } }
      ]
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

```csharp file src/Infrastructure/Persistence/ShopDbContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class ShopDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
}
```

```json expect
{
  "claims": [
    { "kind": "layer.dependencies", "element": "infrastructure-layer", "outcome": "verified", "at": "src/Infrastructure/Persistence/ShopDbContext.cs:2" }
  ]
}
```

## contains

`contains` verifies when the code's text contains the model's. The page's Title is `Order history`; its `<PageTitle>` adds the site's name.

```json case
{ "phase": "later", "kinds": ["page.title"], "cases": ["engine/comparers"], "requirements": ["R10", "R28"], "packs": ["intent.core", "house.shop"], "language": "csharp" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "page.title", "label": "page title", "valueType": "text", "comparer": "contains", "stated": "whenSet", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "page.title",
          "for": { "element": "Component", "where": "hasStereotype('Page')" },
          "value": "stereotype('Page').Title",
          "bindings": { "Component": "name" },
          "own": ["Component"],
          "source": "the Page stereotype's Title"
        }
      ]
    }
  },
  "rules": {
    "User Interface": {
      "facts": {
        "page.title": {
          "find": "file:src/WebApp/Components/**/{Component}.razor",
          "read": [{ "match": "<PageTitle>([^<@]*)</PageTitle>" }]
        }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "Shop.WebApp",
        "elements": [
          { "id": "order-history", "type": "Component", "name": "OrderHistory",
            "stereotypes": { "Page": { "Route": "/orders", "Title": "Order history" } } }
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

```razor file src/WebApp/Components/Pages/OrderHistory.razor
@page "/orders"

<PageTitle>Order history | Shop</PageTitle>

<h1>Order history</h1>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "order-history", "outcome": "verified" }
  ]
}
```

## type

`type` maps the model's type through the language profile before comparing. The model's `Tags` is a collection of `string`; the code's `List<string>` is one of C#'s collection forms.

```json case
{ "phase": 1, "kinds": ["member.type"], "cases": ["engine/comparers"], "requirements": ["R10", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "product", "type": "Class", "name": "Product",
            "children": [
              { "id": "product-tags", "type": "Attribute", "name": "Tags", "typeRef": { "type": "string", "collection": true } }
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

```csharp file src/Domain/Product.cs
namespace Shop.Domain;

public class Product
{
    public List<string> Tags { get; set; } = [];
}
```

```json expect
{
  "claims": [
    { "kind": "member.type", "element": "product-tags", "outcome": "verified", "at": "src/Domain/Product.cs:5" }
  ]
}
```

## route

`route` ignores a leading or trailing `/`, case in literal segments, and parameter constraints. The model's operation route is `{orderId}`; the endpoint maps `/{orderId:int}`.

```json case
{ "phase": 4, "kinds": ["operation.route"], "cases": ["engine/comparers"], "requirements": ["R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{orderId}" } },
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

        api.MapGet("/{orderId:int}", GetOrderAsync);

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
  "claims": [
    { "kind": "operation.route", "element": "get-order", "outcome": "verified", "at": "src/Shop.Api/Apis/OrdersApi.cs:9" }
  ]
}
```

## presence

`presence` verifies when the code has the thing exactly when the model says it's present. The model has `Order.Reference`; the class has no such property, so the claim is contradicted, `present` against `absent` (R8).

```json case
{ "phase": 1, "kinds": ["member.exists"], "cases": ["engine/comparers"], "requirements": ["R8", "R10"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-reference", "type": "Attribute", "name": "Reference", "typeRef": { "type": "string" } }
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

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-id", "outcome": "verified", "at": "src/Domain/Order.cs:5" },
    { "kind": "member.exists", "element": "order-reference", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ]
}
```
