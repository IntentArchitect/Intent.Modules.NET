# type.exists

A modelled element has a code type: found by name among the in-scope types, narrowed by its conventions' locations (R43). A type of a kind the conventions don't allow is still found, and contradicts `type.kind` instead. It is an existence kind, so a type the code lacks is contradicted (R8), and every claim that depends on it is blocked behind it (R5).

## verified

```json case
{ "phase": 1, "kinds": ["type.exists"], "traces": ["ESC-DOM-12"], "requirements": ["R41", "R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" },
    { "kind": "member.exists", "element": "order-id", "outcome": "verified", "at": "src/Domain/Order.cs:5" }
  ]
}
```

## contradicted-missing

`Order` isn't in the code. One finding says so; its members' claims are can't verify, `blocked`, with no finding of their own.

```json case
{ "phase": 1, "base": "verified", "without": ["src/Domain/Order.cs"], "kinds": ["type.exists", "member.exists"], "cases": ["engine/blocked"], "traces": ["ESC-DOM-12"], "requirements": ["R5", "R8", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "contradicted" },
    { "kind": "member.exists", "element": "order-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "isn't in the code" },
    { "rule": "claim/member.exists", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## ambiguous

Two in-scope classes are named `Order`, in different folders and namespaces, and nothing narrows the choice. The engine doesn't pick one: the claim is can't verify, `ambiguous`, and the finding names both (R43).

```json case
{ "phase": 1, "base": "verified", "kinds": ["type.exists"], "cases": ["engine/ambiguous"], "requirements": ["R5", "R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Reporting/Order.cs
namespace Shop.Reporting;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Reporting/Order.cs:3" }
  ]
}
```

## folder-names-the-directory

Two in-scope classes are named `Order`, and neither namespace ends with the element's `Reporting` folder: the one in `src/Reporting` declares the root namespace. Its folder is the element's, so it is the one.

```json case
{ "phase": 1, "kinds": ["type.exists"], "requirements": ["R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "reporting", "type": "Folder", "name": "Reporting",
            "children": [
              { "id": "order", "type": "Class", "name": "Order" }
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

```csharp file src/Reporting/Order.cs
namespace Shop;

public partial class Order
{
}
```

```csharp file src/Sales/Order.cs
namespace Shop.Sales;

public class Order
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Reporting/Order.cs:3" }
  ]
}
```

## package-breaks-a-folder-tie

Each service keeps its own copy of a message in an `IntegrationEvents/Events` folder, so both in-scope `OrderStockItem`s end with the element's folders equally. Its package, `Catalog.API`, names one of them in its namespace and path, so that one is it. eShop10 is one application spanning every project, and its messages were told apart this way once the Guardrails `File Config` was gone.

```json case
{ "phase": 1, "kinds": ["type.exists"], "requirements": ["R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "catalog", "name": "Catalog.API",
        "elements": [
          { "id": "integration-events", "type": "Folder", "name": "IntegrationEvents",
            "children": [
              { "id": "events", "type": "Folder", "name": "Events",
                "children": [
                  { "id": "stock-item", "type": "Class", "name": "OrderStockItem" }
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

```csharp file src/Ordering.API/IntegrationEvents/Events/OrderStockItem.cs
namespace Shop.Ordering.API.IntegrationEvents.Events;

public record OrderStockItem(int ProductId, int Units);
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs
namespace Shop.Catalog.API.IntegrationEvents.Events;

public record OrderStockItem(int ProductId, int Units);
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "stock-item", "outcome": "verified", "at": "src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs:3" }
  ]
}
```

## library-type

A message the code handles comes from a contracts package it references: it uses `Ping` and declares no type of the name. The type exists, read from its use. Its declaration isn't in the code, so its Property can't be read, and that claim waits on it without a finding.

```json case
{ "phase": 1, "kinds": ["type.exists"], "requirements": ["R2", "R5"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "external", "name": "External.Messages", "type": "Eventing Package",
        "elements": [
          { "id": "ping", "type": "Message", "name": "Ping",
            "children": [
              { "id": "ping-source-address", "type": "Property", "name": "SourceAddress", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Crm", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Diagnostics/PingHandler.cs
using Contracts.Diagnostics;

namespace Crm.Diagnostics;

public class PingHandler : IHandleMessages<Ping>
{
    public Task Handle(Ping message, IMessageHandlerContext context) => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "ping", "outcome": "verified", "at": "src/Diagnostics/PingHandler.cs:5" },
    { "kind": "member.exists", "element": "ping-source-address", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "count": 0 },
    { "rule": "claim/member.exists", "count": 0 }
  ]
}
```

## library-type-unused

The code no longer handles `Ping`, nor uses it anywhere, so the modelled message is in neither the code nor anything it uses.

```json case
{ "phase": 1, "base": "library-type", "kinds": ["type.exists"], "requirements": ["R8"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Diagnostics/PingHandler.cs
namespace Crm.Diagnostics;

public class PingHandler
{
    public Task Handle(string message) => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "ping", "outcome": "contradicted", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "ping" }
  ]
}
```

## out-of-scope

The only `Order` is in a folder a scope row excludes. The file's name matches the type's, so the claim is can't verify, `outOfScope`, naming the file and the row, rather than contradicted (R25). The excluded file is never read.

```json case
{ "phase": 1, "base": "verified", "without": ["src/Domain/Order.cs"], "kinds": ["type.exists"], "cases": ["scope/shadow", "scope/notRead"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" }
        ] } } }]
    }
  }
}
```

```csharp file src/Legacy/Order.cs
namespace Shop.Legacy;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" },
    { "kind": "member.exists", "element": "order-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Legacy/**" }
  ],
  "filesRead": { "exclude": ["src/Legacy/**"] }
}
```

## services-dto-verified

A Services designer DTO is a record of that name.

```json case
{ "phase": 1, "kinds": ["type.exists"], "traces": ["ESC-SVC-11"], "requirements": ["R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
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

```csharp file src/Application/Orders/OrderDto.cs
namespace Shop.Application.Orders;

public record OrderDto(Guid Id);
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-dto", "outcome": "verified", "at": "src/Application/Orders/OrderDto.cs:3" }
  ]
}
```

## java-verified

The same claim over Java, through the Java reader and language profile.

```json case
{ "phase": 4, "kinds": ["type.exists"], "traces": ["JAV-DOM-02"], "requirements": ["R33", "R41"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
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
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import java.util.UUID;

public class Order {
    private UUID id;

    public UUID getId() {
        return id;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/main/java/com/shop/domain/Order.java:5" }
  ]
}
```

## repository-interface

eShopCustom describes a Repository by its interface, named `I` and the Repository's name; Common.Drift's repository contract was the concrete class. The house pack's conventions name the type `I{Name}` and allow only an interface (R43, R51).

```json case
{ "phase": 2, "kinds": ["type.exists"], "traces": ["ESC-DOM-69"], "requirements": ["R2", "R28", "R43", "R53"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "type.exists",
          "for": { "element": "Repository" },
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "source": "the Repository",
          "fixHint": "Add the repository interface, I followed by the Repository's name."
        }
      ]
    }
  },
  "conventions": {
    "Domain": {
      "types": {
        "Repository": { "typeKinds": ["interface"], "name": "I{Name}", "note": "eShopCustom describes a Repository by its interface" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order" },
          { "id": "order-repository", "type": "Repository", "name": "OrderRepository", "typeRef": { "element": "order" } }
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

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public interface IOrderRepository
{
    Order Add(Order order);
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-repository", "outcome": "verified",
      "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:3" }
  ]
}
```

## repository-interface-missing

The interface is gone and only the concrete `OrderRepository` class is left. The conventions name the Repository's type `IOrderRepository`, and no type of any kind has that name, so the Repository's type isn't in the code: contradicted.

```json case
{ "phase": 2, "base": "repository-interface", "without": ["src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs"], "kinds": ["type.exists"], "traces": ["ESC-DOM-69"], "requirements": ["R1", "R8", "R43"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.Infrastructure/Repositories/OrderRepository.cs
namespace eShop.Ordering.Infrastructure.Repositories;

public class OrderRepository(OrderingContext context)
{
    public Order Add(Order order) => context.Orders.Add(order).Entity;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-repository", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order-repository", "messageContains": "isn't in the code" }
  ]
}
```

## proto-message

eShopCustom's `Proto Message` stereotype says a DTO is a message in a `.proto` file, not a C# type. The Services pack's vocabulary still claims the DTO's type. The Protobuf reader makes each `message` declaration a type node, so the structure rule locates the DTO by name like any type.

```json case
{ "phase": 2, "kinds": ["type.exists"], "traces": ["ESC-SVC-82"], "requirements": ["R2"], "packs": ["intent.core", "intent.services"], "language": "protobuf" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-item", "type": "DTO", "name": "BasketItem",
            "stereotypes": { "Proto Message": {} } }
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

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

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
    { "kind": "type.exists", "element": "basket-item", "outcome": "verified", "at": "src/Basket.API/Proto/basket.proto:9" }
  ]
}
```

## proto-message-missing

The `.proto` no longer declares `BasketItem`; `CustomerBasketResponse` still names it as a field type, which isn't a declaration. A `.proto` reaches a type no in-scope file declares only through an import of a file the engine doesn't read, and this one imports nothing, so `BasketItem` isn't anywhere.

```json case
{ "phase": 2, "base": "proto-message", "kinds": ["type.exists"], "traces": ["ESC-SVC-82"], "requirements": ["R1", "R8"], "packs": ["intent.core", "intent.services"], "language": "protobuf" }
```

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

package BasketApi;

message CustomerBasketResponse {
    repeated BasketItem items = 1;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "basket-item", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "basket-item" }
  ]
}
```

## proto-message-imported

The `.proto` doesn't declare `BasketItem`, but it imports `catalog/basket_item.proto`, a file the engine doesn't read. The message may be declared there, as a library's type is, so the field that names it says it exists.

```json case
{ "phase": 2, "base": "proto-message", "kinds": ["type.exists"], "traces": ["ESC-SVC-82"], "requirements": ["R2"], "packs": ["intent.core", "intent.services"], "language": "protobuf" }
```

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

package BasketApi;

import "catalog/basket_item.proto";

message CustomerBasketResponse {
    repeated BasketItem items = 1;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "basket-item", "outcome": "verified", "at": "src/Basket.API/Proto/basket.proto:8" }
  ]
}
```

