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

## web-api-2-conventional-route

Web API 2's attribute routing covers only actions with a `[Route]`, so its controller's `[RoutePrefix]` doesn't route `Submit`, which has none: Web API 2 serves it on the first conventional route `MapHttpRoute` maps with a `{controller}`, `api/{controller}/{id}`. Its `{controller}` is `Policies`, and `id` is optional in the route's defaults and `Submit` doesn't take one, so it is `api/Policies`, outside the controller's route. The Operation's Route says so with an absolute route, `~/api/Policies`, and `Get`, which has a `[Route]`, is under the prefix.

```json case
{ "phase": 4, "kinds": ["operation.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Policies.Api",
        "elements": [
          { "id": "policy-request", "type": "DTO", "name": "PolicyRequest" },
          { "id": "policies", "type": "Service", "name": "PoliciesController",
            "stereotypes": { "Http Service Settings": { "Route": "api/microservices/policies" } },
            "children": [
              { "id": "submit", "type": "Operation", "name": "Submit",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/api/Policies" } },
                "children": [{ "id": "submit-request", "type": "Parameter", "name": "request", "typeRef": { "element": "policy-request" } }] },
              { "id": "get", "type": "Operation", "name": "Get",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{reference}" } },
                "children": [{ "id": "get-reference", "type": "Parameter", "name": "reference", "typeRef": { "type": "string" } }] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Policies", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Policies.Api/Startup.cs
namespace Policies.Api;

public class Startup
{
    public void Configuration(HttpConfiguration config)
    {
        config.MapHttpAttributeRoutes();
        config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
    }
}
```

```csharp file src/Policies.Api/Controllers/PoliciesController.cs
namespace Policies.Api.Controllers;

public class PolicyRequest
{
    public string Reference { get; set; }
}

[RoutePrefix("api/microservices/policies")]
public class PoliciesController : ApiController
{
    [HttpPost]
    public IHttpActionResult Submit([FromBody] PolicyRequest request) => Ok();

    [HttpGet]
    [Route("{reference}")]
    public IHttpActionResult Get(string reference) => Ok();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "submit", "outcome": "verified", "expected": "~/api/Policies", "actual": "~/api/Policies" },
    { "kind": "operation.route", "element": "get", "outcome": "verified", "expected": "{reference}", "actual": "{reference}" }
  ],
  "findings": [
    { "rule": "aspnetcore/conventional.route", "count": 0 }
  ]
}
```

## web-api-2-conventional-route-not-modelled

The same code, with `Submit`'s Route left empty, so the diagram puts it at its service's Route, `api/microservices/policies`, where Web API 2 doesn't serve it. An empty Route makes no `operation.route` claim, so the `aspnetcore/conventional.route` rule reports it. `ClaimsController` has no `[RoutePrefix]` and its service's Route is the conventional route Web API 2 serves its `Post` on, `api/Claims`, so it isn't reported.

```json case
{ "phase": 4, "base": "web-api-2-conventional-route", "kinds": ["operation.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Policies.Api",
        "elements": [
          { "id": "policy-request", "type": "DTO", "name": "PolicyRequest" },
          { "id": "policies", "type": "Service", "name": "PoliciesController",
            "stereotypes": { "Http Service Settings": { "Route": "api/microservices/policies" } },
            "children": [
              { "id": "submit", "type": "Operation", "name": "Submit",
                "stereotypes": { "Http Settings": { "Verb": "POST" } },
                "children": [{ "id": "submit-request", "type": "Parameter", "name": "request", "typeRef": { "element": "policy-request" } }] }
            ] },
          { "id": "claims", "type": "Service", "name": "ClaimsController",
            "stereotypes": { "Http Service Settings": { "Route": "api/Claims" } },
            "children": [
              { "id": "post-claim", "type": "Operation", "name": "Post",
                "stereotypes": { "Http Settings": { "Verb": "POST" } },
                "children": [{ "id": "post-claim-request", "type": "Parameter", "name": "request", "typeRef": { "element": "policy-request" } }] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Policies", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Policies.Api/Controllers/ClaimsController.cs
namespace Policies.Api.Controllers;

public class ClaimsController : ApiController
{
    public IHttpActionResult Post([FromBody] PolicyRequest request) => Ok();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "submit", "absent": true }
  ],
  "findings": [
    { "rule": "aspnetcore/conventional.route", "severity": "error", "subject": "src/Policies.Api/Controllers/PoliciesController.cs:11", "messageContains": "`~/api/Policies`" },
    { "rule": "aspnetcore/conventional.route", "subject": "src/Policies.Api/Controllers/ClaimsController.cs:5", "count": 0 }
  ]
}
```

