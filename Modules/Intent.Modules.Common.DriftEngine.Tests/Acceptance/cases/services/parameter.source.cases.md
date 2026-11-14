# parameter.source

Where a Parameter is bound from: the `Parameter Settings` stereotype's Source (`From Header`, `From Route`, `From Query`, `From Body`, `From Form`). A Source of `Default`, or no stereotype, states nothing: the model leaves the binding to the framework, so there's no claim. Against the code's binding attribute: `[FromHeader]`, `[FromRoute]`, `[FromQuery]`, `[FromBody]` or `[FromForm]`; over Spring MVC, `@PathVariable`, `@RequestParam`, `@RequestBody` and `@RequestHeader`. A binding the framework infers, from the route template or the parameter's type, isn't the code stating one, so a modelled source with no attribute is contradicted as not stated (the user's decision, 2026-11-13: a parameter the code binds by inference is modelled `Default`). A parameter injected from services isn't bound from the request at all. `parameter.source` is `stated: whenSet`.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and the Java spike's `OrdersController`.

## verified-from-attribute

`requestId` is `From Header` in the model and `[FromHeader(...)]` in the code. Traces ESC-SVC-28 (the attribute).

```json case
{ "phase": 4, "kinds": ["parameter.source"], "traces": ["ESC-SVC-28"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" },
            "children": [
              { "id": "cancel-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "guid" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Header", "Header Name": "x-requestid" } } },
                  { "id": "command", "type": "Parameter", "name": "command", "typeRef": { "element": "cancel-order-command" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
                ] },
              { "id": "get-order", "type": "Operation", "name": "GetOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{orderId}" } },
                "children": [
                  { "id": "order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Route" } } }
                ] },
              { "id": "get-orders-by-user", "type": "Operation", "name": "GetOrdersByUserAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET" } },
                "children": [
                  { "id": "status", "type": "Parameter", "name": "status", "typeRef": { "type": "string", "nullable": true },
                    "stereotypes": { "Parameter Settings": { "Source": "From Query" } } }
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
        api.MapGet("{orderId:int}", GetOrderAsync);
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

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(string? status, [AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId, status));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "request-id", "outcome": "verified", "expected": "From Header", "actual": "From Header",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:17" }
  ]
}
```

## contradicted-route-template-not-stated

`orderId` has no attribute. ASP.NET binds it from the route, because the endpoint's pattern `{orderId:int}` names it, but that is inference, not the code stating a source, so the modelled `From Route` is contradicted as not stated. The fix is `[FromRoute]` in the code, or Source `Default` in the model. Traces ESC-SVC-28 (the route template).

```json case
{ "phase": 4, "base": "verified-from-attribute", "kinds": ["parameter.source"], "traces": ["ESC-SVC-28"], "requirements": ["R2", "R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "order-id", "outcome": "contradicted", "expected": "From Route", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/parameter.source", "severity": "error", "subject": "order-id", "messageContains": "doesn't state" }
  ]
}
```

## contradicted-simple-type-not-stated

`status` is a `string?` with no attribute that no route names. ASP.NET binds a simple type from the query string, but the code doesn't state it, so the modelled `From Query` is contradicted as not stated. Traces ESC-SVC-28 (simple types).

```json case
{ "phase": 4, "base": "verified-from-attribute", "kinds": ["parameter.source"], "traces": ["ESC-SVC-28"], "requirements": ["R2", "R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "status", "outcome": "contradicted", "expected": "From Query", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/parameter.source", "severity": "error", "subject": "status", "messageContains": "doesn't state" }
  ]
}
```

## contradicted-complex-type-not-stated

`command` is a `CancelOrderCommand` with no attribute. ASP.NET binds a complex type from the body, but the code doesn't state it, so the modelled `From Body` is contradicted as not stated. Traces ESC-SVC-28 (complex types).

