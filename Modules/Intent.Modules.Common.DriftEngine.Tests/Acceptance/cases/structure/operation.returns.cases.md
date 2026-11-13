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

## typescript-proxy-operation

The Java spike's Front End Proxies are object literals in a TypeScript file, `export const api = { … }`, whose methods are the Proxy Operations. The TypeScript reader (intent.typescript, `later`) gives each method a `method` node, but the object literal is no type the structure comparison could locate, so the storefront's house pack reads the claim with a recipe: the method in the file the Proxy's Implementation stereotype names, and the argument of its `Promise<…>`. That text is read as a TypeScript type and resolved through the file's imports (rules.md, Recipes), so `ProductDto` is the interface the Contract is located as. `ProductDto | null` matches the model's nullable `ProductDto`, nullability aside, and `ProductDto[]` a collection of it.

```json case
{ "phase": "later", "kinds": ["operation.returns"], "traces": ["JAV-FE-09"], "requirements": ["R2", "R28", "R42", "R46"], "packs": ["intent.core", "intent.typescript", "house.storefront"], "language": "typescript" }
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
        {
          "kind": "operation.returns",
          "for": { "element": "Proxy Operation" },
          "bindings": { "Operation": "camel(name)", "ProxyFile": "parent.stereotype('Implementation').Path" },
          "own": ["Operation"],
          "source": "the Proxy Operation's type"
        }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Contract": { "typeKinds": ["interface", "typeAlias"], "locations": ["src/api/*.ts"], "note": "Contracts are exported interfaces or type aliases in src/api" }
      }
    }
  },
  "rules": {
    "Front End": {
      "facts": {
        "operation.returns": {
          "files": "src/**/*.ts",
          "find": "file:{ProxyFile} > method:{Operation}",
          "read": [{ "match": "\\)\\s*:\\s*Promise<(.+)>\\s*\\{" }],
          "note": "Every proxy method is async: its return is the argument of Promise<T>, read as a TypeScript type"
        }
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
    { "kind": "operation.returns", "element": "api-get-product-by-id", "outcome": "verified", "expected": "ProductDto?", "actual": "ProductDto | null", "at": "src/api/client.ts:10" },
    { "kind": "operation.returns", "element": "api-list-products", "outcome": "verified", "expected": "ProductDto[]", "actual": "ProductDto[]", "at": "src/api/client.ts:15" }
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
    { "kind": "operation.returns", "element": "api-list-products", "outcome": "contradicted", "expected": "ProductDto[]", "actual": "ProductDto", "at": "src/api/client.ts:15" }
  ],
  "findings": [
    { "rule": "claim/operation.returns", "severity": "error", "subject": "api-list-products" }
  ]
}
```