## aspnetcore-mvc-conventional-route

An ASP.NET Core MVC controller with no `[Route]` serves its actions that have none of their own on the conventional routes the application maps: `MapDefaultControllerRoute()` maps `{controller=Home}/{action=Index}/{id?}`. `AccountController.Login` is `~/Account/Login`, absolute as it isn't under a controller's route, and `Details(id)` takes the optional `id`, so it is `~/Account/Details/{id?}`. `Logout` has its own template, `[HttpPost("signout")]`, which isn't conventional, and `ProfileController` has a `[Route]`, which routes every action it has: `Show` is under it. The adapted code is eShop's `Identity.API` Quickstart controllers.

```json case
{ "phase": 4, "kinds": ["operation.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "login", "type": "Operation", "name": "Login",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Login" } },
                "children": [{ "id": "login-return-url", "type": "Parameter", "name": "returnUrl", "typeRef": { "type": "string" } }] },
              { "id": "details", "type": "Operation", "name": "Details",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Details/{id?}" } },
                "children": [{ "id": "details-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }] },
              { "id": "logout", "type": "Operation", "name": "Logout",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "signout" } } }
            ] },
          { "id": "profile", "type": "Service", "name": "ProfileController",
            "stereotypes": { "Http Service Settings": { "Route": "profile" } },
            "children": [
              { "id": "show", "type": "Operation", "name": "Show",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "show" } } }
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

```csharp file src/Identity.API/Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.MapDefaultControllerRoute();
app.Run();
```

```csharp file src/Identity.API/Quickstart/AccountController.cs
namespace Identity.API.Quickstart;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string returnUrl) => View();

    [HttpGet]
    public IActionResult Details(int id) => View();

    [HttpPost("signout")]
    public IActionResult Logout() => View();
}
```

```csharp file src/Identity.API/Quickstart/ProfileController.cs
namespace Identity.API.Quickstart;

