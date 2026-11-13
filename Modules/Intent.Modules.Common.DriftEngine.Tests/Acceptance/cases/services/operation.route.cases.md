# operation.route

An Operation's `Http Settings` Route, against the route pattern its endpoint is mapped with: the pattern of a minimal API `Map*` call, a controller action's `[Http*]` template, or a Spring `*Mapping` path. `operation.route` is `stated: whenSet` (decision 4). The `route` comparer compares segments by position, ignoring a leading or trailing `/`: a literal matches the same literal ignoring case, and a parameter matches a parameter of the same name ignoring case and constraints, while an optional `?` and a catch-all `*` must agree. So `api/Orders/{id:guid}` is the same route as `/api/orders/{Id}`, but not as `api/orders/{orderId}` or `api/orders/{id?}`.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and the Java spike's `OrdersController`.

## verified

The Operation's Route is `{orderId}` and the endpoint's pattern is `{orderId:int}`: the constraint is dropped, so both sides read `{orderId}`. Traces ESC-SVC-22.

```json case
{ "phase": 4, "kinds": ["operation.route"], "traces": ["ESC-SVC-22"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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

        api.MapGet("{orderId:int}", GetOrderAsync);

        return api;
    }

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
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order", "outcome": "verified", "expected": "{orderId}", "actual": "{orderId}",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ]
}
```

## verified-ignoring-case-slashes-and-constraints

The `route` comparer's own example. A router with no group maps the full route itself, as `api/Orders/{id:guid}`; the Operation's Route is `/api/orders/{Id}`. The leading `/`, the capital `O` in a literal segment, the capital `I` in the parameter's name and the `guid` constraint all fall away, so the claim is verified. The canonical text keeps each side's case and drops the constraint: only the comparison ignores case. Traces ESC-SVC-22.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Ordering.API/Apis/OrdersApi.cs"], "kinds": ["operation.route"], "traces": ["ESC-SVC-22"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-lookup-api", "type": "Service", "name": "OrderLookupApi",
            "children": [
              { "id": "get-order-by-id", "type": "Operation", "name": "GetOrderByIdAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "/api/orders/{Id}" } },
                "children": [
                  { "id": "get-order-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
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

```csharp file src/Ordering.API/Apis/OrderLookupApi.cs
namespace Ordering.API.Apis;

public static class OrderLookupApi
{
    public static IEndpointRouteBuilder MapOrderLookupApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/Orders/{id:guid}", GetOrderByIdAsync);

        return app;
    }

    public static async Task<Results<Ok<Order>, NotFound>> GetOrderByIdAsync(Guid id, [AsParameters] OrderServices services)
    {
        var order = await services.Queries.FindOrderAsync(id);
        return order is null ? TypedResults.NotFound() : TypedResults.Ok(order);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order-by-id", "outcome": "verified", "expected": "api/orders/{Id}", "actual": "api/Orders/{id}",
      "at": "src/Ordering.API/Apis/OrderLookupApi.cs:7" }
  ]
}
```

## contradicted

The endpoint's pattern has an extra literal segment, `by-id/{orderId:int}`. Constraints are still dropped, so the code's route reads `by-id/{orderId}`. Traces ESC-SVC-22.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.route"], "traces": ["ESC-SVC-22"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet("by-id/{orderId:int}", GetOrderAsync);

        return api;
    }

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
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order", "outcome": "contradicted", "expected": "{orderId}", "actual": "by-id/{orderId}",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "get-order", "messageContains": "by-id/{orderId}" }
  ]
}
```

## contradicted-parameter-name

The endpoint's pattern is `{id:int}`: a parameter in the same position as the model's `{orderId}`, but with another name. Parameters match by position and name, so the routes differ. Traces ESC-SVC-22 (parameters by position).

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.route"], "traces": ["ESC-SVC-22"], "requirements": ["R1", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet("{id:int}", GetOrderAsync);

        return api;
    }

    public static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(int id, [AsParameters] OrderServices services)
    {
        var order = await services.Queries.GetOrderAsync(id);
        return TypedResults.Ok(order);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order", "outcome": "contradicted", "expected": "{orderId}", "actual": "{id}",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "get-order", "messageContains": "{id}" }
  ]
}
```

## contradicted-optional-parameter

The endpoint's pattern is `{orderId:int?}`, an optional parameter, and the model's `{orderId}` is required. The constraint is dropped, the `?` is kept, and an optional parameter must agree with the model's. Traces ESC-SVC-22 (`?` kept).

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.route"], "traces": ["ESC-SVC-22"], "requirements": ["R1", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet("{orderId:int?}", GetOrderAsync);

        return api;
    }

    public static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(int? orderId, [AsParameters] OrderServices services)
    {
        var order = await services.Queries.GetOrderAsync(orderId ?? 0);
        return TypedResults.Ok(order);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order", "outcome": "contradicted", "expected": "{orderId}", "actual": "{orderId?}",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "get-order", "messageContains": "{orderId?}" }
  ]
}
```

## contradicted-not-stated

A controller action with a bare `[HttpGet]` states no route template of its own; ASP.NET Core would bind `orderId` from the query string. The model's Route `{orderId}` is contradicted with the code side "not stated": the code saying nothing never verifies a `whenSet` route (decision 4).

```json case
{ "phase": 4, "base": "verified", "without": ["src/Ordering.API/Apis/OrdersApi.cs"], "kinds": ["operation.route"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
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
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderQueries queries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Order>> GetOrderAsync(int orderId)
    {
        return await queries.GetOrderAsync(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order", "outcome": "contradicted", "expected": "{orderId}", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "get-order", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The Operation's `Http Settings` has a Verb and no Route, and the endpoint's pattern is `{orderId:int}`. No route claim exists, and nothing is reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.route"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "get-order", "type": "Operation", "name": "GetOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET" } },
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

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order", "absent": true }
  ],
  "findings": [
    { "rule": "claim/operation.route", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## unreadable

The pattern is a constant from another class. The rule can't read it as a route, so the claim is can't verify, `unreadable`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.route"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet(OrderRoutes.ById, GetOrderAsync);

        return api;
    }

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
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-order", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "get-order", "messageContains": "can't read" }
  ]
}
```

## java-verified

The same claim over Spring MVC. The class-level `@RequestMapping("/api/orders")` is the Service's route and the method's `@GetMapping("/{orderId}")` is the Operation's, so together they state the full route `api/orders/{orderId}` and each part is compared with the model property that states it. Traces JAV-SVC-06 (the route part).

```json case
{ "phase": 4, "kinds": ["operation.route", "service.route"], "traces": ["JAV-SVC-06"], "requirements": ["R2", "R10", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "get-order-by-id", "type": "Operation", "name": "GetOrderById",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{orderId}" } },
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

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
@RequestMapping("/api/orders")
public class OrdersController {
    private final OrderService orders;

    public OrdersController(OrderService orders) {
        this.orders = orders;
    }

    @GetMapping("/{orderId}")
    public OrderDto getOrderById(@PathVariable UUID orderId) {
        return orders.getById(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-controller", "outcome": "verified", "expected": "api/orders", "actual": "api/orders",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:11" },
    { "kind": "operation.route", "element": "get-order-by-id", "outcome": "verified", "expected": "{orderId}", "actual": "{orderId}",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:19" }
  ]
}
```