## front-end-types

eShopCustom's Front End module checked that each Model Definition is a record, class or struct of its name, and each Service Proxy a class. No stock pack before phase 6 knows the User Interface designer, so the house pack gives both their vocabulary and conventions (R28, R43).

```json case
{ "phase": 2, "kinds": ["type.exists"], "traces": ["ESC-FE-39", "ESC-FE-50"], "requirements": ["R2", "R28", "R43", "R53"], "packs": ["intent.core", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "vocabulary": {
    "User Interface": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Model Definition" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Model Definition" },
        { "kind": "type.exists", "for": { "element": "Service Proxy" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Service Proxy" }
      ]
    }
  },
  "conventions": {
    "User Interface": {
      "types": {
        "Model Definition": { "typeKinds": ["record", "class", "struct"] },
        "Service Proxy": { "typeKinds": ["class"] }
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
        "id": "web-app-components", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog-brand", "type": "Model Definition", "name": "CatalogBrand" },
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService" }
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

```csharp file src/WebAppComponents/Catalog/CatalogItem.cs
namespace eShop.WebAppComponents.Catalog;

public record CatalogItem(int Id, string Name, decimal Price, CatalogBrand CatalogBrand);

public record CatalogBrand(int Id, string Brand);
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    public Task<CatalogItem?> GetCatalogItem(int id)
        => httpClient.GetFromJsonAsync<CatalogItem>($"api/catalog/items/{id}");
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "catalog-brand", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:5" },
    { "kind": "type.exists", "element": "catalog-service", "outcome": "verified", "at": "src/WebAppComponents/Services/CatalogService.cs:5" }
  ]
}
```

## model-definition-interface

`CatalogBrand` is declared as an interface. eShopCustom checked that a Model Definition is a record, class or struct, not an interface or enum. The type is located by name whatever its kind, so `type.exists` is verified, and the kind is `type.kind`'s to report: the interface isn't among the kinds the house pack's conventions allow (R43).

```json case
{ "phase": 2, "base": "front-end-types", "kinds": ["type.exists", "type.kind"], "traces": ["ESC-FE-39"], "requirements": ["R1", "R11", "R43"], "packs": ["intent.core", "house.eshop"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Catalog/CatalogItem.cs
namespace eShop.WebAppComponents.Catalog;

public record CatalogItem(int Id, string Name, decimal Price, CatalogBrand CatalogBrand);

public interface CatalogBrand
{
    int Id { get; }
    string Brand { get; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "catalog-brand", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:5" },
    { "kind": "type.kind", "element": "catalog-brand", "outcome": "contradicted", "expected": "class, record, struct", "actual": "interface",
      "at": "src/WebAppComponents/Catalog/CatalogItem.cs:5" }
  ],
  "findings": [
    { "rule": "claim/type.kind", "severity": "error", "subject": "catalog-brand", "messageContains": "interface" },
    { "rule": "claim/type.exists", "subject": "catalog-brand", "count": 0 }
  ]
}
```

## spring-controller

The Java spike's Services are Spring controllers: a Service is the class named after it, with `Service` replaced by `Controller`. The storefront's house pack names a Service's type that way in its conventions, so the structure rule locates `OrdersController` (R33).

```json case
{ "phase": 4, "kinds": ["type.exists"], "traces": ["JAV-SVC-02"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.storefront"], "language": "java" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "name": "{Name|trimEnd:Service}Controller", "note": "A Service is its controller class" }
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
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "orders-service", "type": "Service", "name": "OrdersService" }
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

```java file src/main/java/com/shop/api/OrdersController.java
package com.shop.api;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/orders")
public class OrdersController {
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "orders-service", "outcome": "verified", "at": "src/main/java/com/shop/api/OrdersController.java:6" }
  ]
}
```

## spring-controller-not-rest

The class is an MVC `@Controller`, not a `@RestController`. The class of the Service's name is there, so the type is verified; the house pack also states that a Service's class is a `@RestController`, as a marker the structure rule reads from its annotations, and that is contradicted.

```json case
{ "phase": 4, "base": "spring-controller", "kinds": ["type.exists", "type.marker"], "traces": ["JAV-SVC-02"], "requirements": ["R1", "R33", "R41"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.storefront"], "language": "java" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        { "kind": "type.marker", "for": { "element": "Service" }, "discriminator": "'RestController'", "value": "true", "dependsOn": "type.exists", "source": "the storefront's rule that a Service is a REST controller" }
      ]
    }
  },
  "conventions": {
    "Services": {
      "types": {
        "Service": { "name": "{Name|trimEnd:Service}Controller", "note": "A Service is its controller class" }
      }
    }
  }
}
```

```java file src/main/java/com/shop/api/OrdersController.java
package com.shop.api;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;

@Controller
@RequestMapping("/orders")
public class OrdersController {
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "orders-service", "outcome": "verified" },
    { "kind": "type.marker", "element": "orders-service", "key": "RestController", "outcome": "contradicted", "expected": "true", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/type.marker", "severity": "error", "subject": "orders-service" }
  ]
}
```

## java-record-dto

The Java spike's DTOs are Java records. The storefront's house pack allows a DTO only a record. It requires the Services pack, so its conventions for DTOs override that pack's (rules.md, Which conventions apply).

```json case
{ "phase": 4, "kinds": ["type.exists"], "traces": ["JAV-SVC-18"], "requirements": ["R2", "R33", "R43"], "packs": ["intent.core", "intent.services", "intent.java", "house.storefront"], "language": "java" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "DTO": { "typeKinds": ["record"], "note": "The storefront's DTOs are Java records" }
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
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "line-item-dto", "type": "DTO", "name": "LineItemDto" }
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

```java file src/main/java/com/shop/api/dto/LineItemDto.java
package com.shop.api.dto;

import java.util.UUID;

public record LineItemDto(UUID id, UUID productId, int quantity) {
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "line-item-dto", "outcome": "verified", "at": "src/main/java/com/shop/api/dto/LineItemDto.java:5" }
  ]
}
```

## java-class-dto

`LineItemDto` is a plain class with getters, not a record. The type is still the DTO's, so `type.exists` is verified, and its kind contradicts the house pack's `record` (R43).

```json case
{ "phase": 4, "base": "java-record-dto", "kinds": ["type.exists", "type.kind"], "traces": ["JAV-SVC-18"], "requirements": ["R1", "R11", "R43"], "packs": ["intent.core", "intent.services", "intent.java", "house.storefront"], "language": "java" }
```

```java file src/main/java/com/shop/api/dto/LineItemDto.java
package com.shop.api.dto;

import java.util.UUID;

public class LineItemDto {
    private UUID id;
    private int quantity;

    public UUID getId() {
        return id;
    }

    public int getQuantity() {
        return quantity;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "line-item-dto", "outcome": "verified", "at": "src/main/java/com/shop/api/dto/LineItemDto.java:5" },
    { "kind": "type.kind", "element": "line-item-dto", "outcome": "contradicted", "expected": "record", "actual": "class",
      "at": "src/main/java/com/shop/api/dto/LineItemDto.java:5" }
  ],
  "findings": [
    { "rule": "claim/type.kind", "severity": "error", "subject": "line-item-dto" }
  ]
}
```

## vue-contract

The Java spike's Front End designer has Contracts, the API's types as the Vue app sees them: an `export interface` or `export type` of that name in a TypeScript file in its folder. The TypeScript reader arrives later, with intent.typescript, so at phase 4 the storefront's house pack finds the declaration as text (R38). A `file with` find can't name the element's folder, so this case doesn't check the folder; `vue-contract-in-folder` does, once the reader is there.

```json case
{ "phase": 4, "kinds": ["type.exists"], "traces": ["JAV-FE-03"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.java", "house.storefront"], "language": "typescript" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Contract" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Contract" }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "api-folder", "type": "Folder", "name": "api",
            "children": [
              { "id": "product-dto", "type": "Contract", "name": "ProductDto" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Storefront", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/api/types.ts
export interface ProductDto {
  Id: string;
  Name: string;
  Price: number;
}

export type OrderStatus = 'Placed' | 'Shipped' | 'Delivered';
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "product-dto", "outcome": "verified", "at": "src/api/types.ts:1" }
  ]
}
```

## vue-contract-missing

The interface was renamed `Product` in the code and not in the model.

```json case
{ "phase": 4, "base": "vue-contract", "kinds": ["type.exists"], "traces": ["JAV-FE-03"], "requirements": ["R1", "R8", "R38"], "packs": ["intent.core", "intent.java", "house.storefront"], "language": "typescript" }
```

```typescript file src/api/types.ts
export interface Product {
  Id: string;
  Name: string;
  Price: number;
}

export type OrderStatus = 'Placed' | 'Shipped' | 'Delivered';
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "product-dto", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "product-dto" }
  ]
}
```

## vue-contract-in-folder

With the TypeScript reader (intent.typescript, `later`), an exported interface or type alias is a type node, so the structure comparison locates a Contract like any type. The storefront's conventions allow an interface or a type alias, in a `.ts` file in the folder that mirrors the Contract's designer folders, `{FolderPath}`: the whole check, folder included.

```json case
{ "phase": "later", "kinds": ["type.exists"], "traces": ["JAV-FE-03"], "requirements": ["R2", "R33", "R43"], "packs": ["intent.core", "intent.java", "intent.typescript", "house.storefront"], "language": "typescript" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.typescript", "version": "^1.0.0" }],
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Contract" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Contract" }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Contract": { "typeKinds": ["interface", "typeAlias"], "locations": ["src/{FolderPath}/*.ts"], "note": "A Contract is declared in its designer folder" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "api-folder", "type": "Folder", "name": "api",
            "children": [
              { "id": "product-dto", "type": "Contract", "name": "ProductDto" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Storefront", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/api/types.ts
export interface ProductDto {
  Id: string;
  Name: string;
  Price: number;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "product-dto", "outcome": "verified", "at": "src/api/types.ts:1" }
  ]
}
```

## vue-contract-outside-folder

`ProductDto` is declared beside the cart store instead of in `api`. Nothing named `ProductDto` is in the Contract's folder, so the Contract's type isn't where its conventions look: contradicted.

```json case
{ "phase": "later", "base": "vue-contract-in-folder", "without": ["src/api/types.ts"], "kinds": ["type.exists"], "traces": ["JAV-FE-03"], "requirements": ["R1", "R8", "R43"], "packs": ["intent.core", "intent.java", "intent.typescript", "house.storefront"], "language": "typescript" }
```

```typescript file src/stores/types.ts
export interface ProductDto {
  Id: string;
  Name: string;
  Price: number;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "product-dto", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "product-dto" }
  ]
}
```
