# operation.parameters

A modelled operation or constructor has the parameters the model lists, by name and type, in order; the code side is the method's or constructor's parameter list, and the `list` comparer needs the same items in the same order (rules.md, What is compared). Each item's type maps through the language profile as `parameter.type` does. With one method of the operation's name, that method is the operation's; parameter types choose only among overloads (rules.md, Locating an element's code). It is a `stated: always` kind that depends on the member: when the method isn't there, the claim is blocked. The cases don't spell out `expected` and `actual`, since the canonical text of a parameter item isn't defined.

## verified

`Order.AddOrderItem`'s four parameters are the method's four, with the same names and types in the same order.

```json case
{ "phase": 1, "kinds": ["operation.parameters"], "traces": ["ESC-DOM-64"], "requirements": ["R2", "R10", "R41", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-add-order-item", "type": "Operation", "name": "AddOrderItem",
                "children": [
                  { "id": "order-add-order-item-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "int" } },
                  { "id": "order-add-order-item-product-name", "type": "Parameter", "name": "productName", "typeRef": { "type": "string" } },
                  { "id": "order-add-order-item-unit-price", "type": "Parameter", "name": "unitPrice", "typeRef": { "type": "decimal" } },
                  { "id": "order-add-order-item-units", "type": "Parameter", "name": "units", "typeRef": { "type": "int" } }
                ] }
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

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public void AddOrderItem(int productId, string productName, decimal unitPrice, int units)
    {
        _orderItems.Add(new OrderItem(productId, productName, unitPrice, units));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "count": 0 }
  ]
}
```

## contradicted-order

The method takes `units` before `unitPrice`. The same parameters in another order are a different signature: contradicted.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-64"], "requirements": ["R1", "R10", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public void AddOrderItem(int productId, string productName, int units, decimal unitPrice)
    {
        _orderItems.Add(new OrderItem(productId, productName, unitPrice, units));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-add-order-item" }
  ]
}
```

## contradicted-missing

The method has no `units` parameter: contradicted.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-64"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public void AddOrderItem(int productId, string productName, decimal unitPrice)
    {
        _orderItems.Add(new OrderItem(productId, productName, unitPrice, 1));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-add-order-item" }
  ]
}
```

## blocked