```json case
{ "phase": 4, "base": "verified-from-attribute", "kinds": ["parameter.source"], "traces": ["ESC-SVC-28"], "requirements": ["R2", "R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "command", "outcome": "contradicted", "expected": "From Body", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/parameter.source", "severity": "error", "subject": "command", "messageContains": "doesn't state" }
  ]
}
```

## default-not-claimed

`orderId` has no `Parameter Settings`: its Source is `Default`, so the model leaves the binding to the framework and states no source. There is no claim, whatever the code binds it from (the user's decision, 2026-11-13: a parameter the code binds by inference is modelled `Default`, and only a stated Source is checked). Traces ESC-SVC-28 (the model's Default).

```json case
{ "phase": 4, "base": "verified-from-attribute", "kinds": ["parameter.source"], "traces": ["ESC-SVC-28"], "requirements": ["R2", "R30"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                  { "id": "order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
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
    { "kind": "parameter.source", "element": "order-id", "absent": true }
  ]
}
```

## contradicted

The model binds `requestId` from the query string, and the code binds it from a header.

```json case
{ "phase": 4, "base": "verified-from-attribute", "kinds": ["parameter.source"], "traces": ["ESC-SVC-28"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" },
            "children": [
              { "id": "cancel-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "guid" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Query" } } },
                  { "id": "command", "type": "Parameter", "name": "command", "typeRef": { "element": "cancel-order-command" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
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
    { "kind": "parameter.source", "element": "request-id", "outcome": "contradicted", "expected": "From Query", "actual": "From Header",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:17" }
  ],
  "findings": [
    { "rule": "claim/parameter.source", "severity": "error", "subject": "request-id", "messageContains": "From Header" }
  ]
}
```

## contradicted-injected

The model has a request Parameter `identityService` bound from the body, but the code takes it with `[FromServices]`: it is injected from the container, not bound from the request, so the model's Parameter is wrong. Traces ESC-SVC-27.

```json case
{ "phase": 4, "base": "verified-from-attribute", "kinds": ["parameter.source"], "traces": ["ESC-SVC-27"], "requirements": ["R1"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "identity-service", "type": "Service", "name": "IdentityService",
            "children": [
              { "id": "get-user-identity", "type": "Operation", "name": "GetUserIdentity", "typeRef": { "type": "string" } }
            ] },
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "get-orders-by-user", "type": "Operation", "name": "GetOrdersByUserAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET" } },
                "children": [
                  { "id": "identity-service-parameter", "type": "Parameter", "name": "identityService", "typeRef": { "element": "identity-service" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
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

        api.MapGet("/", GetOrdersByUserAsync);

        return api;
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(
        [FromServices] IIdentityService identityService,
        [AsParameters] OrderServices services)
    {
        var userId = identityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "identity-service-parameter", "outcome": "contradicted", "expected": "From Body", "actual": "From Services",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:15" }
  ],
  "findings": [
    { "rule": "claim/parameter.source", "severity": "error", "subject": "identity-service-parameter", "messageContains": "From Services" }
  ]
}
```

## blocked-when-endpoint-missing

Nothing maps `CancelOrderAsync`. Its endpoint is contradicted, and its parameters' bindings, which depend on it, are can't verify, `blocked`.

```json case
{ "phase": 4, "base": "verified-from-attribute", "kinds": ["parameter.source", "endpoint.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet("{orderId:int}", GetOrderAsync);
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

    public static async Task<Ok<Order>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        return TypedResults.Ok(await services.Queries.GetOrderAsync(orderId));
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync(string? status, [AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        return TypedResults.Ok(await services.Queries.GetOrdersFromUserAsync(userId, status));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "contradicted", "expected": "present", "actual": "absent" },
    { "kind": "parameter.source", "element": "request-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "endpoint.exists" },
    { "kind": "parameter.source", "element": "command", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "endpoint.exists" }
  ],
  "findings": [
    { "rule": "claim/endpoint.exists", "severity": "error", "subject": "cancel-order", "messageContains": "isn't in the code" },
    { "rule": "claim/parameter.source", "count": 0, "note": "Blocked claims have no finding of their own" }
  ]
}
```

