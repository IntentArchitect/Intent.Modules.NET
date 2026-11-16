# security

Whether an endpoint is secured: `Secured`, `Unsecured` or `None`, from the `Secured` and `Unsecured` stereotypes on the Operation or inherited from its Service, against the code's authorization: `RequireAuthorization` and `AllowAnonymous` on a group or an endpoint, `[Authorize]` and `[AllowAnonymous]` on a class or a method, Spring's `@PreAuthorize`, or a Blazor page's `@attribute [Authorize]`. `security` is `stated: always`: an endpoint whose code says nothing about authorization states `None`, so it can verify or contradict from silence.

The fixtures adapt eShopCustom's `Ordering.API` (`OrdersApi.cs`, `Program.cs`), its `Basket.API` gRPC service and `WebApp` pages, and the Java spike's `OrdersController`.

## verified-on-group

The Service is `Secured`, and its Operations inherit it. The router's group calls `RequireAuthorization()`, and each endpoint inherits that from its group, so `CancelOrderAsync` reads `Secured` at the group. Traces ESC-SVC-15.

```json case
{ "phase": 4, "kinds": ["security"], "traces": ["ESC-SVC-15"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" }, "Secured": {} },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } },
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
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders").RequireAuthorization();

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("{orderId:int}", GetOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" },
    { "kind": "security", "element": "get-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ]
}
```

## verified-chained-in-program

As in eShopCustom, the router's group has no authorization of its own: `Program.cs` chains `.RequireAuthorization()` on the call to `MapOrdersApiV1()`, which returns the group, so the requirement applies to the group and every endpoint in it. The reading is located in `Program.cs`. Traces ESC-SVC-14.

```json case
{ "phase": 4, "base": "verified-on-group", "kinds": ["security"], "traces": ["ESC-SVC-14"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("{orderId:int}", GetOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }
}
```

```csharp file src/Ordering.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

var app = builder.Build();

var orders = app.NewVersionedApi("Orders");

orders.MapOrdersApiV1().RequireAuthorization();

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Ordering.API/Program.cs:9" }
  ]
}
```

## verified-unsecured-endpoint

`GetOrderAsync` is `Unsecured` in the model, overriding its Service, and its endpoint calls `.AllowAnonymous()`, overriding its group. `CancelOrderAsync` still inherits `Secured` from both. Traces ESC-SVC-15.

```json case
{ "phase": 4, "base": "verified-on-group", "kinds": ["security"], "traces": ["ESC-SVC-15"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" }, "Secured": {} },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } },
              { "id": "get-order", "type": "Operation", "name": "GetOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{orderId}" }, "Unsecured": {} },
                "children": [
                  { "id": "get-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders").RequireAuthorization();

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("{orderId:int}", GetOrderAsync).AllowAnonymous();

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "get-order", "outcome": "verified", "expected": "Unsecured", "actual": "Unsecured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:10" },
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ]
}
```

## contradicted-not-secured

The model secures the Service, and nothing in the code requires authorization: no group call, no endpoint call, nothing chained in `Program.cs`. Saying nothing states `None`, so the claim is contradicted from silence. Traces ESC-SVC-15.

```json case
{ "phase": 4, "base": "verified-on-group", "kinds": ["security"], "traces": ["ESC-SVC-15"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("{orderId:int}", GetOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "cancel-order", "outcome": "contradicted", "expected": "Secured", "actual": "None", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/security", "severity": "error", "subject": "cancel-order", "messageContains": "None" }
  ]
}
```

## contradicted-code-secures-unmodelled-endpoint

The other direction. Only `CancelOrderAsync` is `Secured` in the model; `GetOrderAsync` has neither stereotype, on itself or its Service, so the model says it is `None`. The code requires authorization on both endpoints, so `GetOrderAsync` is contradicted. Traces ESC-SVC-15.