[Route("profile")]
public class ProfileController : Controller
{
    [HttpGet("show")]
    public IActionResult Show() => View();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "login", "outcome": "verified", "expected": "~/Account/Login", "actual": "~/Account/Login",
      "at": "src/Identity.API/Quickstart/AccountController.cs:5" },
    { "kind": "operation.route", "element": "details", "outcome": "verified", "actual": "~/Account/Details/{id?}" },
    { "kind": "operation.route", "element": "logout", "outcome": "verified", "actual": "signout" },
    { "kind": "operation.route", "element": "show", "outcome": "verified", "actual": "show" }
  ],
  "findings": [
    { "rule": "aspnetcore/conventional.route", "count": 0 }
  ]
}
```

## aspnetcore-mvc-map-controller-route

The same controller under a pattern `MapControllerRoute` maps, named by argument: `portal/{controller=Home}/{action=Index}/{id?}`. `Login` is `~/portal/Account/Login`; the model's `~/Account/Login`, the default route's, is contradicted. `Details` has no Route, so the diagram puts it at its service's Route, `account`, where ASP.NET Core doesn't serve it, and the `aspnetcore/conventional.route` rule reports it.

```json case
{ "phase": 4, "base": "aspnetcore-mvc-conventional-route", "kinds": ["operation.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Identity.API",
        "elements": [
          { "id": "account", "type": "Service", "name": "AccountController",
            "stereotypes": { "Http Service Settings": { "Route": "account" } },
            "children": [
              { "id": "login", "type": "Operation", "name": "Login",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Account/Login" } },
                "children": [{ "id": "login-return-url", "type": "Parameter", "name": "returnUrl", "typeRef": { "type": "string" } }] },
              { "id": "details", "type": "Operation", "name": "Details",
                "stereotypes": { "Http Settings": { "Verb": "GET" } },
                "children": [{ "id": "details-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }] }
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

```csharp file src/Identity.API/Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "portal/{controller=Home}/{action=Index}/{id?}");
app.Run();
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "login", "outcome": "contradicted", "expected": "~/Account/Login", "actual": "~/portal/Account/Login" },
    { "kind": "operation.route", "element": "details", "absent": true }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "login", "messageContains": "~/portal/Account/Login" },
    { "rule": "aspnetcore/conventional.route", "severity": "error", "subject": "src/Identity.API/Quickstart/AccountController.cs:8", "messageContains": "`~/portal/Account/Details/{id?}`" }
  ]
}
```

## odata-conventional-route

OData routes an `ODataController` by convention: under the prefix its route is mapped with, `MapODataServiceRoute("requestpacks", "odata", model)`, at the entity set the EDM model names after the controller, `EntitySet<RequestPack>("RequestPacks")` for `RequestPacksController`, and with the key in brackets for an action that takes one. A migrated Web API 2 application's model states `GetRequestPacks` at `odata/RequestPacks`, under a controller with no route of its own, where it is the absolute route too; `Get(key)` is stated absolute.

```json case
{ "phase": 4, "kinds": ["operation.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Packs.Api",
        "elements": [
          { "id": "request-pack", "type": "DTO", "name": "RequestPack" },
          { "id": "request-packs", "type": "Service", "name": "RequestPacksController",
            "children": [
              { "id": "get-all", "type": "Operation", "name": "GetRequestPacks", "typeRef": { "element": "request-pack", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "odata/RequestPacks" } } },
              { "id": "get-one", "type": "Operation", "name": "Get", "typeRef": { "element": "request-pack" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/odata/RequestPacks({key})" } },
                "children": [{ "id": "get-one-key", "type": "Parameter", "name": "key", "typeRef": { "type": "int" } }] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Packs", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Packs.Api/Startup.cs
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;

namespace Packs.Api;

public class Startup
{
    public void Configuration(HttpConfiguration config)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<RequestPack>("RequestPacks");
        config.MapODataServiceRoute("requestpacks", "odata", modelBuilder.GetEdmModel());
    }
}
```

```csharp file src/Packs.Api/Controllers/RequestPacksController.cs
using Microsoft.AspNet.OData;

namespace Packs.Api.Controllers;

public class RequestPack
{
    public int Id { get; set; }
}

public class RequestPacksController : ODataController
{
    [EnableQuery]
    public IQueryable<RequestPack> GetRequestPacks() => Enumerable.Empty<RequestPack>().AsQueryable();

    [EnableQuery]
    public SingleResult<RequestPack> Get([FromODataUri] int key) => SingleResult.Create(Enumerable.Empty<RequestPack>().AsQueryable());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-all", "outcome": "verified", "expected": "odata/RequestPacks", "actual": "odata/RequestPacks" },
    { "kind": "operation.route", "element": "get-one", "outcome": "verified", "expected": "~/odata/RequestPacks({key})", "actual": "~/odata/RequestPacks({key})" }
  ],
  "findings": [
    { "rule": "claim/operation.route", "count": 0 }
  ]
}
```

## odata-controller-no-entity-set-names

The EDM model names the entity set `Packs`, so OData routes no `RequestPacksController`, and the code doesn't state the route the model gives.

```json case
{ "phase": 4, "base": "odata-conventional-route", "kinds": ["operation.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Packs.Api/Startup.cs
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;

namespace Packs.Api;

public class Startup
{
    public void Configuration(HttpConfiguration config)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<RequestPack>("Packs");
        config.MapODataServiceRoute("requestpacks", "odata", modelBuilder.GetEdmModel());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "get-all", "outcome": "contradicted", "expected": "odata/RequestPacks" }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "get-all" }
  ]
}
```