`Order` has no `AddOrderItem`: the operation's `member.exists` claim is contradicted, and its parameters claim is blocked by it.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.parameters", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-add-order-item", "outcome": "contradicted" },
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-add-order-item" },
    { "rule": "claim/operation.parameters", "count": 0 }
  ]
}
```

## constructor

A modelled constructor's parameters against the constructor chosen for it. `OrderItem(int productId, int units)` is chosen over the protected parameterless constructor, and its parameters are the model's.

```json case
{ "phase": 1, "kinds": ["operation.parameters"], "traces": ["ESC-DOM-61"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "children": [
              { "id": "order-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "int" } },
              { "id": "order-item-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } },
              { "id": "order-item-ctor", "type": "Class Constructor", "name": "OrderItem",
                "children": [
                  { "id": "order-item-ctor-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "int" } },
                  { "id": "order-item-ctor-units", "type": "Parameter", "name": "units", "typeRef": { "type": "int" } }
                ] }
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

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class OrderItem
{
    protected OrderItem() { }

    public OrderItem(int productId, int units)
    {
        ProductId = productId;
        Units = units;
    }

    public int ProductId { get; private set; }

    public int Units { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-item-ctor", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs:7" }
  ]
}
```

## constructor-names

The constructor takes `(int units, int productId)`. The types match position by position, so it is still the constructor chosen, but the names don't: contradicted.

```json case
{ "phase": 1, "base": "constructor", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-61"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class OrderItem
{
    protected OrderItem() { }

    public OrderItem(int units, int productId)
    {
        ProductId = productId;
        Units = units;
    }

    public int ProductId { get; private set; }

    public int Units { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-item-ctor", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-item-ctor" }
  ]
}
```

## repository-operation

A Repository Operation's parameters against the interface method's: `GetAsync(int orderId)`.

```json case
{ "phase": 1, "kinds": ["operation.parameters"], "traces": ["ESC-DOM-73"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
          { "id": "order-repository", "type": "Repository", "name": "OrderRepository", "typeRef": { "element": "order" },
            "children": [
              { "id": "order-repository-get", "type": "Operation", "name": "GetAsync", "typeRef": { "element": "order" },
                "children": [
                  { "id": "order-repository-get-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] }
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

    Task<Order> GetAsync(int orderId);
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-repository-get", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:7" }
  ]
}
```

## repository-operation-signature

`GetAsync` takes a `Guid`, not the modelled `int`: the Repository Operation's signature is contradicted.

```json case
{ "phase": 1, "base": "repository-operation", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-73"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public interface IOrderRepository
{
    Order Add(Order order);

    Task<Order> GetAsync(Guid orderId);
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-repository-get", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-repository-get" }
  ]
}
```

## services-endpoint

ClientA models each controller as a Service named as the class is, with one Operation per action; its house pack says a Service is the class of its name. The action `GetPrice(string expDate, string product)` has the modelled Parameters. A controller takes its dependencies through its constructor, so the action's parameters are all bound from the request. A minimal API handler's injected parameters (`[AsParameters]` holders, services, a `CancellationToken`) would be compared too: nothing in the language profile or conventions leaves them out.

```json case
{ "phase": 2, "kinds": ["operation.parameters"], "traces": ["ESC-SVC-26"], "requirements": ["R2", "R41", "R53"], "packs": ["intent.core", "intent.services", "house.acme"], "language": "csharp" }
```

```json pack
{
  "id": "house.acme",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["Service/**/Controllers/**"], "note": "Acme: each controller is a Service named as the class is" }
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
        "id": "services", "name": "Acme.API",
        "elements": [
          { "id": "prices-controller", "type": "Service", "name": "PricesController",
            "children": [
              { "id": "prices-controller-get-price", "type": "Operation", "name": "GetPrice",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "Price/Get/{expDate}/{product}" } },
                "children": [
                  { "id": "prices-controller-get-price-exp-date", "type": "Parameter", "name": "expDate", "typeRef": { "type": "string" } },
                  { "id": "prices-controller-get-price-product", "type": "Parameter", "name": "product", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "Service/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file Service/AcmeService/Acme.API/Controllers/Pricing/PricesController.cs
using Microsoft.AspNetCore.Mvc;

namespace Acme.API.Controllers.Pricing
{
    [ApiController]
    [Route("v1/[controller]")]
    public class PricesController : ControllerBase
    {
        private readonly PricesProcess _pricesProcess;

        public PricesController(PricesProcess pricesProcess)
        {
            _pricesProcess = pricesProcess;
        }

        [HttpGet("Price/Get/{expDate}/{product}")]
        public IActionResult GetPrice(string expDate, string product)
        {
            var data = _pricesProcess.GetPrice(expDate, product);
            return Ok(data);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "prices-controller-get-price", "outcome": "verified" }
  ]
}
```

## action-from-services-parameter

An action may take a dependency the container supplies rather than its constructor: `[FromServices] IGetAccountUseCase useCase`. The request never sends it, so it is no part of the operation's signature, which the model states as `accountId` alone.

```json case
{ "phase": 2, "kinds": ["operation.parameters"], "requirements": ["R2", "R41", "R53"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Bank.Api",
        "elements": [
          { "id": "accounts-controller", "type": "Service", "name": "AccountsController",
            "stereotypes": { "Http Service Settings": { "Route": "api/accounts" } },
            "children": [
              { "id": "accounts-get", "type": "Operation", "name": "Get",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{accountId}" } },
                "children": [
                  { "id": "accounts-get-account-id", "type": "Parameter", "name": "accountId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Bank", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Bank.Api/Controllers/AccountsController.cs
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

public interface IGetAccountUseCase
{
    Task Execute(Guid accountId);
}

[ApiController]
[Route("api/accounts")]
public sealed class AccountsController : ControllerBase
{
    [HttpGet("{accountId}")]
    public async Task<IActionResult> Get([FromServices] IGetAccountUseCase useCase, [FromRoute] Guid accountId)
    {
        await useCase.Execute(accountId);
        return Ok();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "accounts-get", "outcome": "verified" }
  ]
}
```

## services-endpoint-parameter-missing

The model adds a `month` Parameter that the action doesn't take: contradicted, `services.parameter.missing` in eShopCustom's terms.

```json case
{ "phase": 2, "base": "services-endpoint", "kinds": ["operation.parameters"], "traces": ["ESC-SVC-26"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "house.acme"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.API",
        "elements": [
          { "id": "prices-controller", "type": "Service", "name": "PricesController",
            "children": [
              { "id": "prices-controller-get-price", "type": "Operation", "name": "GetPrice",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "Price/Get/{expDate}/{product}" } },
                "children": [
                  { "id": "prices-controller-get-price-exp-date", "type": "Parameter", "name": "expDate", "typeRef": { "type": "string" } },
                  { "id": "prices-controller-get-price-product", "type": "Parameter", "name": "product", "typeRef": { "type": "string" } },
                  { "id": "prices-controller-get-price-month", "type": "Parameter", "name": "month", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "Service/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "prices-controller-get-price", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "prices-controller-get-price" }
  ]
}
```

## grpc-rpc

A gRPC Service Operation's one Parameter is the rpc's request message (`services.grpc.operation.rpc`). The service's override also takes gRPC's `ServerCallContext`, which the C# language profile counts as injected, as it does a `CancellationToken`, so the override's parameters are the modelled `request`.

```json case
{ "phase": 2, "kinds": ["operation.parameters"], "traces": ["ESC-SVC-79"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "get-basket-request", "type": "DTO", "name": "GetBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "update-basket-request", "type": "DTO", "name": "UpdateBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "customer-basket-response", "type": "DTO", "name": "CustomerBasketResponse", "stereotypes": { "Proto Message": {} } },
          { "id": "basket-service", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Proto/basket.proto", "Proto Service": "Basket" } },
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasket", "typeRef": { "element": "customer-basket-response" },
                "children": [
                  { "id": "basket-service-get-basket-request", "type": "Parameter", "name": "request", "typeRef": { "element": "get-basket-request" } }
                ] }
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

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data is null ? new() : MapToCustomerBasketResponse(data);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "basket-service-get-basket", "outcome": "verified" }
  ]
}
```

## java-proxy-parameters

A Java spike storefront Proxy Operation's Parameters are its method's, by position, name and type. The house pack gives the spike's own Front End designer vocabulary; TypeScript names the method in camel case and writes the model's `guid` as `string`.

```json case
{ "phase": 4, "kinds": ["operation.parameters"], "traces": ["JAV-FE-10"], "requirements": ["R2", "R28", "R41"], "packs": ["intent.core", "house.storefront"], "language": "typescript" }
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
        { "kind": "type.exists", "for": { "element": "Proxy" }, "value": "self", "bindings": { "Name": "name" }, "own": ["Name"], "source": "the Proxy" },
        { "kind": "member.exists", "for": { "element": "Proxy Operation" }, "value": "self", "dependsOn": "parent:type.exists", "source": "the Proxy Operation" },
        { "kind": "operation.parameters", "for": { "element": "Proxy Operation" }, "dependsOn": "member.exists", "source": "the Proxy Operation's parameters" }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Proxy": { "name": "{Name|camel}", "typeKinds": ["object"], "states": ["method"], "note": "The storefront: a Proxy is the object of its methods" }
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
          { "id": "api", "type": "Proxy", "name": "Api",
            "children": [
              { "id": "api-add-address", "type": "Proxy Operation", "name": "AddAddress", "typeRef": { "type": "string" },
                "children": [
                  { "id": "api-add-address-customer-id", "type": "Parameter", "name": "customerId", "typeRef": { "type": "guid" } },
                  { "id": "api-add-address-line", "type": "Parameter", "name": "line", "typeRef": { "type": "string" } }
                ] }
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

```typescript file src/api/client.ts
async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
    const res = await fetch(path, { method, body: JSON.stringify(body) });
    return (await res.json()) as T;
}

export const api = {
    addAddress(customerId: string, line: string): Promise<string> {
        return request<string>('POST', `/api/customers/${encodeURIComponent(customerId)}/addresses`, { line });
    },
};
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "api-add-address", "outcome": "verified" }
  ]
}
```

## java-proxy-parameters-reordered

The method takes `line` before `customerId`: the Parameters are compared by position, so the order the model gives isn't the code's.

```json case
{ "phase": 4, "base": "java-proxy-parameters", "kinds": ["operation.parameters"], "traces": ["JAV-FE-10"], "requirements": ["R1", "R11"], "packs": ["intent.core", "house.storefront"], "language": "typescript" }
```

```typescript file src/api/client.ts
async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
    const res = await fetch(path, { method, body: JSON.stringify(body) });
    return (await res.json()) as T;
}

export const api = {
    addAddress(line: string, customerId: string): Promise<string> {
        return request<string>('POST', `/api/customers/${encodeURIComponent(customerId)}/addresses`, { line });
    },
};
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "api-add-address", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "api-add-address" }
  ]
}
```

## grpc-rpc-other-request

The override takes `UpdateBasketRequest`, another rpc's request message, so it isn't the modelled rpc's.

```json case
{ "phase": 2, "base": "grpc-rpc", "kinds": ["operation.parameters"], "traces": ["ESC-SVC-79"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    public override async Task<CustomerBasketResponse> GetBasket(UpdateBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data is null ? new() : MapToCustomerBasketResponse(data);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "basket-service-get-basket", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "basket-service-get-basket" }
  ]
}
```