```json case
{ "phase": 4, "base": "verified-on-group", "kinds": ["security"], "traces": ["ESC-SVC-15"], "requirements": ["R1"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" }, "Secured": {} } },
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
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization();
        api.MapGet("{orderId:int}", GetOrderAsync).RequireAuthorization();

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" },
    { "kind": "security", "element": "get-order", "outcome": "contradicted", "expected": "None", "actual": "Secured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:10" }
  ],
  "findings": [
    { "rule": "claim/security", "severity": "error", "subject": "get-order" }
  ]
}
```

## verified-none-from-silence

The same model, and only `CancelOrderAsync`'s endpoint requires authorization. `GetOrderAsync`'s code says nothing, which states `None`, as the model does: verified from silence.

```json case
{ "phase": 4, "base": "contradicted-code-secures-unmodelled-endpoint", "kinds": ["security"], "requirements": ["R3"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization();
        api.MapGet("{orderId:int}", GetOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "get-order", "outcome": "verified", "expected": "None", "actual": "None", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/security", "subject": "get-order", "count": 0 }
  ]
}
```

## no-rule

Only the core and Services packs are active, so nothing reads how an endpoint is secured. Every security claim is can't verify, `noRule`, one error each, never a silent skip (principle 2).

```json case
{ "phase": 1, "base": "verified-on-group", "kinds": ["security"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "cancel-order", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/security", "severity": "error", "subject": "cancel-order", "messageContains": "no rule" }
  ]
}
```

## grpc-class-and-override

eShopCustom's gRPC services carry its own `gRPC Service` stereotype, which no stock pack knows, so the house pack brings vocabulary for their Operations, and its module's claim reader (`house.readers`) reads them, since ASP.NET Core's reads only controllers and Minimal APIs: `[AllowAnonymous]` or `[Authorize]` on the method, else inherited from the class, else `None`. The class is `[Authorize]`, and `GetBasket` overrides it with `[AllowAnonymous]`; the model secures the Service and marks `GetBasket` `Unsecured`. Traces ESC-SVC-16, its attribute part: a `.RequireAuthorization()` chained on `MapGrpcService<BasketService>()` in another file needs a cross-file rule (a plug-in, as for ESC-SVC-14) that no stock pack declares for gRPC, so it isn't shown here.

```json case
{ "phase": 4, "kinds": ["security"], "traces": ["ESC-SVC-16"], "requirements": ["R2", "R28", "R46"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop", "house.readers"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-service", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": {}, "Secured": {} },
            "children": [
              { "id": "get-basket", "type": "Operation", "name": "GetBasket",
                "stereotypes": { "Unsecured": {} } },
              { "id": "update-basket", "type": "Operation", "name": "UpdateBasket" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basket", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "security",
          "for": { "element": "Operation", "where": "parent.hasStereotype('gRPC Service')" },
          "value": "if(hasStereotype('Unsecured'), 'Unsecured', if(hasStereotype('Secured') || parent.hasStereotype('Secured'), 'Secured', 'None'))",
          "bindings": { "Service": "parent.name", "Operation": "name" },
          "own": ["Operation"],
          "source": "the Secured and Unsecured stereotypes on the gRPC Service and its Operation"
        }
      ]
    }
  }
}
```

```csharp file src/Basket.API/Grpc/BasketService.cs
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace eShop.Basket.API.Grpc;

[Authorize]
public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    [AllowAnonymous]
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return MapToCustomerBasketResponse(data);
    }

    public override async Task<CustomerBasketResponse> UpdateBasket(UpdateBasketRequest request, ServerCallContext context)
    {
        var response = await repository.UpdateBasketAsync(MapToCustomerBasket(context.GetUserIdentity(), request));
        return MapToCustomerBasketResponse(response);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "get-basket", "outcome": "verified", "expected": "Unsecured", "actual": "Unsecured",
      "at": "src/Basket.API/Grpc/BasketService.cs:9" },
    { "kind": "security", "element": "update-basket", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Basket.API/Grpc/BasketService.cs:6" }
  ]
}
```

