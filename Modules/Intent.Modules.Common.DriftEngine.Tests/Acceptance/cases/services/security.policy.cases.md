# security.policy

The `Secured` stereotype's Policy on an Operation (or inherited from its Service), against the authorization policy the endpoint requires: `RequireAuthorization("<policy>")`, `new AuthorizeAttribute { Policy = ... }` or `[Authorize(Policy = ...)]`. `security.policy` is `stated: whenSet` (decision 4) and compared as a set.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and the Java spike's `OrdersController`.

## verified

The model's Policy is `CanCancelOrders` and the endpoint requires it by name. Traces ESC-SVC-15 (its policies).

```json case
{ "phase": 4, "kinds": ["security.policy", "security"], "traces": ["ESC-SVC-15"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" }, "Secured": { "Policy": "CanCancelOrders" } } }
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

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization("CanCancelOrders");

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security.policy", "element": "cancel-order", "outcome": "verified", "expected": "CanCancelOrders", "actual": "CanCancelOrders",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" },
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ]
}
```

## verified-authorize-attribute

The same policy, given as `new AuthorizeAttribute { Policy = ... }`. Traces ESC-SVC-15.

```json case
{ "phase": 4, "base": "verified", "kinds": ["security.policy"], "traces": ["ESC-SVC-15"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization(new AuthorizeAttribute { Policy = "CanCancelOrders" });

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security.policy", "element": "cancel-order", "outcome": "verified", "expected": "CanCancelOrders", "actual": "CanCancelOrders",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ]
}
```

## contradicted

The endpoint requires another policy.

```json case
{ "phase": 4, "base": "verified", "kinds": ["security.policy"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization("CanManageOrders");

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security.policy", "element": "cancel-order", "outcome": "contradicted", "expected": "CanCancelOrders", "actual": "CanManageOrders",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/security.policy", "severity": "error", "subject": "cancel-order", "messageContains": "CanManageOrders" }
  ]
}
```

## unreadable

The policy name is a constant from another class, which the rule can't read. The claim is can't verify, `unreadable`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["security.policy"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization(OrderPolicies.CancelOrders);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security.policy", "element": "cancel-order", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/security.policy", "severity": "error", "subject": "cancel-order", "messageContains": "can't read" }
  ]
}
```

## java-policy-not-stated

Spring Security has no named policies: `@PreAuthorize` states roles and expressions, never a policy name. The Spring MVC pack finds the endpoint and no policy, so a modelled Policy is contradicted with the code side "not stated" (decision 4), as the Java spike's check made every modelled Policy wrong. The endpoint's security itself verifies. Traces JAV-SVC-14.

```json case
{ "phase": 4, "kinds": ["security.policy", "security"], "traces": ["JAV-SVC-14"], "requirements": ["R4", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
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
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel" }, "Secured": { "Policy": "CanCancelOrders" } },
                "children": [
                  { "id": "cancel-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
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
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderService orders;

    public OrdersController(OrderService orders) {
        this.orders = orders;
    }

    @PostMapping("/api/orders/{orderId}/cancel")
    @PreAuthorize("hasRole('Admin')")
    public ResponseEntity<Void> cancelOrder(@PathVariable UUID orderId) {
        orders.cancel(orderId);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "security.policy", "element": "cancel-order", "outcome": "contradicted", "expected": "CanCancelOrders", "actual": null, "fromSilence": false },
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:20" }
  ],
  "findings": [
    { "rule": "claim/security.policy", "severity": "error", "subject": "cancel-order", "messageContains": "doesn't state" }
  ]
}
```
