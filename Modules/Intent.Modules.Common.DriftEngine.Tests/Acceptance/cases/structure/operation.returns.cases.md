# operation.returns

A modelled operation returns the type its type reference states, and an operation with no type reference returns nothing. The code side is the method's return type, unwrapped of the language profile's async wrappers (in C#, `Task<T>` and `ValueTask<T>` are `T`, and a bare `Task` or `ValueTask` means nothing is returned), then of the result wrappers a framework pack's conventions list (rules.md, Language profiles). The `type` comparer then maps the model's type through the profile, as for `member.type`; nullability isn't part of it. It is a `stated: always` kind that depends on the method: when the method isn't there, the claim is blocked. An unwrapped return is shown by `expected` alone, since the code's text is the wrapper's.

## verified

`Order.GetTotal` returns `decimal`, as the model's operation does.

```json case
{ "phase": 1, "kinds": ["operation.returns"], "traces": ["ESC-DOM-64"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-get-total", "type": "Operation", "name": "GetTotal", "typeRef": { "type": "decimal" } }
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

    public decimal GetTotal() => _orderItems.Sum(o => o.Units * o.UnitPrice);
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "order-get-total", "outcome": "verified", "expected": "decimal", "actual": "decimal", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## contradicted

`GetTotal` returns `double`: contradicted, and the finding shows both types.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.returns"], "traces": ["ESC-DOM-64"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public double GetTotal() => (double)_orderItems.Sum(o => o.Units * o.UnitPrice);
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "order-get-total", "outcome": "contradicted", "expected": "decimal", "actual": "double", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "order-get-total", "messageContains": "code `double`" }
  ]
}
```

## blocked

`Order` has no `GetTotal`. The operation's `member.exists` claim is contradicted, and its return claim waits on it: can't verify, `blocked`.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.returns", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
    { "kind": "member.exists", "element": "order-get-total", "outcome": "contradicted" },
    { "kind": "operation.returns", "element": "order-get-total", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-get-total" },
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## house-entry-without-depends-on

A house's own designer, `Ledger`: a Book is a class and its Ledger Operations are its methods. The house pack's `operation.returns` entry names no `dependsOn`. The engine's rule reads the method the operation's `member.exists` claim located, so the claim depends on that claim, as a stock entry's does, and `Balance` returning `decimal` verifies it.

```json case
{ "phase": 2, "kinds": ["operation.returns"], "requirements": ["R2", "R8"], "packs": ["intent.core", "house.ledger"], "language": "csharp" }
```

```json pack
{
  "id": "house.ledger",
  "version": "1.0.0",
  "languages": ["csharp"],
  "designers": ["Ledger"],
  "vocabulary": {
    "Ledger": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Book" }, "value": "self", "source": "the Book" },
        { "kind": "member.exists", "for": { "element": "Ledger Operation" }, "value": "self", "dependsOn": "parent:type.exists", "source": "the Ledger Operation" },
        { "kind": "operation.returns", "for": { "element": "Ledger Operation" }, "value": "type", "source": "the Ledger Operation's type" }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Ledger": {
      "packages": [{
        "id": "ledger", "name": "Accounts",
        "elements": [
          { "id": "general-ledger", "type": "Book", "name": "GeneralLedger",
            "children": [
              { "id": "general-ledger-balance", "type": "Ledger Operation", "name": "Balance", "typeRef": { "type": "decimal" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Accounts", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Accounts/GeneralLedger.cs
namespace Accounts;

public class GeneralLedger
{
    public decimal Balance() => 0m;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "general-ledger", "outcome": "verified" },
    { "kind": "member.exists", "element": "general-ledger-balance", "outcome": "verified" },
    { "kind": "operation.returns", "element": "general-ledger-balance", "outcome": "verified", "expected": "decimal", "actual": "decimal", "at": "src/Accounts/GeneralLedger.cs:5" }
  ]
}
```

## house-entry-without-existence-claim

The house pack claims `operation.returns` on a Ledger Operation but no `member.exists`, so nothing locates the method its rule would read: can't verify, `blocked`, where the engine once failed.

```json case
{ "phase": 2, "base": "house-entry-without-depends-on", "kinds": ["operation.returns"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "house.ledger"], "language": "csharp" }
```

```json pack
{
  "id": "house.ledger",
  "version": "1.0.0",
  "languages": ["csharp"],
  "designers": ["Ledger"],
  "vocabulary": {
    "Ledger": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Book" }, "value": "self", "source": "the Book" },
        { "kind": "operation.returns", "for": { "element": "Ledger Operation" }, "value": "type", "source": "the Ledger Operation's type" }
      ]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "general-ledger-balance", "outcome": "cantVerify", "reason": "blocked" }
  ]
}
```

## async-unwrapped

A Repository's operations are the methods of its interface. The C# profile unwraps `Task<Order>` to `Order` and `ValueTask<int>` to `int`, and reads a bare `Task` as returning nothing, which is what `UpdateAsync`, with no type reference, says.

```json case
{ "phase": 1, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
                ] },
              { "id": "order-repository-update", "type": "Operation", "name": "UpdateAsync",
                "children": [
                  { "id": "order-repository-update-order", "type": "Parameter", "name": "order", "typeRef": { "element": "order" } }
                ] },
              { "id": "order-repository-count-pending", "type": "Operation", "name": "CountPendingAsync", "typeRef": { "type": "int" } }
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
    Task<Order> GetAsync(int orderId);

    Task UpdateAsync(Order order);

    ValueTask<int> CountPendingAsync();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "order-repository-get", "outcome": "verified", "expected": "Order", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:5" },
    { "kind": "operation.returns", "element": "order-repository-update", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:7" },
    { "kind": "operation.returns", "element": "order-repository-count-pending", "outcome": "verified", "expected": "int", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## async-returns-a-value

`UpdateAsync` now returns `Task<Order>`, which unwraps to `Order`, while the model's operation returns nothing: contradicted.

```json case
{ "phase": 1, "base": "async-unwrapped", "kinds": ["operation.returns"], "requirements": ["R1", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public interface IOrderRepository
{
    Task<Order> GetAsync(int orderId);

    Task<Order> UpdateAsync(Order order);

    ValueTask<int> CountPendingAsync();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "order-repository-update", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "order-repository-update" }
  ]
}
```

## services-endpoint-results

eShopCustom models each minimal API router as a Service, `OrdersApi`, with one Operation per handler method; its house pack says a Service is the class of its name. A handler's return type is read through `Task`, then through ASP.NET Core's typed results, which the ASP.NET Core pack's conventions list as result wrappers (`ActionResult<T>`, `Ok<T>`, `Results<Ok<T>, …>`): `Results<Ok<Order>, NotFound>` succeeds with `Order`, and `Ok<IEnumerable<OrderSummary>>` with a collection of `OrderSummary`. Error results aren't part of the Operation's type.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "traces": ["ESC-SVC-23"], "requirements": ["R2", "R41", "R42", "R53"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["src/**"], "note": "eShopCustom: a Service with no interface is described by its class" }
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
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "Order",
            "children": [
              { "id": "order-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "orders-api-get-order", "type": "Operation", "name": "GetOrderAsync", "typeRef": { "element": "order-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{orderId}" } },
                "children": [
                  { "id": "orders-api-get-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] },
              { "id": "orders-api-get-orders", "type": "Operation", "name": "GetOrdersByUserAsync", "typeRef": { "element": "order-summary", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "" } } }
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

```csharp file src/Ordering.API/Application/Queries/OrderViewModel.cs
namespace eShop.Ordering.API.Application.Queries;

public record Order(int OrderNumber);

public record OrderSummary(int OrderNumber);
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace eShop.Ordering.API.Apis;

public static class OrdersApi
{
    public static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        try
        {
            var order = await services.Queries.GetOrderAsync(orderId);
            return TypedResults.Ok(order);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync([AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity()!;
        var orders = await services.Queries.GetOrdersFromUserAsync(userId);
        return TypedResults.Ok(orders);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "orders-api-get-order", "outcome": "verified", "expected": "Order", "at": "src/Ordering.API/Apis/OrdersApi.cs:5" },
    { "kind": "operation.returns", "element": "orders-api-get-orders", "outcome": "verified", "expected": "OrderSummary[]", "at": "src/Ordering.API/Apis/OrdersApi.cs:18" }
  ]
}
```

## services-endpoint-results-contradicted

`GetOrderAsync` now succeeds with `Ok<OrderSummary>`, not the modelled `Order`: contradicted, `services.operation.return` in eShopCustom's terms.

```json case
{ "phase": 4, "base": "services-endpoint-results", "kinds": ["operation.returns"], "traces": ["ESC-SVC-23"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace eShop.Ordering.API.Apis;

public static class OrdersApi
{
    public static async Task<Results<Ok<OrderSummary>, NotFound>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        try
        {
            var order = await services.Queries.GetOrderSummaryAsync(orderId);
            return TypedResults.Ok(order);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync([AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity()!;
        var orders = await services.Queries.GetOrdersFromUserAsync(userId);
        return TypedResults.Ok(orders);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "orders-api-get-order", "outcome": "contradicted", "expected": "Order", "at": "src/Ordering.API/Apis/OrdersApi.cs:5" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "orders-api-get-order" }
  ]
}
```

## action-result-body

A controller action returning `IActionResult`, or `Task<IActionResult>`, doesn't state what it succeeds with, so the ASP.NET Core stack reads each return for the success result it builds, and what that carries is what the action returns: `Ok(data)` is `data`'s type, here what `IProductService.GetAllAsync` gives once awaited, `ProductDTO[]`. `product is null ? NotFound() : Ok(product)` is an error result, which the model doesn't state, or a `ProductDTO`; `NoContent()` has no body, so `Delete` returns nothing.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Api",
        "elements": [
          { "id": "product-dto", "type": "DTO", "name": "ProductDTO",
            "children": [
              { "id": "product-dto-code", "type": "DTO-Field", "name": "Code", "typeRef": { "type": "string" } }
            ] },
          { "id": "product-catalog-service", "type": "Service", "name": "ProductCatalogService",
            "stereotypes": { "Http Service Settings": { "Route": "api/product-catalog" } },
            "children": [
              { "id": "product-catalog-get-all", "type": "Operation", "name": "GetAll", "typeRef": { "element": "product-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "" } } },
              { "id": "product-catalog-get", "type": "Operation", "name": "Get", "typeRef": { "element": "product-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" } },
                "children": [
                  { "id": "product-catalog-get-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] },
              { "id": "product-catalog-delete", "type": "Operation", "name": "Delete",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "{id}" } },
                "children": [
                  { "id": "product-catalog-delete-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Api/Products/ProductDTO.cs
namespace Acme.Api.Products;

public class ProductDTO
{
    public string Code { get; set; } = "";
}

public interface IProductService
{
    Task<ProductDTO[]> GetAllAsync();

    Task<ProductDTO?> FindAsync(int id);

    Task DeleteAsync(int id);
}
```

```csharp file src/Acme.Api/Controllers/ProductCatalogController.cs
using Acme.Api.Products;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

[ApiController]
[Route("api/product-catalog")]
public class ProductCatalogController : ControllerBase
{
    private readonly IProductService _products;

    public ProductCatalogController(IProductService products)
    {
        _products = products;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _products.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _products.FindAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _products.DeleteAsync(id);
        return NoContent();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "product-catalog-get-all", "outcome": "verified", "expected": "ProductDTO[]", "actual": "Task<IActionResult> with ProductDTO[]" },
    { "kind": "operation.returns", "element": "product-catalog-get", "outcome": "verified", "expected": "ProductDTO", "actual": "Task<IActionResult> with ProductDTO?" },
    { "kind": "operation.returns", "element": "product-catalog-delete", "outcome": "verified", "actual": "Task<IActionResult> with no body" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## action-result-from-output-port

A clean-architecture controller is its use case's output port: the action runs the use case, which calls the port back, and returns the result the port set, `return this._viewModel!`. The controller's own members that assign the field say what it returns, as returns through a local do: `IOutputPort.Ok(Account account)` sets `Ok(new AccountModel(account))`, so `Get` returns an `AccountModel`, with a 200; `NotFound()` is an error, which the model doesn't state. `Close`'s port sets `Accepted(...)` where the model says 200.

```json case
{ "phase": 4, "kinds": ["operation.returns", "operation.status"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Bank.Api",
        "elements": [
          { "id": "account-model", "type": "DTO", "name": "AccountModel",
            "children": [
              { "id": "account-model-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] },
          { "id": "accounts-controller", "type": "Service", "name": "AccountsController",
            "stereotypes": { "Http Service Settings": { "Route": "api/accounts" } },
            "children": [
              { "id": "accounts-get", "type": "Operation", "name": "Get", "typeRef": { "element": "account-model" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{accountId}", "Success Response Code": "200 (Ok)" } },
                "children": [
                  { "id": "accounts-get-account-id", "type": "Parameter", "name": "accountId", "typeRef": { "type": "guid" } }
                ] }
            ] },
          { "id": "close-controller", "type": "Service", "name": "CloseController",
            "stereotypes": { "Http Service Settings": { "Route": "api/accounts" } },
            "children": [
              { "id": "close-close", "type": "Operation", "name": "Close", "typeRef": { "element": "account-model" },
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "{accountId}", "Success Response Code": "200 (Ok)" } },
                "children": [
                  { "id": "close-close-account-id", "type": "Parameter", "name": "accountId", "typeRef": { "type": "guid" } }
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

```csharp file src/Bank.Api/Accounts.cs
namespace Bank.Api;

public class Account
{
    public Guid Id { get; set; }
}

public class AccountModel
{
    public AccountModel(Account account) => Id = account.Id;

    public Guid Id { get; }
}

public interface IOutputPort
{
    void Ok(Account account);

    void NotFound();
}

public interface IGetAccountUseCase
{
    void SetOutputPort(IOutputPort outputPort);

    Task Execute(Guid accountId);
}
```

```csharp file src/Bank.Api/Controllers/AccountsController.cs
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class AccountsController : ControllerBase, IOutputPort
{
    private readonly IGetAccountUseCase _useCase;
    private IActionResult? _viewModel;

    public AccountsController(IGetAccountUseCase useCase) => _useCase = useCase;

    void IOutputPort.Ok(Account account) => this._viewModel = this.Ok(new AccountModel(account));

    void IOutputPort.NotFound() => this._viewModel = this.NotFound();

    [HttpGet("{accountId}")]
    public async Task<IActionResult> Get(Guid accountId)
    {
        this._useCase.SetOutputPort(this);
        await this._useCase.Execute(accountId);
        return this._viewModel!;
    }
}
```

```csharp file src/Bank.Api/Controllers/CloseController.cs
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class CloseController : ControllerBase, IOutputPort
{
    private readonly IGetAccountUseCase _useCase;
    private IActionResult? _viewModel;

    public CloseController(IGetAccountUseCase useCase) => _useCase = useCase;

    void IOutputPort.Ok(Account account) => this._viewModel = this.Accepted(new AccountModel(account));

    void IOutputPort.NotFound() => this._viewModel = this.NotFound();

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> Close(Guid accountId)
    {
        this._useCase.SetOutputPort(this);
        await this._useCase.Execute(accountId);
        return this._viewModel!;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "accounts-get", "outcome": "verified", "expected": "AccountModel", "actual": "Task<IActionResult> with AccountModel" },
    { "kind": "operation.status", "element": "accounts-get", "outcome": "verified", "actual": "200", "at": "src/Bank.Api/Controllers/AccountsController.cs:14" },
    { "kind": "operation.returns", "element": "close-close", "outcome": "verified", "expected": "AccountModel", "actual": "Task<IActionResult> with AccountModel" },
    { "kind": "operation.status", "element": "close-close", "outcome": "contradicted", "actual": "202", "at": "src/Bank.Api/Controllers/CloseController.cs:14" }
  ],
  "findings": [
    { "rule": "claim/operation.status", "severity": "error", "subject": "close-close" }
  ]
}
```

## action-result-body-contradicted

`Get` now succeeds with every product, `Ok(await _products.GetAllAsync())`, against the model's single `ProductDTO`; `Delete` succeeds with `Ok(id)`, an `int`, against a model that returns nothing. Both are contradicted.

```json case
{ "phase": 4, "base": "action-result-body", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Api/Controllers/ProductCatalogController.cs
using Acme.Api.Products;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

[ApiController]
[Route("api/product-catalog")]
public class ProductCatalogController : ControllerBase
{
    private readonly IProductService _products;

    public ProductCatalogController(IProductService products)
    {
        _products = products;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _products.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _products.GetAllAsync());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _products.DeleteAsync(id);
        return Ok(id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "product-catalog-get-all", "outcome": "verified", "expected": "ProductDTO[]" },
    { "kind": "operation.returns", "element": "product-catalog-get", "outcome": "contradicted", "expected": "ProductDTO", "actual": "Task<IActionResult> with ProductDTO[]" },
    { "kind": "operation.returns", "element": "product-catalog-delete", "outcome": "contradicted", "actual": "Task<IActionResult> with int" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "product-catalog-get" },
    { "rule": "claim/operation.returns", "severity": "error", "subject": "product-catalog-delete" }
  ]
}
```

## action-result-body-unknown

`GetAll` succeeds with what a library gives, `_cache.Values` of a `Dictionary<int, object>`, and `Get` returns a result an interface the code doesn't declare builds, so what either returns isn't known: can't verify, never a guess.

```json case
{ "phase": 4, "base": "action-result-body", "kinds": ["operation.returns"], "requirements": ["R2", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Api/Controllers/ProductCatalogController.cs
using Acme.Api.Products;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

[ApiController]
[Route("api/product-catalog")]
public class ProductCatalogController : ControllerBase
{
    private readonly Dictionary<int, object> _cache;
    private readonly IResultFactory _results;

    public ProductCatalogController(Dictionary<int, object> cache, IResultFactory results)
    {
        _cache = cache;
        _results = results;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_cache.Values);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return _results.Build(id);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return NoContent();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "product-catalog-get-all", "outcome": "cantVerify", "reason": "unresolved" },
    { "kind": "operation.returns", "element": "product-catalog-get", "outcome": "cantVerify", "reason": "unreadable" },
    { "kind": "operation.returns", "element": "product-catalog-delete", "outcome": "verified", "actual": "IActionResult with no body" }
  ]
}
```

## action-result-file

`GetTemplate` sends a file, `File(excelData, contentType, fileName)`, whose content is the `byte[]` `ITemplateService.Build` gives, so the action returns `byte[]`.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Api",
        "elements": [
          { "id": "bid-import-service", "type": "Service", "name": "BidImportService",
            "stereotypes": { "Http Service Settings": { "Route": "api/bid-import" } },
            "children": [
              { "id": "bid-import-get-template", "type": "Operation", "name": "GetTemplate", "typeRef": { "type": "binary" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "template" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Api/Controllers/BidImportController.cs
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

public interface ITemplateService
{
    byte[] Build();
}

[ApiController]
[Route("api/bid-import")]
public class BidImportController : ControllerBase
{
    private readonly ITemplateService _templates;

    public BidImportController(ITemplateService templates)
    {
        _templates = templates;
    }

    [HttpGet("template")]
    public IActionResult GetTemplate()
    {
        var excelData = _templates.Build();
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = "BidImportTemplate.xlsx";
        return File(excelData, contentType, fileName);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "bid-import-get-template", "outcome": "verified", "actual": "IActionResult with byte[]" }
  ]
}
```

## action-result-file-contradicted

The model says `GetTemplate` returns a collection of strings, which the file's `byte[]` isn't: contradicted, compared item by item as any array is, where the rule once failed.

```json case
{ "phase": 4, "base": "action-result-file", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Api",
        "elements": [
          { "id": "bid-import-service", "type": "Service", "name": "BidImportService",
            "stereotypes": { "Http Service Settings": { "Route": "api/bid-import" } },
            "children": [
              { "id": "bid-import-get-template", "type": "Operation", "name": "GetTemplate", "typeRef": { "type": "string", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "template" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "bid-import-get-template", "outcome": "contradicted", "actual": "IActionResult with byte[]" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "bid-import-get-template" }
  ]
}
```

## action-result-body-values

What an action's success carries is read as the code types it. A literal is its own type: `Ok("Cache Cleared")` a `string`, `Ok($"…")` too, `Ok(true)` a `bool`. A `Select` projects to what its lambda returns, so `notifications.Select(x => new NotificationDTO { … })` is a sequence of `NotificationDTO`. Of `BidTypes.Get`'s overloads, the one whose parameter the argument's enum fits, `Get(BidTypeGroup)`, is the one called. The model's `object` is any type, so an action the model says returns `object` is verified by its declared `IActionResult` alone: less precise than the code, but true.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Api",
        "elements": [
          { "id": "notification-dto", "type": "DTO", "name": "NotificationDTO",
            "children": [
              { "id": "notification-dto-text", "type": "DTO-Field", "name": "Text", "typeRef": { "type": "string" } }
            ] },
          { "id": "bid-type-dto", "type": "DTO", "name": "BidTypeDTO",
            "children": [
              { "id": "bid-type-dto-name", "type": "DTO-Field", "name": "Name", "typeRef": { "type": "string" } }
            ] },
          { "id": "feature-service", "type": "Service", "name": "FeatureService",
            "stereotypes": { "Http Service Settings": { "Route": "api/feature" } },
            "children": [
              { "id": "feature-clear-cache", "type": "Operation", "name": "ClearCache", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "cache/clear" } } },
              { "id": "feature-describe", "type": "Operation", "name": "Describe", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "describe/{id}" } },
                "children": [
                  { "id": "feature-describe-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] },
              { "id": "feature-is-enabled", "type": "Operation", "name": "IsEnabled", "typeRef": { "type": "bool" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "enabled" } } },
              { "id": "feature-get-notifications", "type": "Operation", "name": "GetNotifications", "typeRef": { "element": "notification-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "notifications" } } },
              { "id": "feature-get-bid-types", "type": "Operation", "name": "GetBidTypes", "typeRef": { "element": "bid-type-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "bid-types" } } },
              { "id": "feature-get-open-bids-count", "type": "Operation", "name": "GetOpenBidsCount", "typeRef": { "type": "object" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "open-bids/count" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Api/Business/Features.cs
namespace Acme.Api.Business;

public enum BidTypeGroup
{
    MarketPlace,
    Tender
}

public class NotificationDTO
{
    public string Text { get; set; } = "";
}

public class BidTypeDTO
{
    public string Name { get; set; } = "";
}

public class Notification
{
    public string Message { get; set; } = "";
}

public class BidTypes
{
    public BidTypeDTO Get(int id) => new BidTypeDTO();

    public List<BidTypeDTO> Get(BidTypeGroup group) => new List<BidTypeDTO>();

    public List<BidTypeDTO> Get() => new List<BidTypeDTO>();
}

public class Bids
{
    public dynamic GetOpenBidsCount() => new { Count = 0 };
}
```

```csharp file src/Acme.Api/Controllers/FeatureController.cs
using Acme.Api.Business;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

[ApiController]
[Route("api/feature")]
public class FeatureController : ControllerBase
{
    private readonly List<Notification> _notifications = new List<Notification>();
    private readonly BidTypes _bidTypes = new BidTypes();
    private readonly Bids _bids = new Bids();

    [HttpPost("cache/clear")]
    public IActionResult ClearCache()
    {
        return Ok("Cache Cleared");
    }

    [HttpGet("describe/{id}")]
    public IActionResult Describe(int id)
    {
        return Ok($"Feature {id}");
    }

    [HttpGet("enabled")]
    public IActionResult IsEnabled()
    {
        return Ok(true);
    }

    [HttpGet("notifications")]
    public IActionResult GetNotifications()
    {
        var result = _notifications.Select(x => new NotificationDTO { Text = x.Message });
        return Ok(result);
    }

    [HttpGet("bid-types")]
    public IActionResult GetBidTypes()
    {
        return Ok(_bidTypes.Get(BidTypeGroup.MarketPlace));
    }

    [HttpGet("open-bids/count")]
    public IActionResult GetOpenBidsCount()
    {
        return Ok(_bids.GetOpenBidsCount());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "feature-clear-cache", "outcome": "verified", "actual": "IActionResult with string" },
    { "kind": "operation.returns", "element": "feature-describe", "outcome": "verified", "actual": "IActionResult with string" },
    { "kind": "operation.returns", "element": "feature-is-enabled", "outcome": "verified", "actual": "IActionResult with bool" },
    { "kind": "operation.returns", "element": "feature-get-notifications", "outcome": "verified", "actual": "IActionResult with IEnumerable<NotificationDTO>" },
    { "kind": "operation.returns", "element": "feature-get-bid-types", "outcome": "verified", "actual": "IActionResult with List<BidTypeDTO>" },
    { "kind": "operation.returns", "element": "feature-get-open-bids-count", "outcome": "verified", "actual": "IActionResult" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## action-result-body-values-contradicted

`ClearCache` now succeeds with `Ok(1)`, an `int`, and `GetBidTypes` with `Get(42)`, the overload that returns one `BidTypeDTO`, against the model's string and collection: both contradicted.

```json case
{ "phase": 4, "base": "action-result-body-values", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Api/Controllers/FeatureController.cs
using Acme.Api.Business;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

[ApiController]
[Route("api/feature")]
public class FeatureController : ControllerBase
{
    private readonly List<Notification> _notifications = new List<Notification>();
    private readonly BidTypes _bidTypes = new BidTypes();
    private readonly Bids _bids = new Bids();

    [HttpPost("cache/clear")]
    public IActionResult ClearCache()
    {
        return Ok(1);
    }

    [HttpGet("describe/{id}")]
    public IActionResult Describe(int id)
    {
        return Ok($"Feature {id}");
    }

    [HttpGet("enabled")]
    public IActionResult IsEnabled()
    {
        return Ok(true);
    }

    [HttpGet("notifications")]
    public IActionResult GetNotifications()
    {
        var result = _notifications.Select(x => new NotificationDTO { Text = x.Message });
        return Ok(result);
    }

    [HttpGet("bid-types")]
    public IActionResult GetBidTypes()
    {
        return Ok(_bidTypes.Get(42));
    }

    [HttpGet("open-bids/count")]
    public IActionResult GetOpenBidsCount()
    {
        return Ok(_bids.GetOpenBidsCount());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "feature-clear-cache", "outcome": "contradicted", "actual": "IActionResult with int" },
    { "kind": "operation.returns", "element": "feature-get-bid-types", "outcome": "contradicted", "actual": "IActionResult with BidTypeDTO" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "feature-clear-cache" },
    { "rule": "claim/operation.returns", "severity": "error", "subject": "feature-get-bid-types" }
  ]
}
```

## action-result-body-shapes

More of what an action's success carries is read as the code types it:

- An anonymous object is the modelled type of its shape, its members by name with case aside: `new { Id = id }` is `IdResult`'s one `Id`. The model's `object` is any type, so `AddRole`'s declared `IActionResult` verifies it before its body is read.
- A `GroupBy(...).Select(x => new { x.Key.Region }).ToList()` is a list of that anonymous shape, `RegionDTO`'s.
- A deserializer's or a mapper's type argument is what it gives: `JsonConvert.DeserializeObject<AnalysisDTO>(json)`, `Mapper.Map<BidDTO>(bid)`. The fallback `Ok()` beside a body returns nothing of its type, not another type.
- A `DateTime`'s `ToLocalTime()` is a `DateTime`.
- A query over what the code's own method gives keeps its element: `GetCacheData<IEnumerable<CategoryDTO>>(...).ToList()`, then `Where(...).FirstOrDefault()`, is one `CategoryDTO`.
- A file sent as bytes is the model's `Stream` as much as its `binary`.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.API",
        "elements": [
          { "id": "id-result", "type": "DTO", "name": "IdResult",
            "children": [ { "id": "id-result-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "int" } } ] },
          { "id": "region-dto", "type": "DTO", "name": "RegionDTO",
            "children": [ { "id": "region-dto-region", "type": "DTO-Field", "name": "region", "typeRef": { "type": "string" } } ] },
          { "id": "analysis-dto", "type": "DTO", "name": "AnalysisDTO",
            "children": [ { "id": "analysis-dto-summary", "type": "DTO-Field", "name": "Summary", "typeRef": { "type": "string" } } ] },
          { "id": "bid-dto", "type": "DTO", "name": "BidDTO",
            "children": [ { "id": "bid-dto-number", "type": "DTO-Field", "name": "Number", "typeRef": { "type": "int" } } ] },
          { "id": "category-dto", "type": "DTO", "name": "CategoryDTO",
            "children": [ { "id": "category-dto-link", "type": "DTO-Field", "name": "link", "typeRef": { "type": "string" } } ] },
          { "id": "shapes-service", "type": "Service", "name": "ShapesService",
            "stereotypes": { "Http Service Settings": { "Route": "api/shapes" } },
            "children": [
              { "id": "shapes-add-member", "type": "Operation", "name": "AddMember", "typeRef": { "element": "id-result" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "member" } } },
              { "id": "shapes-add-role", "type": "Operation", "name": "AddRole", "typeRef": { "type": "object" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "role" } } },
              { "id": "shapes-get-regions", "type": "Operation", "name": "GetRegions", "typeRef": { "element": "region-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "regions" } } },
              { "id": "shapes-analyze", "type": "Operation", "name": "Analyze", "typeRef": { "element": "analysis-dto" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "analyze" } } },
              { "id": "shapes-create-bid", "type": "Operation", "name": "CreateBid", "typeRef": { "element": "bid-dto" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "bid" } } },
              { "id": "shapes-get-expiry", "type": "Operation", "name": "GetExpiry", "typeRef": { "type": "datetime" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "expiry" } } },
              { "id": "shapes-get-category", "type": "Operation", "name": "GetCategory", "typeRef": { "element": "category-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "category/{material}" } },
                "children": [ { "id": "shapes-get-category-material", "type": "Parameter", "name": "material", "typeRef": { "type": "string" } } ] },
              { "id": "shapes-get-template", "type": "Operation", "name": "GetTemplate", "typeRef": { "element": "stream" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "template" } } }
            ] }
        ]
      }, {
        "id": "types", "name": "Types",
        "elements": [ { "id": "stream", "type": "Type-Definition", "name": "Stream" } ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.API/Business/Business.cs
namespace Acme.API.Business;

public class Bid
{
    public int Number { get; set; }

    public string region { get; set; } = "";
}

public class BidDTO
{
    public int Number { get; set; }
}

public class AnalysisDTO
{
    public string Summary { get; set; } = "";
}

public class CategoryDTO
{
    public string link { get; set; } = "";
}

public interface ICache
{
    T GetCacheData<T>(string key, int seconds, Func<T> load);
}

public class Bids
{
    public List<Bid> GetBidSummary() => new List<Bid>();

    public DateTime GetDefaultBidExpiryDate() => DateTime.UtcNow;

    public int CreateMember() => 1;

    public IEnumerable<CategoryDTO> GetCategories() => new List<CategoryDTO>();

    public byte[] BuildTemplate() => new byte[0];
}
```

```csharp file src/Acme.API/Controllers/ShapesController.cs
using AutoMapper;
using Acme.API.Business;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Acme.API.Controllers;

[ApiController]
[Route("api/shapes")]
public class ShapesController : ControllerBase
{
    private readonly Bids _bids = new Bids();
    private readonly ICache _cache;
    private readonly IMapper _mapper;

    public ShapesController(ICache cache, IMapper mapper)
    {
        _cache = cache;
        _mapper = mapper;
    }

    [HttpPost("member")]
    public IActionResult AddMember()
    {
        var id = _bids.CreateMember();
        return Ok(new { Id = id });
    }

    [HttpPost("role")]
    public IActionResult AddRole(bool success)
    {
        return Ok(new { Success = success, Message = $"Role assigned {success}" });
    }

    [HttpGet("regions")]
    public IActionResult GetRegions()
    {
        var regions = _bids.GetBidSummary().GroupBy(x => new { x.region }).Select(x => new { x.Key.region }).ToList();
        return Ok(regions);
    }

    [HttpPost("analyze")]
    public IActionResult Analyze(string response)
    {
        try
        {
            var jsonResponse = JsonConvert.DeserializeObject<AnalysisDTO>(response);
            return Ok(jsonResponse);
        }
        catch
        {
            return Ok();
        }
    }

    [HttpPost("bid")]
    public IActionResult CreateBid(Bid bid)
    {
        return Ok(_mapper.Map<BidDTO>(bid));
    }

    [HttpGet("expiry")]
    public IActionResult GetExpiry()
    {
        return Ok(_bids.GetDefaultBidExpiryDate().ToLocalTime());
    }

    [HttpGet("category/{material}")]
    public IActionResult GetCategory(string material)
    {
        var all = _cache.GetCacheData<IEnumerable<CategoryDTO>>("categories", 60, _bids.GetCategories).ToList();
        var category = all.Where(x => x.link == material).FirstOrDefault();
        return Ok(category);
    }

    [HttpGet("template")]
    public IActionResult GetTemplate()
    {
        var excelData = _bids.BuildTemplate();
        return File(excelData, "application/octet-stream", "Template.xlsx");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "shapes-add-member", "outcome": "verified", "actual": "IActionResult with new { Id }" },
    { "kind": "operation.returns", "element": "shapes-add-role", "outcome": "verified", "actual": "IActionResult" },
    { "kind": "operation.returns", "element": "shapes-get-regions", "outcome": "verified", "actual": "IActionResult with List<new { region }>" },
    { "kind": "operation.returns", "element": "shapes-analyze", "outcome": "verified", "actual": "IActionResult with AnalysisDTO" },
    { "kind": "operation.returns", "element": "shapes-create-bid", "outcome": "verified", "actual": "IActionResult with BidDTO" },
    { "kind": "operation.returns", "element": "shapes-get-expiry", "outcome": "verified", "actual": "IActionResult with DateTime" },
    { "kind": "operation.returns", "element": "shapes-get-category", "outcome": "verified", "actual": "IActionResult with CategoryDTO" },
    { "kind": "operation.returns", "element": "shapes-get-template", "outcome": "verified", "actual": "IActionResult with byte[]" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## action-result-body-shapes-contradicted

`AddMember` now succeeds with `new { Id = id, Name = "x" }`, a member `IdResult` lacks, and `CreateBid` maps to `AnalysisDTO`: both contradicted.

```json case
{ "phase": 4, "base": "action-result-body-shapes", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.API/Controllers/ShapesController.cs
using AutoMapper;
using Acme.API.Business;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Acme.API.Controllers;

[ApiController]
[Route("api/shapes")]
public class ShapesController : ControllerBase
{
    private readonly Bids _bids = new Bids();
    private readonly ICache _cache;
    private readonly IMapper _mapper;

    public ShapesController(ICache cache, IMapper mapper)
    {
        _cache = cache;
        _mapper = mapper;
    }

    [HttpPost("member")]
    public IActionResult AddMember()
    {
        var id = _bids.CreateMember();
        return Ok(new { Id = id, Name = "x" });
    }

    [HttpPost("role")]
    public IActionResult AddRole(bool success)
    {
        return Ok(new { Success = success, Message = $"Role assigned {success}" });
    }

    [HttpGet("regions")]
    public IActionResult GetRegions()
    {
        var regions = _bids.GetBidSummary().GroupBy(x => new { x.region }).Select(x => new { x.Key.region }).ToList();
        return Ok(regions);
    }

    [HttpPost("analyze")]
    public IActionResult Analyze(string response)
    {
        try
        {
            var jsonResponse = JsonConvert.DeserializeObject<AnalysisDTO>(response);
            return Ok(jsonResponse);
        }
        catch
        {
            return Ok();
        }
    }

    [HttpPost("bid")]
    public IActionResult CreateBid(Bid bid)
    {
        return Ok(_mapper.Map<AnalysisDTO>(bid));
    }

    [HttpGet("expiry")]
    public IActionResult GetExpiry()
    {
        return Ok(_bids.GetDefaultBidExpiryDate().ToLocalTime());
    }

    [HttpGet("category/{material}")]
    public IActionResult GetCategory(string material)
    {
        var all = _cache.GetCacheData<IEnumerable<CategoryDTO>>("categories", 60, _bids.GetCategories).ToList();
        var category = all.Where(x => x.link == material).FirstOrDefault();
        return Ok(category);
    }

    [HttpGet("template")]
    public IActionResult GetTemplate()
    {
        var excelData = _bids.BuildTemplate();
        return File(excelData, "application/octet-stream", "Template.xlsx");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "shapes-add-member", "outcome": "contradicted", "actual": "IActionResult with new { Id, Name }" },
    { "kind": "operation.returns", "element": "shapes-create-bid", "outcome": "contradicted", "actual": "IActionResult with AnalysisDTO" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "shapes-add-member" },
    { "rule": "claim/operation.returns", "severity": "error", "subject": "shapes-create-bid" }
  ]
}
```

## mvc-view-results

An MVC action renders a view, and the model the view renders is what it returns, as Intent's MVC module models it: `View(vm)` is `LoginViewModel`, and `View()` and `View("Done")`, a view's name, render none, so `Index` and `Done` return nothing. `Redirect` returns `this.LoadingPage("Redirect", returnUrl)`, a controller extension method of the code's own whose body is `controller.View(viewName, new RedirectViewModel { ... })`, so it returns `RedirectViewModel`. `Revoke` only redirects, so it returns nothing. The POST `Logout` renders `LoggedOutViewModel` on one path and, through `LoadingPage`, `RedirectViewModel` on the other. An MVC action may render several pages and the model states one, so a view model it renders verifies: `LoggedOutViewModel`. The GET `Logout(logoutId)` renders its own `LogoutViewModel`, or hands that to the POST `Logout(LogoutInputModel model)`, the overload its type derives from, whose pages are its pages too: `LogoutViewModel` verifies. `Profile` models `LoginViewModel`, a view model it never renders, which is contradicted. The adapted code is eShop's `Identity.API` Quickstart controllers.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Identity.API",
        "elements": [
          { "id": "login-vm", "type": "DTO", "name": "LoginViewModel" },
          { "id": "redirect-vm", "type": "DTO", "name": "RedirectViewModel" },
          { "id": "logged-out-vm", "type": "DTO", "name": "LoggedOutViewModel" },
          { "id": "logout-input", "type": "DTO", "name": "LogoutInputModel" },
          { "id": "logout-vm", "type": "DTO", "name": "LogoutViewModel" },
          { "id": "account", "type": "Service", "name": "AccountController",
            "children": [
              { "id": "login", "type": "Operation", "name": "Login", "typeRef": { "element": "login-vm" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Login" } } },
              { "id": "index", "type": "Operation", "name": "Index",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Index" } } },
              { "id": "done", "type": "Operation", "name": "Done",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Done" } } },
              { "id": "redirect", "type": "Operation", "name": "Redirect", "typeRef": { "element": "redirect-vm" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Redirect" } },
                "children": [{ "id": "redirect-url", "type": "Parameter", "name": "returnUrl", "typeRef": { "type": "string" } }] },
              { "id": "revoke", "type": "Operation", "name": "Revoke",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Account/Revoke" } } },
              { "id": "logout", "type": "Operation", "name": "Logout", "typeRef": { "element": "logged-out-vm" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Account/Logout" } },
                "children": [{ "id": "logout-model", "type": "Parameter", "name": "model", "typeRef": { "element": "logout-input" } }] },
              { "id": "logout-get", "type": "Operation", "name": "Logout", "typeRef": { "element": "logout-vm" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Logout" } },
                "children": [{ "id": "logout-get-id", "type": "Parameter", "name": "logoutId", "typeRef": { "type": "string" } }] },
              { "id": "profile", "type": "Operation", "name": "Profile", "typeRef": { "element": "login-vm" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Profile" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Identity", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Identity.API/Quickstart/ViewModels.cs
namespace Identity.API.Quickstart;

public class LoginViewModel { public string ReturnUrl { get; set; } = ""; }

public class RedirectViewModel { public string RedirectUrl { get; set; } = ""; }

public class LoggedOutViewModel { public string ClientName { get; set; } = ""; }

public class LogoutInputModel { public string LogoutId { get; set; } = ""; }

public class LogoutViewModel : LogoutInputModel { public bool ShowLogoutPrompt { get; set; } }
```

```csharp file src/Identity.API/Quickstart/Extensions.cs
namespace Identity.API.Quickstart;

public static class Extensions
{
    public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
    {
        controller.HttpContext.Response.StatusCode = 200;
        return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
    }
}
```

```csharp file src/Identity.API/Quickstart/AccountController.cs
namespace Identity.API.Quickstart;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        var vm = new LoginViewModel();
        return View(vm);
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Done() => View("Done");

    [HttpGet]
    public IActionResult Redirect(string returnUrl)
    {
        return this.LoadingPage("Redirect", returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Revoke()
    {
        await Task.Yield();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var vm = new LogoutViewModel { LogoutId = logoutId };
        if (!vm.ShowLogoutPrompt)
        {
            return await Logout(vm);
        }

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Logout(LogoutInputModel model)
    {
        await Task.Yield();
        if (model.LogoutId == "")
        {
            return this.LoadingPage("Redirect", "/");
        }

        return View("LoggedOut", new LoggedOutViewModel());
    }

    [HttpGet]
    public IActionResult Profile() => View(new RedirectViewModel());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "login", "outcome": "verified", "expected": "LoginViewModel", "actual": "IActionResult with LoginViewModel" },
    { "kind": "operation.returns", "element": "index", "outcome": "verified", "actual": "IActionResult with no body" },
    { "kind": "operation.returns", "element": "done", "outcome": "verified", "actual": "IActionResult with no body" },
    { "kind": "operation.returns", "element": "redirect", "outcome": "verified", "actual": "IActionResult with RedirectViewModel" },
    { "kind": "operation.returns", "element": "revoke", "outcome": "verified", "actual": "Task<IActionResult> with no body" },
    { "kind": "operation.returns", "element": "logout", "outcome": "verified", "expected": "LoggedOutViewModel", "actual": "Task<IActionResult> with RedirectViewModel or LoggedOutViewModel" },
    { "kind": "operation.returns", "element": "logout-get", "outcome": "verified", "expected": "LogoutViewModel" },
    { "kind": "operation.returns", "element": "profile", "outcome": "contradicted", "expected": "LoginViewModel", "actual": "IActionResult with RedirectViewModel" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "profile", "messageContains": "RedirectViewModel" },
    { "rule": "claim/operation.returns", "subject": "logout", "count": 0 },
    { "rule": "claim/operation.returns", "subject": "logout-get", "count": 0 },
    { "rule": "claim/operation.returns", "subject": "login", "count": 0 }
  ]
}
```

## mvc-view-results-declared-type

A model may state an action's own result type: it is what the signature says, true whatever the body builds, if less than the body's type would say. The Services designer requires a GET to return a type, so an MVC GET that renders nothing can only be modelled with it: `AccessDenied` renders `View()`, `Challenge` only challenges, and `Home.Index` renders `View()` or returns `NotFound()`. `Profile` renders `RedirectViewModel`, which the model could state, and states `IActionResult` instead. Each is modelled as `IActionResult` and verifies, without its body being read. The adapted code is eShop's `Identity.API` Quickstart controllers.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Identity.API",
        "elements": [
          { "id": "account", "type": "Service", "name": "AccountController",
            "children": [
              { "id": "access-denied", "type": "Operation", "name": "AccessDenied", "typeRef": { "type": "IActionResult" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/AccessDenied" } } },
              { "id": "challenge", "type": "Operation", "name": "Challenge", "typeRef": { "type": "IActionResult" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Challenge" } },
                "children": [{ "id": "challenge-scheme", "type": "Parameter", "name": "scheme", "typeRef": { "type": "string" } }] },
              { "id": "profile", "type": "Operation", "name": "Profile", "typeRef": { "type": "IActionResult" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Profile" } } }
            ] },
          { "id": "home", "type": "Service", "name": "HomeController",
            "children": [
              { "id": "home-index", "type": "Operation", "name": "Index", "typeRef": { "type": "IActionResult" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Home/Index" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Identity", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Identity.API/Quickstart/AccountController.cs
namespace Identity.API.Quickstart;

public class RedirectViewModel { public string RedirectUrl { get; set; } = ""; }

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Challenge(string scheme)
    {
        var props = new AuthenticationProperties();
        return Challenge(props, scheme);
    }

    [HttpGet]
    public IActionResult Profile() => View(new RedirectViewModel());
}

public class HomeController : Controller
{
    private readonly bool _development = true;

    public IActionResult Index()
    {
        if (_development)
        {
            return View();
        }

        return NotFound();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "access-denied", "outcome": "verified", "expected": "IActionResult", "actual": "IActionResult" },
    { "kind": "operation.returns", "element": "challenge", "outcome": "verified", "expected": "IActionResult", "actual": "IActionResult" },
    { "kind": "operation.returns", "element": "home-index", "outcome": "verified", "expected": "IActionResult", "actual": "IActionResult" },
    { "kind": "operation.returns", "element": "profile", "outcome": "verified", "expected": "IActionResult", "actual": "IActionResult" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## action-result-declared-type

A Common.Drift model states a controller action's own result type, as its contract was the action's signature: `GetPrices` returns `IActionResult` and `GetProducts` `Task<IActionResult>`, each with `Ok(data)`, and the model states `IActionResult` for both. That is true of the signature, so both verify. `GetMaterials` states `ProductDTO`, a body the code doesn't build, so its body is read and it is contradicted; `GetCategories` states `IActionResult` over an action that returns `CategoriesDTO` itself, which isn't its result type, and is contradicted too. Adapted from ClientA's controllers.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Api",
        "elements": [
          { "id": "product-dto", "type": "DTO", "name": "ProductDTO" },
          { "id": "catalog-service", "type": "Service", "name": "CatalogController",
            "stereotypes": { "Http Service Settings": { "Route": "api/catalog" } },
            "children": [
              { "id": "get-prices", "type": "Operation", "name": "GetPrices", "typeRef": { "type": "IActionResult" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "prices" } } },
              { "id": "get-products", "type": "Operation", "name": "GetProducts", "typeRef": { "type": "IActionResult" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "products" } } },
              { "id": "get-materials", "type": "Operation", "name": "GetMaterials", "typeRef": { "element": "product-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "materials" } } },
              { "id": "get-categories", "type": "Operation", "name": "GetCategories", "typeRef": { "type": "IActionResult" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "categories" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Api/Controllers/CatalogController.cs
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

public class ProductDTO { public string Code { get; set; } = ""; }

public class PriceDTO { public decimal Price { get; set; } }

public class MaterialDTO { public string Name { get; set; } = ""; }

public class CategoriesDTO { public string Category { get; set; } = ""; }

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    [HttpGet("prices")]
    public IActionResult GetPrices()
    {
        var prices = new PriceDTO();
        return Ok(prices);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        await Task.Yield();
        return Ok(new List<ProductDTO>());
    }

    [HttpGet("materials")]
    public IActionResult GetMaterials() => Ok(new List<MaterialDTO>());

    [HttpGet("categories")]
    public CategoriesDTO GetCategories() => new CategoriesDTO();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-prices", "outcome": "verified", "expected": "IActionResult", "actual": "IActionResult" },
    { "kind": "operation.returns", "element": "get-products", "outcome": "verified", "expected": "IActionResult", "actual": "Task<IActionResult>" },
    { "kind": "operation.returns", "element": "get-materials", "outcome": "contradicted", "expected": "ProductDTO", "actual": "IActionResult with List<MaterialDTO>" },
    { "kind": "operation.returns", "element": "get-categories", "outcome": "contradicted", "expected": "IActionResult" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "get-materials", "messageContains": "MaterialDTO" },
    { "rule": "claim/operation.returns", "severity": "error", "subject": "get-categories", "messageContains": "CategoriesDTO" },
    { "rule": "claim/operation.returns", "subject": "get-prices", "count": 0 },
    { "rule": "claim/operation.returns", "subject": "get-products", "count": 0 }
  ]
}
```

## web-api-2-content-status

A Web API 2 action gives its status first: `Content(HttpStatusCode.OK, model)` succeeds with `model`, and `Content(HttpStatusCode.ExpectationFailed, ex.Message)` is an error result, which the model doesn't state. Any status .NET's `HttpStatusCode` names is read, so the 417 isn't taken for ASP.NET Core's `Content(text)`, a `string` success: the action returns `List<AcmeAddressModel>`, the model's collection.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Crm",
        "elements": [
          { "id": "acme-address-model", "type": "DTO", "name": "AcmeAddressModel",
            "children": [ { "id": "acme-address-model-line1", "type": "DTO-Field", "name": "Line1", "typeRef": { "type": "string" } } ] },
          { "id": "address-controller", "type": "Service", "name": "AddressController",
            "stereotypes": { "Http Service Settings": { "Route": "api/Address" } },
            "children": [
              { "id": "get-address-by-person-id", "type": "Operation", "name": "GetAddressByPersonId", "typeRef": { "element": "acme-address-model", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "GetAddressByPersonId" } },
                "children": [ { "id": "get-address-by-person-id-person-id", "type": "Parameter", "name": "personId", "typeRef": { "type": "int" } } ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Crm/Models/AcmeAddressModel.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.Crm.Models;

public class AcmeAddressModel
{
    public string Line1 { get; set; } = "";
}

public interface IAddressLogic
{
    Task<List<AcmeAddressModel>> GetAddressByPersonId(int personId);
}
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    private readonly IAddressLogic _logic;

    public AddressController(IAddressLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [Route("GetAddressByPersonId")]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        try
        {
            var model = await _logic.GetAddressByPersonId(personId);
            return Content(HttpStatusCode.OK, model);
        }
        catch (Exception ex)
        {
            return Content(HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "verified", "expected": "AcmeAddressModel[]", "actual": "Task<IHttpActionResult> with List<AcmeAddressModel>" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## web-api-2-content-status-contradicted

The action now succeeds with `Content(HttpStatusCode.OK, personId)`, an `int`, against the model's collection: contradicted. The 417 is still an error result, so the finding names only the `int`.

```json case
{ "phase": 4, "base": "web-api-2-content-status", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    private readonly IAddressLogic _logic;

    public AddressController(IAddressLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [Route("GetAddressByPersonId")]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        try
        {
            var model = await _logic.GetAddressByPersonId(personId);
            return Content(HttpStatusCode.OK, personId);
        }
        catch (Exception ex)
        {
            return Content(HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "contradicted", "expected": "AcmeAddressModel[]", "actual": "Task<IHttpActionResult> with int" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "get-address-by-person-id" }
  ]
}
```

## web-api-2-content-status-unknown

The action's error path gives its status as a field, `Content(_failure, ex.Message)`. The field is an `HttpStatusCode`, so this is Web API 2's `Content(status, value)`, never ASP.NET Core's `Content(text)`, and whether it succeeds isn't known: can't verify, not a `string` success.

```json case
{ "phase": 4, "base": "web-api-2-content-status", "kinds": ["operation.returns"], "requirements": ["R2", "R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    private readonly IAddressLogic _logic;
    private readonly HttpStatusCode _failure;

    public AddressController(IAddressLogic logic, HttpStatusCode failure)
    {
        _logic = logic;
        _failure = failure;
    }

    [HttpGet]
    [Route("GetAddressByPersonId")]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        try
        {
            var model = await _logic.GetAddressByPersonId(personId);
            return Content(HttpStatusCode.OK, model);
        }
        catch (Exception ex)
        {
            return Content(_failure, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "cantVerify", "reason": "unreadable" }
  ]
}
```

## web-api-2-content-configure-await

The action awaits the logic with `.ConfigureAwait(false)`, which changes only where it resumes: `model` is still what the task gives, `List<AcmeAddressModel>`.

```json case
{ "phase": 4, "base": "web-api-2-content-status", "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    private readonly IAddressLogic _logic;

    public AddressController(IAddressLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [Route("GetAddressByPersonId")]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        try
        {
            var model = await _logic.GetAddressByPersonId(personId).ConfigureAwait(false);
            return Content(HttpStatusCode.OK, model);
        }
        catch (Exception ex)
        {
            return Content(HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "verified", "expected": "AcmeAddressModel[]", "actual": "Task<IHttpActionResult> with List<AcmeAddressModel>" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## web-api-2-task-from-result

The action hands each result back through `await Task.FromResult(...)`, which is the result it wraps: `Content(HttpStatusCode.OK, model)` succeeds with `List<AcmeAddressModel>`.

```json case
{ "phase": 4, "base": "web-api-2-content-status", "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    private readonly IAddressLogic _logic;

    public AddressController(IAddressLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [Route("GetAddressByPersonId")]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        try
        {
            var model = await _logic.GetAddressByPersonId(personId);
            return await Task.FromResult(Content(HttpStatusCode.OK, model));
        }
        catch (Exception ex)
        {
            return await Task.FromResult(Content(HttpStatusCode.ExpectationFailed, ex.Message));
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "verified", "expected": "AcmeAddressModel[]", "actual": "Task<IHttpActionResult> with List<AcmeAddressModel>" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## web-api-2-response-type-stub

The action is a stub that only throws, so its body builds no success result, and its `[ResponseType(typeof(List<AcmeAddressModel>))]` says what it returns: the model's collection.

```json case
{ "phase": 4, "base": "web-api-2-content-status", "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    [HttpGet]
    [Route("GetAddressByPersonId")]
    [ResponseType(typeof(List<AcmeAddressModel>))]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        throw new NotImplementedException();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "verified", "expected": "AcmeAddressModel[]", "actual": "Task<IHttpActionResult> with List<AcmeAddressModel>" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## web-api-2-response-type-stub-contradicted

The stub's `[ResponseType]` now names one `AcmeAddressModel`, against the model's collection: contradicted.

```json case
{ "phase": 4, "base": "web-api-2-content-status", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    [HttpGet]
    [Route("GetAddressByPersonId")]
    [ResponseType(typeof(AcmeAddressModel))]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        throw new NotImplementedException();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "contradicted", "expected": "AcmeAddressModel[]", "actual": "Task<IHttpActionResult> with AcmeAddressModel" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "get-address-by-person-id" }
  ]
}
```

## web-api-2-response-type-body-first

The action's body succeeds with `Content(HttpStatusCode.OK, personId)`, an `int`, and its `[ResponseType]` still names the collection. The body comes first and the attribute answers only when the body builds no success, so a changed return that left its attribute behind is still caught: contradicted.

```json case
{ "phase": 4, "base": "web-api-2-content-status", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AddressController.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Acme.Crm.Models;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    [HttpGet]
    [Route("GetAddressByPersonId")]
    [ResponseType(typeof(List<AcmeAddressModel>))]
    public async Task<IHttpActionResult> GetAddressByPersonId(int personId)
    {
        await Task.Yield();
        return Content(HttpStatusCode.OK, personId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-address-by-person-id", "outcome": "contradicted", "expected": "AcmeAddressModel[]", "actual": "Task<IHttpActionResult> with int" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "get-address-by-person-id" }
  ]
}
```

## action-result-produces-response-type

ASP.NET Core stubs that only throw state what they return with `[ProducesResponseType]`: `typeof(ProductDTO[])` with a 200, the generic `ProducesResponseType<ProductDTO>(200)` beside an error response, which the model doesn't state, and a 204 with no type, which has no body.

```json case
{ "phase": 4, "base": "action-result-body", "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Api/Controllers/ProductCatalogController.cs
using Acme.Api.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Api.Controllers;

[ApiController]
[Route("api/product-catalog")]
public class ProductCatalogController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ProductDTO[]), StatusCodes.Status200OK)]
    public IActionResult GetAll() => throw new NotSupportedException();

    [HttpGet("{id}")]
    [ProducesResponseType<ProductDTO>(200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int id) => throw new NotSupportedException();

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(int id) => throw new NotSupportedException();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "product-catalog-get-all", "outcome": "verified", "expected": "ProductDTO[]", "actual": "IActionResult with ProductDTO[]" },
    { "kind": "operation.returns", "element": "product-catalog-get", "outcome": "verified", "expected": "ProductDTO", "actual": "IActionResult with ProductDTO" },
    { "kind": "operation.returns", "element": "product-catalog-delete", "outcome": "verified", "actual": "IActionResult with no body" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## component-operation

A Component Operation is a method of the component's `@code` block. `OnBrandSelected` has no type reference, and its method returns a bare `Task`, which means nothing; `GetBrandCount` returns `int`.

```json case
{ "phase": 6, "kinds": ["operation.returns"], "traces": ["ESC-FE-26"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "catalog-search-brand-changed", "type": "Event Emitter", "name": "BrandChanged", "typeRef": { "type": "int", "nullable": true } },
              { "id": "catalog-search-on-brand-selected", "type": "Component Operation", "name": "OnBrandSelected",
                "children": [
                  { "id": "catalog-search-on-brand-selected-brand-id", "type": "Parameter", "name": "brandId", "typeRef": { "type": "int", "nullable": true } }
                ] },
              { "id": "catalog-search-get-brand-count", "type": "Component Operation", "name": "GetBrandCount", "typeRef": { "type": "int" } }
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

```razor file src/WebAppComponents/Catalog/CatalogSearch.razor
<div class="catalog-search">
    @foreach (var brand in catalogBrands)
    {
        <a href="" @onclick="() => OnBrandSelected(brand.Id)">@brand.Brand</a>
    }
</div>

@code {
    IEnumerable<CatalogBrand> catalogBrands = [];

    [Parameter] public EventCallback<int?> BrandChanged { get; set; }

    private async Task OnBrandSelected(int? brandId)
    {
        await BrandChanged.InvokeAsync(brandId);
    }

    private int GetBrandCount() => catalogBrands.Count();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "catalog-search-on-brand-selected", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:13" },
    { "kind": "operation.returns", "element": "catalog-search-get-brand-count", "outcome": "verified", "expected": "int", "actual": "int", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:18" }
  ]
}
```

## model-definition-operation

A Model Definition's Operation is a public method of the record: `PriceWithTax` returns `decimal`.

```json case
{ "phase": 6, "kinds": ["operation.returns"], "traces": ["ESC-FE-46"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "catalog-item-price", "type": "Property", "name": "Price", "typeRef": { "type": "decimal" } },
              { "id": "catalog-item-price-with-tax", "type": "Operation", "name": "PriceWithTax", "typeRef": { "type": "decimal" },
                "children": [
                  { "id": "catalog-item-price-with-tax-rate", "type": "Parameter", "name": "rate", "typeRef": { "type": "decimal" } }
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

```csharp file src/WebAppComponents/Catalog/CatalogItem.cs
namespace Shop.WebAppComponents.Catalog;

public record CatalogItem(decimal Price)
{
    public decimal PriceWithTax(decimal rate) => Price * (1 + rate);
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "catalog-item-price-with-tax", "outcome": "verified", "expected": "decimal", "actual": "decimal", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:5" }
  ]
}
```

## service-proxy-operation

A Service Proxy's Operation is a public instance method of the proxy class. `GetCatalogItem` returns `Task<CatalogItem?>`, a `CatalogItem`. `GetCatalogItems` has two overloads; the model's parameter names, `[ids]`, are the second overload's and not the first's, `[pageIndex, pageSize]`, so the `IEnumerable<int> ids` overload is chosen, whose `Task<List<CatalogItem>>` is a collection of `CatalogItem`. The paged overload, which the model doesn't mention, isn't compared.

```json case
{ "phase": 6, "kinds": ["operation.returns"], "traces": ["ESC-FE-53"], "requirements": ["R2", "R41", "R42", "R44"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "catalog-item-id", "type": "Property", "name": "Id", "typeRef": { "type": "int" } }
            ] },
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-item", "type": "Operation", "name": "GetCatalogItem", "typeRef": { "element": "catalog-item", "nullable": true },
                "children": [
                  { "id": "catalog-service-get-item-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] },
              { "id": "catalog-service-get-items", "type": "Operation", "name": "GetCatalogItems", "typeRef": { "element": "catalog-item", "collection": true },
                "children": [
                  { "id": "catalog-service-get-items-ids", "type": "Parameter", "name": "ids", "typeRef": { "type": "int", "collection": true } }
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

public record CatalogItem(int Id);

public record CatalogResult(int PageIndex, int PageSize, List<CatalogItem> Data);
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using Shop.WebAppComponents.Catalog;

namespace Shop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }

    public async Task<CatalogResult> GetCatalogItems(int pageIndex, int pageSize)
    {
        var uri = $"{remoteServiceBaseUrl}items?pageIndex={pageIndex}&pageSize={pageSize}";
        var result = await httpClient.GetFromJsonAsync<CatalogResult>(uri);
        return result!;
    }

    public async Task<List<CatalogItem>> GetCatalogItems(IEnumerable<int> ids)
    {
        var uri = $"{remoteServiceBaseUrl}items/by?ids={string.Join("&ids=", ids)}";
        var result = await httpClient.GetFromJsonAsync<List<CatalogItem>>(uri);
        return result!;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "catalog-service-get-item", "outcome": "verified", "expected": "CatalogItem?", "at": "src/WebAppComponents/Services/CatalogService.cs:10" },
    { "kind": "operation.returns", "element": "catalog-service-get-items", "outcome": "verified", "expected": "CatalogItem[]", "at": "src/WebAppComponents/Services/CatalogService.cs:23" }
  ]
}
```

## java-response-entity

The same claim over a Spring MVC controller. The Spring MVC pack's conventions name a Service's type `{Name|trimEnd:Service}Controller`, so `InventoryService` is its `@RestController` `InventoryController`, as the Java spike's checks map it, and list `ResponseEntity<T>` among their result wrappers, so a handler's return type is read through it: `ResponseEntity<StockLevelDto>` returns `StockLevelDto`, and a plain `List<StockLevelDto>` a collection of it.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "traces": ["JAV-SVC-08"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "stock-level-dto", "type": "DTO", "name": "StockLevelDto",
            "children": [
              { "id": "stock-level-dto-product-id", "type": "DTO-Field", "name": "ProductId", "typeRef": { "type": "guid" } },
              { "id": "stock-level-dto-quantity-on-hand", "type": "DTO-Field", "name": "QuantityOnHand", "typeRef": { "type": "int" } }
            ] },
          { "id": "inventory-service", "type": "Service", "name": "InventoryService",
            "children": [
              { "id": "inventory-service-get-stock-level", "type": "Operation", "name": "GetStockLevel", "typeRef": { "element": "stock-level-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "api/inventory/{productId}" } },
                "children": [
                  { "id": "inventory-service-get-stock-level-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } }
                ] },
              { "id": "inventory-service-get-stock-levels", "type": "Operation", "name": "GetStockLevels", "typeRef": { "element": "stock-level-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "api/inventory" } },
                "children": [
                  { "id": "inventory-service-get-stock-levels-low-stock-only", "type": "Parameter", "name": "lowStockOnly", "typeRef": { "type": "bool" } }
                ] }
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

```java file src/main/java/com/eshop/backend/application/StockLevelDto.java
package com.eshop.backend.application;

import java.util.UUID;

public record StockLevelDto(UUID productId, int quantityOnHand) {
}
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevelDto;
import com.eshop.backend.application.StockLevels;
import java.util.List;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}")
    public ResponseEntity<StockLevelDto> getStockLevel(@PathVariable UUID productId) {
        return ResponseEntity.ok(stockLevels.get(productId));
    }

    @GetMapping("/api/inventory")
    public List<StockLevelDto> getStockLevels(@RequestParam boolean lowStockOnly) {
        return stockLevels.list(lowStockOnly);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "inventory-service-get-stock-level", "outcome": "verified", "expected": "StockLevelDto" },
    { "kind": "operation.returns", "element": "inventory-service-get-stock-levels", "outcome": "verified", "expected": "StockLevelDto[]", "actual": "List<StockLevelDto>" }
  ]
}
```

## java-response-entity-contradicted

`getStockLevel` now returns `ResponseEntity<List<StockLevelDto>>`, a collection, against the model's single `StockLevelDto`: contradicted.

```json case
{ "phase": 4, "base": "java-response-entity", "kinds": ["operation.returns"], "traces": ["JAV-SVC-08"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevelDto;
import com.eshop.backend.application.StockLevels;
import java.util.List;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}")
    public ResponseEntity<List<StockLevelDto>> getStockLevel(@PathVariable UUID productId) {
        return ResponseEntity.ok(List.of(stockLevels.get(productId)));
    }

    @GetMapping("/api/inventory")
    public List<StockLevelDto> getStockLevels(@RequestParam boolean lowStockOnly) {
        return stockLevels.list(lowStockOnly);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "inventory-service-get-stock-level", "outcome": "contradicted", "expected": "StockLevelDto" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "inventory-service-get-stock-level" }
  ]
}
```

## java-response-entity-built

A handler returning `ResponseEntity<Object>` or `ResponseEntity<?>` doesn't state its body's type, so the Spring MVC stack reads what the handler builds its successful responses with, each a chain from a `ResponseEntity` factory. `ResponseEntity.ok(stockLevels.get(productId))` gives what `StockLevels.get` returns, `StockLevelDto`; `ResponseEntity.noContent().build()` has no body, so `receiveStock` returns nothing; `badRequest().build()` is an error response, which the model doesn't state.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "traces": ["JAV-SVC-08"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "stock-level-dto", "type": "DTO", "name": "StockLevelDto",
            "children": [
              { "id": "stock-level-dto-product-id", "type": "DTO-Field", "name": "ProductId", "typeRef": { "type": "guid" } }
            ] },
          { "id": "inventory-service", "type": "Service", "name": "InventoryService",
            "children": [
              { "id": "inventory-service-get-stock-level", "type": "Operation", "name": "GetStockLevel", "typeRef": { "element": "stock-level-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "api/inventory/{productId}" } },
                "children": [
                  { "id": "inventory-service-get-stock-level-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } }
                ] },
              { "id": "inventory-service-receive-stock", "type": "Operation", "name": "ReceiveStock",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "api/inventory/{productId}/receive" } },
                "children": [
                  { "id": "inventory-service-receive-stock-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } }
                ] }
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

```java file src/main/java/com/eshop/backend/application/StockLevelDto.java
package com.eshop.backend.application;

import java.util.UUID;

public record StockLevelDto(UUID productId) {
}
```

```java file src/main/java/com/eshop/backend/application/StockLevels.java
package com.eshop.backend.application;

import java.util.UUID;

public class StockLevels {
    public StockLevelDto get(UUID productId) {
        return new StockLevelDto(productId);
    }

    public StockLevelDto receive(UUID productId) {
        return new StockLevelDto(productId);
    }
}
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}")
    public ResponseEntity<Object> getStockLevel(@PathVariable UUID productId) {
        if (productId == null) return ResponseEntity.badRequest().build();
        return ResponseEntity.ok(stockLevels.get(productId));
    }

    @PostMapping("/api/inventory/{productId}/receive")
    public ResponseEntity<?> receiveStock(@PathVariable UUID productId) {
        stockLevels.receive(productId);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "inventory-service-get-stock-level", "outcome": "verified", "expected": "StockLevelDto", "actual": "ResponseEntity<Object> with StockLevelDto" },
    { "kind": "operation.returns", "element": "inventory-service-receive-stock", "outcome": "verified", "expected": "void", "actual": "ResponseEntity<?> with no body" }
  ]
}
```

## java-response-entity-built-contradicted

`getStockLevel` now builds its response with no body, `ResponseEntity.ok().build()`, against the model's `StockLevelDto`; `receiveStock` builds a `201 Created` with the `StockLevelDto` `receive` returns, against a model that returns nothing. Both are contradicted.

```json case
{ "phase": 4, "base": "java-response-entity-built", "kinds": ["operation.returns"], "traces": ["JAV-SVC-08"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import java.util.UUID;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}")
    public ResponseEntity<Object> getStockLevel(@PathVariable UUID productId) {
        stockLevels.get(productId);
        return ResponseEntity.ok().build();
    }

    @PostMapping("/api/inventory/{productId}/receive")
    public ResponseEntity<?> receiveStock(@PathVariable UUID productId) {
        return ResponseEntity.status(HttpStatus.CREATED).body(stockLevels.receive(productId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "inventory-service-get-stock-level", "outcome": "contradicted", "expected": "StockLevelDto", "actual": "ResponseEntity<Object> with no body" },
    { "kind": "operation.returns", "element": "inventory-service-receive-stock", "outcome": "contradicted", "expected": "void", "actual": "ResponseEntity<?> with StockLevelDto" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "inventory-service-get-stock-level" },
    { "rule": "claim/operation.returns", "severity": "error", "subject": "inventory-service-receive-stock" }
  ]
}
```

## java-response-entity-built-unknown

`getStockLevel` builds its response with what a library call gives, `cache.get(productId)` on a `Map<UUID, Object>`, so what it returns isn't known: can't verify, never a guess.

```json case
{ "phase": 4, "base": "java-response-entity-built", "kinds": ["operation.returns"], "traces": ["JAV-SVC-08"], "requirements": ["R2", "R11"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import java.util.Map;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final Map<UUID, Object> cache;

    public InventoryController(Map<UUID, Object> cache) {
        this.cache = cache;
    }

    @GetMapping("/api/inventory/{productId}")
    public ResponseEntity<Object> getStockLevel(@PathVariable UUID productId) {
        return ResponseEntity.ok(cache.get(productId));
    }

    @PostMapping("/api/inventory/{productId}/receive")
    public ResponseEntity<?> receiveStock(@PathVariable UUID productId) {
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "inventory-service-get-stock-level", "outcome": "cantVerify", "reason": "unresolved", "expected": "StockLevelDto" },
    { "kind": "operation.returns", "element": "inventory-service-receive-stock", "outcome": "verified", "expected": "void" }
  ]
}
```

## java-response-entity-helper

The controller builds its responses with the code's own helper, `Http.created(x)`, whose body is
`ResponseEntity.status(HttpStatus.CREATED).body(body)`: the helper is read as the handler's own response, its `body`
parameter given what the call passes, so `getStockLevel` returns the `StockLevelDto` `stockLevels.get` gives and
`receiveStock`'s `Http.created(null)` has no body. `Http.badRequest()` builds an error response, which isn't the operation's
result.

```json case
{ "phase": 4, "base": "java-response-entity-built", "kinds": ["operation.returns"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/Http.java
package com.eshop.backend.api;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

final class Http {
    private Http() {
    }

    static ResponseEntity<Object> problem(HttpStatus status, String title) {
        return ResponseEntity.status(status).body(title);
    }

    static ResponseEntity<Object> badRequest() {
        return problem(HttpStatus.BAD_REQUEST, "Bad Request");
    }

    static ResponseEntity<Object> created(Object body) {
        return ResponseEntity.status(HttpStatus.CREATED).body(body);
    }
}
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}")
    public ResponseEntity<Object> getStockLevel(@PathVariable UUID productId) {
        if (productId == null) return Http.badRequest();
        return Http.created(stockLevels.get(productId));
    }

    @PostMapping("/api/inventory/{productId}/receive")
    public ResponseEntity<Object> receiveStock(@PathVariable UUID productId) {
        stockLevels.receive(productId);
        return Http.created(null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "inventory-service-get-stock-level", "outcome": "verified", "expected": "StockLevelDto", "actual": "ResponseEntity<Object> with StockLevelDto",
      "at": "src/main/java/com/eshop/backend/api/InventoryController.java:19" },
    { "kind": "operation.returns", "element": "inventory-service-receive-stock", "outcome": "verified", "expected": "void", "actual": "ResponseEntity<Object> with no body" }
  ]
}
```

## java-response-entity-helper-contradicted

A planted drift: `getStockLevel` now answers with the `productId` it was given, through the same helper, against the model's `StockLevelDto`.

```json case
{ "phase": 4, "base": "java-response-entity-helper", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}")
    public ResponseEntity<Object> getStockLevel(@PathVariable UUID productId) {
        return Http.created(productId);
    }

    @PostMapping("/api/inventory/{productId}/receive")
    public ResponseEntity<Object> receiveStock(@PathVariable UUID productId) {
        stockLevels.receive(productId);
        return Http.created(null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "inventory-service-get-stock-level", "outcome": "contradicted", "expected": "StockLevelDto", "actual": "ResponseEntity<Object> with UUID" },
    { "kind": "operation.returns", "element": "inventory-service-receive-stock", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "inventory-service-get-stock-level" }
  ]
}
```

## typescript-proxy-operation

The Java spike's Front End Proxies are object literals in a TypeScript file, `export const api = { … }`, whose methods are the Proxy Operations. The TypeScript reader (intent.typescript, `later`) makes the object literal an `object` type with a `method` node for each method, and the storefront's house pack's conventions name a Proxy's type in camel case, so the structure comparison locates each Proxy Operation's method in `api`. The method's return is the argument of its `Promise<…>`, the TypeScript profile's async wrapper, so `ProductDto` is the interface the Contract is located as. `ProductDto | null` matches the model's nullable `ProductDto`, nullability aside, and `ProductDto[]` a collection of it.

```json case
{ "phase": "later", "kinds": ["operation.returns"], "traces": ["JAV-FE-09"], "requirements": ["R2", "R28", "R42"], "packs": ["intent.core", "intent.typescript", "house.storefront"], "language": "typescript" }
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
        { "kind": "type.exists", "for": { "element": "Proxy" }, "value": "self", "bindings": { "Name": "name" }, "own": ["Name"], "source": "the Proxy" },
        { "kind": "member.exists", "for": { "element": "Proxy Operation" }, "value": "self", "dependsOn": "parent:type.exists", "source": "the Proxy Operation" },
        {
          "kind": "operation.returns",
          "for": { "element": "Proxy Operation" },
          "value": "type",
          "dependsOn": "member.exists",
          "source": "the Proxy Operation's type"
        }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Contract": { "typeKinds": ["interface", "typeAlias"], "locations": ["src/api/*.ts"], "note": "Contracts are exported interfaces or type aliases in src/api" },
        "Proxy": { "name": "{Name|camel}", "typeKinds": ["object"], "note": "The storefront: a Proxy is the object of its methods" }
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
          { "id": "product-dto", "type": "Contract", "name": "ProductDto",
            "children": [
              { "id": "product-dto-id", "type": "Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] },
          { "id": "api", "type": "Proxy", "name": "Api",
            "stereotypes": { "Implementation": { "Path": "src/api/client.ts" } },
            "children": [
              { "id": "api-get-product-by-id", "type": "Proxy Operation", "name": "GetProductById", "typeRef": { "element": "product-dto", "nullable": true },
                "children": [
                  { "id": "api-get-product-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
                ] },
              { "id": "api-list-products", "type": "Proxy Operation", "name": "ListProducts", "typeRef": { "element": "product-dto", "collection": true } }
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

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string): Promise<T> {
    const res = await fetch(path, { method });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products/{id}
    getProductById(id: string): Promise<ProductDto | null> {
        return request<ProductDto | null>('GET', `/api/products/${encodeURIComponent(id)}`);
    },

    // GET /api/products
    listProducts(): Promise<ProductDto[]> {
        return request<ProductDto[]>('GET', '/api/products');
    },
};
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "api-get-product-by-id", "outcome": "verified", "expected": "ProductDto?", "actual": "Promise<ProductDto|null>", "at": "src/api/client.ts:10" },
    { "kind": "operation.returns", "element": "api-list-products", "outcome": "verified", "expected": "ProductDto[]", "actual": "Promise<ProductDto[]>", "at": "src/api/client.ts:15" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## typescript-proxy-operation-contradicted

`listProducts` resolves to one `ProductDto`, against the model's collection: contradicted.

```json case
{ "phase": "later", "base": "typescript-proxy-operation", "kinds": ["operation.returns"], "traces": ["JAV-FE-09"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.typescript", "house.storefront"], "language": "typescript" }
```

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string): Promise<T> {
    const res = await fetch(path, { method });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products/{id}
    getProductById(id: string): Promise<ProductDto | null> {
        return request<ProductDto | null>('GET', `/api/products/${encodeURIComponent(id)}`);
    },

    // GET /api/products
    listProducts(): Promise<ProductDto> {
        return request<ProductDto>('GET', '/api/products');
    },
};
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "api-list-products", "outcome": "contradicted", "expected": "ProductDto[]", "actual": "Promise<ProductDto>", "at": "src/api/client.ts:15" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "api-list-products" }
  ]
}
```

## web-api-2-untyped-body-declared

A Web API 2 action succeeds with what a library gives, `LegacyStore.Lookup(orderCode)`, whose type the code doesn't say, and declares its success with `[ResponseType(typeof(string))]`. The declaration is the code's own statement of the body, so the untyped success is a `string`, the model's return type: verified. The 417 is still an error result.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Crm",
        "elements": [
          { "id": "acme-controller", "type": "Service", "name": "AcmeController",
            "stereotypes": { "Http Service Settings": { "Route": "api/Acme" } },
            "children": [
              { "id": "get-order-status", "type": "Operation", "name": "GetOrderStatusByOrderCode", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "GetOrderStatusByOrderCode" } },
                "children": [ { "id": "get-order-status-order-code", "type": "Parameter", "name": "orderCode", "typeRef": { "type": "string" } } ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Crm/Controllers/AcmeController.cs
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Legacy.Data;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Acme")]
public class AcmeController : ApiController
{
    [HttpGet]
    [ResponseType(typeof(string))]
    [Route("GetOrderStatusByOrderCode")]
    public IHttpActionResult GetOrderStatusByOrderCode(string orderCode)
    {
        try
        {
            return Content(HttpStatusCode.OK, LegacyStore.Lookup(orderCode));
        }
        catch (Exception ex)
        {
            return Content(HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-order-status", "outcome": "verified", "expected": "string", "actual": "IHttpActionResult with string" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## web-api-2-untyped-body-declared-contradicted

The action now declares `[ResponseType(typeof(int))]`, so its untyped success is an `int` against the model's `string`: contradicted.

```json case
{ "phase": 4, "base": "web-api-2-untyped-body-declared", "kinds": ["operation.returns"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AcmeController.cs
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Legacy.Data;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Acme")]
public class AcmeController : ApiController
{
    [HttpGet]
    [ResponseType(typeof(int))]
    [Route("GetOrderStatusByOrderCode")]
    public IHttpActionResult GetOrderStatusByOrderCode(string orderCode)
    {
        try
        {
            return Content(HttpStatusCode.OK, LegacyStore.Lookup(orderCode));
        }
        catch (Exception ex)
        {
            return Content(HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-order-status", "outcome": "contradicted", "expected": "string", "actual": "IHttpActionResult with int" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "get-order-status" }
  ]
}
```

## web-api-2-untyped-body-undeclared

Without a `[ResponseType]`, nothing in the code says what `LegacyStore.Lookup(orderCode)` is: can't verify, never a guess.

```json case
{ "phase": 4, "base": "web-api-2-untyped-body-declared", "kinds": ["operation.returns"], "requirements": ["R2", "R42"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Acme.Crm/Controllers/AcmeController.cs
using System;
using System.Net;
using System.Web.Http;
using Legacy.Data;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Acme")]
public class AcmeController : ApiController
{
    [HttpGet]
    [Route("GetOrderStatusByOrderCode")]
    public IHttpActionResult GetOrderStatusByOrderCode(string orderCode)
    {
        try
        {
            return Content(HttpStatusCode.OK, LegacyStore.Lookup(orderCode));
        }
        catch (Exception ex)
        {
            return Content(HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-order-status", "outcome": "cantVerify", "reason": "unresolved" }
  ]
}
```

## csharp-expression-types

What C# itself says a value is, with no `[ResponseType]` to fall back on: a comparison is a `bool` whatever it compares (`LegacyStore.Flag(code).ToLowerInvariant() == "y"`), a `var` local is its initializer's type when a later value isn't known (`var vatRate = -1.00m;`, a signed decimal), and `string.Empty` is a `string`, so a conditional of it and a string parameter is one. Each action's success is read from its body and matches the model: verified.

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Crm",
        "elements": [
          { "id": "plan-controller", "type": "Service", "name": "PlanController",
            "stereotypes": { "Http Service Settings": { "Route": "api/Plan" } },
            "children": [
              { "id": "plan-is-bundle", "type": "Operation", "name": "PlanIsBundlePlan", "typeRef": { "type": "bool" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "PlanIsBundlePlan/{planCode}" } },
                "children": [ { "id": "plan-is-bundle-plan-code", "type": "Parameter", "name": "planCode", "typeRef": { "type": "string" } } ] },
              { "id": "apply-vat", "type": "Operation", "name": "ApplyVatForPartner", "typeRef": { "type": "decimal" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "VatForPartner/{referenceId}" } },
                "children": [ { "id": "apply-vat-reference-id", "type": "Parameter", "name": "referenceId", "typeRef": { "type": "int" } } ] },
              { "id": "get-order-code", "type": "Operation", "name": "GetOrderCode", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "GetOrderCode" } },
                "children": [ { "id": "get-order-code-order-code", "type": "Parameter", "name": "orderCode", "typeRef": { "type": "string" } } ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Crm/Controllers/PlanController.cs
using System;
using System.Net;
using System.Web.Http;
using Legacy.Data;

namespace Acme.Crm.Controllers;

[RoutePrefix("api/Plan")]
public class PlanController : ApiController
{
    [HttpGet]
    [Route("PlanIsBundlePlan/{planCode}")]
    public IHttpActionResult PlanIsBundlePlan(string planCode)
    {
        if (!string.IsNullOrWhiteSpace(planCode))
        {
            var queryResult = LegacyStore.Flag(planCode).ToLowerInvariant() == "y";
            return Content(HttpStatusCode.OK, queryResult);
        }

        return Content(HttpStatusCode.OK, false);
    }

    [HttpGet]
    [Route("VatForPartner/{referenceId}")]
    public IHttpActionResult ApplyVatForPartner(int referenceId)
    {
        var vatRate = -1.00m;
        if (referenceId > 0)
        {
            vatRate = LegacyStore.Rate(referenceId);
        }

        return Content(HttpStatusCode.OK, vatRate);
    }

    [HttpGet]
    [Route("GetOrderCode")]
    public IHttpActionResult GetOrderCode(string orderCode)
    {
        return Ok(string.IsNullOrEmpty(orderCode) ? string.Empty : orderCode);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "plan-is-bundle", "outcome": "verified", "expected": "bool", "actual": "IHttpActionResult with bool" },
    { "kind": "operation.returns", "element": "apply-vat", "outcome": "verified", "expected": "decimal", "actual": "IHttpActionResult with decimal" },
    { "kind": "operation.returns", "element": "get-order-code", "outcome": "verified", "expected": "string", "actual": "IHttpActionResult with string" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## javascript-proxy-untyped

A proxy written in JavaScript declares no return or parameter types. Against a return type the model states, the claim is can't verify, `unresolved`, with no finding of its own, and listed in the one `engine/notStated` note (R35); against no return type, as `RemoveLine` has, it is void as before. Its parameters are compared by name alone, so `RemoveLine`'s `id` is not stated too, and `AddLine`, whose code takes no `quantity`, is still contradicted.

```json case
{ "phase": 6, "kinds": ["operation.returns", "operation.parameters"], "cases": ["engine/unresolved", "engine/notStated"], "requirements": ["R1", "R2", "R35", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "shop-api", "type": "Proxy", "name": "ShopApi", "children": [
            { "id": "get-product-name", "type": "Proxy Operation", "name": "GetProductName", "typeRef": { "type": "string" },
              "children": [
                { "id": "get-product-name-id", "type": "Parameter", "name": "id", "typeRef": { "type": "string" } }
              ] },
            { "id": "remove-line", "type": "Proxy Operation", "name": "RemoveLine",
              "children": [
                { "id": "remove-line-id", "type": "Parameter", "name": "id", "typeRef": { "type": "string" } }
              ] },
            { "id": "add-line", "type": "Proxy Operation", "name": "AddLine",
              "children": [
                { "id": "add-line-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "string" } },
                { "id": "add-line-quantity", "type": "Parameter", "name": "quantity", "typeRef": { "type": "int" } }
              ] }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```js file src/shopApi.js
import axios from 'axios';

export const shopApi = {
    getProductName(id) {
        return axios.get(`/products/${id}/name`).then((x) => x.data);
    },

    removeLine(id) {
        return axios.delete(`/cart/lines/${id}`);
    },

    addLine(productId) {
        return axios.post('/cart/lines', { productId, quantity: 1 });
    },
};
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-product-name", "outcome": "cantVerify", "reason": "unresolved", "at": "src/shopApi.js:4" },
    { "kind": "operation.parameters", "element": "get-product-name", "outcome": "cantVerify", "reason": "unresolved" },
    { "kind": "operation.returns", "element": "remove-line", "outcome": "verified" },
    { "kind": "operation.parameters", "element": "remove-line", "outcome": "cantVerify", "reason": "unresolved" },
    { "kind": "operation.parameters", "element": "add-line", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 },
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "add-line" },
    { "rule": "engine/notStated", "severity": "info", "messageContains": "Not checked: 3 claims on types their code leaves out (User Interface 3)" }
  ]
}
```

## controller-typed-action-result

A controller action declared to return `ActionResult<T>`, under `Task` or not, returns a `T`: ASP.NET Core sends the `T` it is given, and the wrapper only lets the action return a status result instead (`NotFound()`), which isn't part of the Operation's type. From an adopted React and ASP.NET Core application's controllers (`Task<ActionResult<ActivityDto>> Details(Guid id)`).

```json case
{ "phase": 4, "kinds": ["operation.returns"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] },
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "get-order", "type": "Operation", "name": "GetOrder", "typeRef": { "element": "order-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" } },
                "children": [
                  { "id": "get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
                ] },
              { "id": "get-orders", "type": "Operation", "name": "GetOrders", "typeRef": { "element": "order-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "" } } }
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

```csharp file src/Api/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        return await Task.FromResult(new OrderDto(id));
    }

    [HttpGet]
    public ActionResult<List<OrderDto>> GetOrders() => new List<OrderDto>();
}

public record OrderDto(Guid Id);
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-order", "outcome": "verified", "actual": "OrderDto" },
    { "kind": "operation.returns", "element": "get-orders", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "count": 0 }
  ]
}
```

## controller-typed-action-result-contradicted

The action's `ActionResult<T>` names another type than the Operation's: `T` is compared, not the wrapper.

```json case
{ "phase": 4, "base": "controller-typed-action-result", "kinds": ["operation.returns"], "requirements": ["R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Api/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<string>> GetOrder(Guid id) => await Task.FromResult(id.ToString());

    [HttpGet]
    public ActionResult<List<OrderDto>> GetOrders() => new List<OrderDto>();
}

public record OrderDto(Guid Id);
```

```json expect
{
  "claims": [
    { "kind": "operation.returns", "element": "get-order", "outcome": "contradicted", "actual": "string" },
    { "kind": "operation.returns", "element": "get-orders", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "get-order" }
  ]
}
```