## blazor-page-authorize

A Blazor page is secured if and only if it is `[Authorize]`. The `intent.ui` pack gives the User Interface designer's vocabulary for a component's `Secured` stereotype, and the Blazor reader reads the `.razor` file's `@attribute [Authorize]`; a page without it states `None`. The `Orders` page is `Secured` and its file has the attribute. Traces ESC-FE-14.

```json case
{ "phase": 4, "kinds": ["security"], "traces": ["ESC-FE-14"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.services", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "Pages",
            "children": [
              { "id": "orders-page", "type": "Component", "name": "Orders",
                "stereotypes": { "Page": { "Route": "/user/orders" }, "Secured": {} } },
              { "id": "catalog-page", "type": "Component", "name": "Catalog",
                "stereotypes": { "Page": { "Route": "/" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "WebApp", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Orders.razor
@page "/user/orders"
@attribute [Authorize]
@inject OrderingService OrderingService

<PageTitle>Orders | AdventureWorks</PageTitle>

<h1>Orders</h1>
```

```razor file src/WebApp/Components/Pages/Catalog.razor
@page "/"
@inject CatalogService CatalogService

<PageTitle>AdventureWorks</PageTitle>

<h1>Ready for a new adventure?</h1>
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "orders-page", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/WebApp/Components/Pages/Orders.razor:2" },
    { "kind": "security", "element": "catalog-page", "outcome": "verified", "expected": "None", "actual": "None", "fromSilence": true }
  ]
}
```

## blazor-page-authorize-unmodelled

The `Catalog` page gains `@attribute [Authorize]`, and the model doesn't secure it: the "only if" half, contradicted. Traces ESC-FE-14.

```json case
{ "phase": 4, "base": "blazor-page-authorize", "kinds": ["security"], "traces": ["ESC-FE-14"], "requirements": ["R1"], "packs": ["intent.core", "intent.services", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Catalog.razor
@page "/"
@attribute [Authorize]
@inject CatalogService CatalogService

<PageTitle>AdventureWorks</PageTitle>

<h1>Ready for a new adventure?</h1>
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "catalog-page", "outcome": "contradicted", "expected": "None", "actual": "Secured",
      "at": "src/WebApp/Components/Pages/Catalog.razor:2" }
  ],
  "findings": [
    { "rule": "claim/security", "severity": "error", "subject": "catalog-page" }
  ]
}
```

## java-pre-authorize

Over Spring MVC, the controller's `@PreAuthorize("isAuthenticated()")` secures every method, and `getOrderById`'s own `@PreAuthorize("permitAll()")` overrides it. The model secures the Service and marks `GetOrderById` `Unsecured`. Traces JAV-SVC-13.

```json case
{ "phase": 4, "kinds": ["security"], "traces": ["JAV-SVC-13"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "stereotypes": { "Secured": {} },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel" } },
                "children": [
                  { "id": "cancel-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
                ] },
              { "id": "get-order-by-id", "type": "Operation", "name": "GetOrderById",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "/api/orders/{orderId}" }, "Unsecured": {} },
                "children": [
                  { "id": "get-order-by-id-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
@PreAuthorize("isAuthenticated()")
public class OrdersController {
    private final OrderService orders;

    public OrdersController(OrderService orders) {
        this.orders = orders;
    }

    @PostMapping("/api/orders/{orderId}/cancel")
    public ResponseEntity<Void> cancelOrder(@PathVariable UUID orderId) {
        orders.cancel(orderId);
        return ResponseEntity.noContent().build();
    }

    @GetMapping("/api/orders/{orderId}")
    @PreAuthorize("permitAll()")
    public OrderDto getOrderById(@PathVariable UUID orderId) {
        return orders.getById(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:13" },
    { "kind": "security", "element": "get-order-by-id", "outcome": "verified", "expected": "Unsecured", "actual": "Unsecured",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:28" }
  ]
}
```
