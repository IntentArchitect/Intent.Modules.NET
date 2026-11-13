# service.route

A Service's `Http Service Settings` Route, against the route prefix its router states: the `MapGroup` prefix of a minimal API router, or a Spring controller's class-level `@RequestMapping`. `service.route` is `stated: whenSet`: the model states a prefix only where the code states one (decision 4), and the `route` comparer ignores a leading or trailing `/`, case in literal segments and parameter constraints.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and the Java spike's `OrdersController`.

## verified

The Service's Route is `api/orders` and the router's `MapGroup` says `api/orders`. Traces ESC-SVC-13.

```json case
{ "phase": 4, "kinds": ["service.route"], "traces": ["ESC-SVC-13"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
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
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-api", "outcome": "verified", "expected": "api/orders", "actual": "api/orders",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ]
}
```

## contradicted

The router's group prefix is another route. The finding names the route, the Service and the line (R11, R14). Traces ESC-SVC-13.

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.route"], "traces": ["ESC-SVC-13"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/ordering");

        api.MapPut("/cancel", CancelOrderAsync);

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
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-api", "outcome": "contradicted", "expected": "api/orders", "actual": "api/ordering",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ],
  "findings": [
    { "rule": "claim/service.route", "severity": "error", "subject": "orders-api", "messageContains": "api/ordering" }
  ]
}
```

## contradicted-not-stated

A builder router: `MapOrdersApiV1` extends a `RouteGroupBuilder` its caller made, and maps its endpoints straight onto it, so the router states no prefix of its own. eShopCustom's check said a builder router has no Route; here the model gives it one, and the code doesn't state it, so the claim is contradicted with the code side "not stated" (decision 4). Traces ESC-SVC-13.

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.route"], "traces": ["ESC-SVC-13"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this RouteGroupBuilder api)
    {
        api.MapPut("/cancel", CancelOrderAsync);

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
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-api", "outcome": "contradicted", "expected": "api/orders", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/service.route", "severity": "error", "subject": "orders-api", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The Service has `Http Service Settings` with no Route, and the router's `MapGroup` says `api/orders`. An unset `whenSet` value is no claim, so nothing is checked and nothing is reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.route"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": {} },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
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
    { "kind": "service.route", "element": "orders-api", "absent": true }
  ],
  "findings": [
    { "rule": "claim/service.route", "count": 0 },
    { "rule": "optin/unmodelled.value", "count": 0, "note": "The rule is off" }
  ]
}
```

## unmodelled-prefix-reported-when-opted-in

The same model and code, with the `unmodelled.value` opt-in rule turned on. eShopCustom reported a prefix the model doesn't state as Backlog; here it is the opt-in rule's finding, on the `MapGroup` line, and still no claim. Traces ESC-SVC-13.

```json case
{ "phase": 4, "base": "not-claimed-when-unset", "kinds": ["service.route"], "cases": ["optin/unmodelled.value"], "traces": ["ESC-SVC-13"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": {} },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-api", "absent": true }
  ],
  "findings": [
    { "rule": "claim/service.route", "count": 0 },
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Ordering.API/Apis/OrdersApi.cs:7", "messageContains": "api/orders" }
  ]
}
```

## unreadable

The prefix is a constant from another class, which the rule can't read as a route. eShopCustom gave a warning for a non-literal prefix and stopped checking it; the claim is can't verify, `unreadable`, an error like every can't-verify (principle 2).

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.route"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(OrderRoutes.Base);

        api.MapPut("/cancel", CancelOrderAsync);

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
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-api", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ],
  "findings": [
    { "rule": "claim/service.route", "severity": "error", "subject": "orders-api", "messageContains": "can't read" }
  ]
}
```

## java-verified

The same claim over Spring MVC: the controller's class-level `@RequestMapping("/api/orders")` is the Service's prefix. The leading `/` is ignored by the `route` comparer, so the canonical text on both sides is `api/orders`. Traces JAV-SVC-06 (the Service route part).

```json case
{ "phase": 4, "kinds": ["service.route"], "traces": ["JAV-SVC-06"], "requirements": ["R2", "R10", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
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
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:11" }
  ]
}
```