## java-sources

Over Spring MVC: `@PathVariable`, `@RequestHeader`, `@RequestBody` and `@RequestParam` are `From Route`, `From Header`, `From Body` and `From Query`. Traces JAV-SVC-09.

```json case
{ "phase": 4, "kinds": ["parameter.source"], "traces": ["JAV-SVC-09"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "cancel-order-input", "type": "DTO", "name": "CancelOrderInput",
            "children": [
              { "id": "cancel-order-input-reason", "type": "DTO-Field", "name": "Reason", "typeRef": { "type": "string" } }
            ] },
          { "id": "order-summary-dto", "type": "DTO", "name": "OrderSummaryDto",
            "children": [
              { "id": "order-summary-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] },
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel" } },
                "children": [
                  { "id": "order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Route" } } },
                  { "id": "request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "string" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Header", "Header Name": "x-requestid" } } },
                  { "id": "body", "type": "Parameter", "name": "body", "typeRef": { "element": "cancel-order-input" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
                ] },
              { "id": "get-orders", "type": "Operation", "name": "GetOrders", "typeRef": { "element": "order-summary-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "/api/orders" } },
                "children": [
                  { "id": "status", "type": "Parameter", "name": "status", "typeRef": { "type": "string", "nullable": true },
                    "stereotypes": { "Parameter Settings": { "Source": "From Query" } } }
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
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;
import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderService orders;

    public OrdersController(OrderService orders) {
        this.orders = orders;
    }

    @PostMapping("/api/orders/{orderId}/cancel")
    public ResponseEntity<Void> cancelOrder(
            @PathVariable UUID orderId,
            @RequestHeader("x-requestid") String requestId,
            @RequestBody CancelOrderInput body) {
        orders.cancel(orderId, body.reason());
        return ResponseEntity.noContent().build();
    }

    @GetMapping("/api/orders")
    public List<OrderSummaryDto> getOrders(@RequestParam(required = false) String status) {
        return orders.list(status);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "order-id", "outcome": "verified", "expected": "From Route", "actual": "From Route",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:25" },
    { "kind": "parameter.source", "element": "request-id", "outcome": "verified", "expected": "From Header", "actual": "From Header",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:26" },
    { "kind": "parameter.source", "element": "body", "outcome": "verified", "expected": "From Body", "actual": "From Body",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:27" },
    { "kind": "parameter.source", "element": "status", "outcome": "verified", "expected": "From Query", "actual": "From Query",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:33" }
  ]
}
```

## java-default-not-claimed

The same controller, with `cancelOrder`'s `orderId` and `body` Parameters given no `Parameter Settings`, so their Source is `Default`. The model states no source for either, so neither has a claim, though the code's `@PathVariable` and `@RequestBody` state one (the user's decision, 2026-11-13: only a stated Source is checked; a code-only binding is the opt-in "must be modelled" rule). Traces JAV-SVC-09 (the model's Default).

```json case
{ "phase": 4, "base": "java-sources", "kinds": ["parameter.source"], "traces": ["JAV-SVC-09"], "requirements": ["R2", "R30", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "cancel-order-input", "type": "DTO", "name": "CancelOrderInput",
            "children": [
              { "id": "cancel-order-input-reason", "type": "DTO-Field", "name": "Reason", "typeRef": { "type": "string" } }
            ] },
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel" } },
                "children": [
                  { "id": "order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } },
                  { "id": "request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "string" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Header", "Header Name": "x-requestid" } } },
                  { "id": "body", "type": "Parameter", "name": "body", "typeRef": { "element": "cancel-order-input" } }
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

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "order-id", "absent": true },
    { "kind": "parameter.source", "element": "body", "absent": true }
  ]
}
```
