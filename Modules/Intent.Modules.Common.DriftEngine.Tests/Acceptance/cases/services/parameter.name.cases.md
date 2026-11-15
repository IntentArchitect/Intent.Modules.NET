# parameter.name

The name a Parameter is bound by on the wire: the `Parameter Settings` stereotype's Header Name or Query String Name, against the `Name` of the parameter's `[FromHeader(Name = ...)]` or `[FromQuery(Name = ...)]`. `parameter.name` is `stated: whenSet` (decision 4): with no Header Name or Query String Name there is no claim, and a modelled name the attribute doesn't state is contradicted, "not stated", even though ASP.NET Core would then bind by the parameter's own name.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs`.

## verified-header-name

The Header Name is `x-requestid`, and the attribute says `[FromHeader(Name = "x-requestid")]`. Traces ESC-SVC-29.

```json case
{ "phase": 4, "kinds": ["parameter.name"], "traces": ["ESC-SVC-29"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "guid" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Header", "Header Name": "x-requestid" } } }
                ] },
              { "id": "get-orders-by-user", "type": "Operation", "name": "GetOrdersByUserAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET" } },
                "children": [
                  { "id": "page-index", "type": "Parameter", "name": "pageIndex", "typeRef": { "type": "int" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Query", "Query String Name": "page" } } }
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

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("/", GetOrdersByUserAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);
        var commandResult = await services.Mediator.Send(requestCancelOrder);

        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(
        [FromQuery(Name = "page")] int pageIndex,
        [AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId, pageIndex));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.name", "element": "request-id", "outcome": "verified", "expected": "x-requestid", "actual": "x-requestid",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:16" }
  ]
}
```

## verified-query-string-name

`pageIndex` is bound from the query string as `page`: the Query String Name, and the `[FromQuery(Name = "page")]` attribute. Traces ESC-SVC-29.

```json case
{ "phase": 4, "base": "verified-header-name", "kinds": ["parameter.name"], "traces": ["ESC-SVC-29"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "parameter.name", "element": "page-index", "outcome": "verified", "expected": "page", "actual": "page",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:27" }
  ]
}
```

## contradicted

The attribute binds the header `x-request-id`. Traces ESC-SVC-29.

```json case
{ "phase": 4, "base": "verified-header-name", "kinds": ["parameter.name"], "traces": ["ESC-SVC-29"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("/", GetOrdersByUserAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        [FromHeader(Name = "x-request-id")] Guid requestId,
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);
        var commandResult = await services.Mediator.Send(requestCancelOrder);

        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(
        [FromQuery(Name = "page")] int pageIndex,
        [AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId, pageIndex));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.name", "element": "request-id", "outcome": "contradicted", "expected": "x-requestid", "actual": "x-request-id",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:16" }
  ],
  "findings": [
    { "rule": "claim/parameter.name", "severity": "error", "subject": "request-id", "messageContains": "x-request-id" }
  ]
}
```

## contradicted-not-stated

The attribute is a bare `[FromHeader]`. ASP.NET Core would read the header `requestId`, but the code states no name, so the modelled `x-requestid` is contradicted with the code side "not stated" (decision 4). Traces ESC-SVC-29.

```json case
{ "phase": 4, "base": "verified-header-name", "kinds": ["parameter.name"], "traces": ["ESC-SVC-29"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("/", GetOrdersByUserAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        [FromHeader] Guid requestId,
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);
        var commandResult = await services.Mediator.Send(requestCancelOrder);

        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(
        [FromQuery(Name = "page")] int pageIndex,
        [AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId, pageIndex));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.name", "element": "request-id", "outcome": "contradicted", "expected": "x-requestid", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/parameter.name", "severity": "error", "subject": "request-id", "messageContains": "doesn't state" }
  ]
}
```

## unreadable

The header's name is a constant from another class, which the rule can't read. The claim is can't verify, `unreadable`.

```json case
{ "phase": 4, "base": "verified-header-name", "kinds": ["parameter.name"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("/", GetOrdersByUserAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        [FromHeader(Name = RequestHeaders.RequestId)] Guid requestId,
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);
        var commandResult = await services.Mediator.Send(requestCancelOrder);

        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(
        [FromQuery(Name = "page")] int pageIndex,
        [AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId, pageIndex));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.name", "element": "request-id", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:16" }
  ],
  "findings": [
    { "rule": "claim/parameter.name", "severity": "error", "subject": "request-id", "messageContains": "can't read" }
  ]
}
```

## not-claimed-when-unset

`requestId` is `From Header` with no Header Name, and the attribute names `x-requestid`. eShopCustom compared the bound name with the Parameter's own name in that case; with decision 4 there is no claim, and the name the code states is the `unmodelled.value` opt-in rule's to report, which is off here. Traces ESC-SVC-29 (the Parameter-name fallback, now a non-claim).

```json case
{ "phase": 4, "base": "verified-header-name", "kinds": ["parameter.name"], "traces": ["ESC-SVC-29"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "guid" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Header" } } }
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
    { "kind": "parameter.name", "element": "request-id", "absent": true }
  ],
  "findings": [
    { "rule": "claim/parameter.name", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## verified-constant

The header's name is a constant the code declares, `RequestHeaders.RequestId`, whose value is `x-requestid`.

```json case
{ "phase": 4, "base": "verified-header-name", "kinds": ["parameter.name"], "traces": ["ESC-SVC-29"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class RequestHeaders
{
    public const string RequestId = "x-requestid";
}

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");
        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("/", GetOrdersByUserAsync);
        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        [FromHeader(Name = RequestHeaders.RequestId)] Guid requestId,
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);
        var commandResult = await services.Mediator.Send(requestCancelOrder);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(
        [FromQuery(Name = "page")] int pageIndex,
        [AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId, pageIndex));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.name", "element": "request-id", "outcome": "verified", "expected": "x-requestid", "actual": "x-requestid",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:19" }
  ]
}
```

## java-header-and-query

Over Spring MVC: `@RequestHeader("x-requestid")` names the header as its first argument, and `@RequestParam(name = "page")` names the query string parameter; a different name is contradicted.

```json case
{ "phase": 4, "kinds": ["parameter.name"], "requirements": ["R1", "R2", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel" } },
                "children": [
                  { "id": "request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "string" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Header", "Header Name": "x-requestid" } } }
                ] },
              { "id": "get-orders", "type": "Operation", "name": "GetOrders",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "/api/orders" } },
                "children": [
                  { "id": "page-index", "type": "Parameter", "name": "pageIndex", "typeRef": { "type": "int" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Query", "Query String Name": "pageIndex" } } }
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
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class OrdersController {
    @PostMapping("/api/orders/{orderId}/cancel")
    public void cancelOrder(@RequestHeader("x-requestid") String requestId) {
    }

    @GetMapping("/api/orders")
    public void getOrders(@RequestParam(name = "page") int pageIndex) {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.name", "element": "request-id", "outcome": "verified", "expected": "x-requestid", "actual": "x-requestid",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:12" },
    { "kind": "parameter.name", "element": "page-index", "outcome": "contradicted", "expected": "pageIndex", "actual": "page",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:16" }
  ],
  "findings": [
    { "rule": "claim/parameter.name", "severity": "error", "subject": "page-index", "messageContains": "code `page`" }
  ]
}
```
