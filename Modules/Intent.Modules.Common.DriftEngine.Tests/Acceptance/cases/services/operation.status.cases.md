# operation.status

An Operation's `Http Settings` Success Response Code, against the success status its endpoint returns: `TypedResults.Created` is 201, `TypedResults.Ok` 200, Spring's `ResponseEntity.noContent()` 204, and so on. `operation.status` is `stated: whenSet` and compared as a number: the model's `201 (Created)` is `201`. A handler that returns a plain value states no status, so it never verifies a modelled one (decision 4).

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and the Java spike's `OrdersController`.

## verified

The Operation's Success Response Code is `201 (Created)` and the handler returns `TypedResults.Created`. Traces ESC-SVC-24.

```json case
{ "phase": 4, "kinds": ["operation.status"], "traces": ["ESC-SVC-24"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "create-order-draft", "type": "Operation", "name": "CreateOrderDraftAsync",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "draft", "Success Response Code": "201 (Created)" } } }
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

        api.MapPost("/draft", CreateOrderDraftAsync);

        return api;
    }

    public static async Task<Created<OrderDraftDTO>> CreateOrderDraftAsync(CreateOrderDraftCommand command, [AsParameters] OrderServices services)
    {
        var draft = await services.Mediator.Send(command);

        return TypedResults.Created($"/api/orders/draft/{command.BuyerId}", draft);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "create-order-draft", "outcome": "verified", "expected": "201", "actual": "201",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:18" }
  ]
}
```

## contradicted

The handler returns `TypedResults.Ok`, a 200. Traces ESC-SVC-24.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.status"], "traces": ["ESC-SVC-24"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/draft", CreateOrderDraftAsync);

        return api;
    }

    public static async Task<Ok<OrderDraftDTO>> CreateOrderDraftAsync(CreateOrderDraftCommand command, [AsParameters] OrderServices services)
    {
        var draft = await services.Mediator.Send(command);

        return TypedResults.Ok(draft);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "create-order-draft", "outcome": "contradicted", "expected": "201", "actual": "200",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:18" }
  ],
  "findings": [
    { "rule": "claim/operation.status", "severity": "error", "subject": "create-order-draft", "messageContains": "200" }
  ]
}
```

## contradicted-not-stated

The handler returns the draft itself, as eShopCustom's `CreateOrderDraftAsync` does. ASP.NET Core sends a 200 for it, but the code states no status, and a convention never verifies a `whenSet` value: the modelled `201` is contradicted with the code side "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.status"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/draft", CreateOrderDraftAsync);

        return api;
    }

    public static async Task<OrderDraftDTO> CreateOrderDraftAsync(CreateOrderDraftCommand command, [AsParameters] OrderServices services)
    {
        return await services.Mediator.Send(command);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "create-order-draft", "outcome": "contradicted", "expected": "201", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/operation.status", "severity": "error", "subject": "create-order-draft", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-when-unset

The Operation has no Success Response Code, and the handler returns `TypedResults.Created`. No status claim exists, and nothing is reported (principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.status"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "create-order-draft", "type": "Operation", "name": "CreateOrderDraftAsync",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "draft" } } }
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
    { "kind": "operation.status", "element": "create-order-draft", "absent": true }
  ],
  "findings": [
    { "rule": "claim/operation.status", "count": 0 }
  ]
}
```

## unreadable

The handler returns `Results.StatusCode` with a variable: it states a status, in a form the rule can't read. The claim is can't verify, `unreadable`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.status"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/draft", CreateOrderDraftAsync);

        return api;
    }

    public static async Task<IResult> CreateOrderDraftAsync(CreateOrderDraftCommand command, [AsParameters] OrderServices services)
    {
        var draft = await services.Mediator.Send(command);

        return Results.StatusCode(services.Options.DraftStatusCode);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "create-order-draft", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:18" }
  ],
  "findings": [
    { "rule": "claim/operation.status", "severity": "error", "subject": "create-order-draft", "messageContains": "can't read" }
  ]
}
```

## java-verified-no-content

Over Spring MVC, `ResponseEntity.noContent()` is a 204, and the Operation's Success Response Code is `204 (No Content)`. Traces JAV-SVC-07.

```json case
{ "phase": 4, "kinds": ["operation.status"], "traces": ["JAV-SVC-07"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
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
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel", "Success Response Code": "204 (No Content)" } },
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
    public ResponseEntity<Void> cancelOrder(@PathVariable UUID orderId) {
        orders.cancel(orderId);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "cancel-order", "outcome": "verified", "expected": "204", "actual": "204",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:21" }
  ]
}
```

## java-contradicted

The model says `201 (Created)` and the controller still returns `noContent()`. Traces JAV-SVC-07.

```json case
{ "phase": 4, "base": "java-verified-no-content", "kinds": ["operation.status"], "traces": ["JAV-SVC-07"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
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
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel", "Success Response Code": "201 (Created)" } },
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

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "cancel-order", "outcome": "contradicted", "expected": "201", "actual": "204",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:21" }
  ],
  "findings": [
    { "rule": "claim/operation.status", "severity": "error", "subject": "cancel-order", "messageContains": "204" }
  ]
}
```

## java-unmodelled-non-200-reported-when-opted-in

The Operation has no Success Response Code and the controller returns `noContent()`. The Java spike reported an unmodelled non-200 status as Backlog; here there is no claim, and with the `unmodelled.value` opt-in rule on, the rule reports the 204 on its line. Traces JAV-SVC-07.

```json case
{ "phase": 4, "base": "java-verified-no-content", "kinds": ["operation.status"], "cases": ["optin/unmodelled.value"], "traces": ["JAV-SVC-07"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
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
                  { "id": "cancel-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "cancel-order", "absent": true }
  ],
  "findings": [
    { "rule": "claim/operation.status", "count": 0 },
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/main/java/com/eshop/backend/api/OrdersController.java:21", "messageContains": "204" }
  ]
}
```
