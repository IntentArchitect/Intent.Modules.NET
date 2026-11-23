# operation.verb

An Operation's `Http Settings` Verb, against the HTTP method its endpoint is mapped with: `MapGet`, `MapPut` and the rest, `MapMethods` with one verb, or a Spring `*Mapping` annotation. `operation.verb` is `stated: always`: every mapped endpoint states its verb.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and the Java spike's `OrdersController`.

## verified

The Operation's Verb is `PUT` and the endpoint is mapped with `MapPut`. Traces ESC-SVC-21.

```json case
{ "phase": 4, "kinds": ["operation.verb"], "traces": ["ESC-SVC-21"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
    { "kind": "operation.verb", "element": "cancel-order", "outcome": "verified", "expected": "PUT", "actual": "PUT",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ]
}
```

## contradicted

The endpoint is mapped with `MapPost`. Traces ESC-SVC-21.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.verb"], "traces": ["ESC-SVC-21"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/cancel", CancelOrderAsync);

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
    { "kind": "operation.verb", "element": "cancel-order", "outcome": "contradicted", "expected": "PUT", "actual": "POST",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.verb", "severity": "error", "subject": "cancel-order", "messageContains": "POST" }
  ]
}
```

## map-methods-with-one-verb

`MapMethods` with a single verb states that verb as plainly as `MapPut` does. Traces ESC-SVC-21.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.verb"], "traces": ["ESC-SVC-21"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapMethods("/cancel", ["PUT"], CancelOrderAsync);

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
    { "kind": "operation.verb", "element": "cancel-order", "outcome": "verified", "expected": "PUT", "actual": "PUT",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ]
}
```

## map-methods-with-several-verbs-unreadable

`MapMethods` with two verbs states no single verb an Operation can match. eShopCustom listed a multi-verb mapping as code it couldn't model; here the claim is can't verify, `unreadable`, so the reviewer sees the endpoint wasn't checked.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.verb"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapMethods("/cancel", ["PUT", "PATCH"], CancelOrderAsync);

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
    { "kind": "operation.verb", "element": "cancel-order", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.verb", "severity": "error", "subject": "cancel-order", "messageContains": "can't read" }
  ]
}
```

## blocked-when-endpoint-missing

No endpoint is mapped for `CancelOrderAsync`: the handler method is there but nothing maps it. `endpoint.exists` is contradicted, and the verb, which depends on it, is can't verify, `blocked`, with no finding of its own.

```json case
{ "phase": 4, "base": "verified", "kinds": ["operation.verb", "endpoint.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

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
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "contradicted", "expected": "present", "actual": "absent" },
    { "kind": "operation.verb", "element": "cancel-order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "endpoint.exists" }
  ],
  "findings": [
    { "rule": "claim/endpoint.exists", "severity": "error", "subject": "cancel-order", "messageContains": "isn't in the code" },
    { "rule": "claim/operation.verb", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## web-api-2-name-convention

A Web API 2 action with no verb attribute takes its verb from its name: `GetCustomerByEmail` is `GET`, `DeleteCustomer` is `DELETE`, and an action whose name starts with no verb, `Archive`, is `POST`. The model says so for the first two and `PUT` for the third, which Web API 2 doesn't answer.

```json case
{ "phase": 4, "kinds": ["operation.verb"], "requirements": ["R1", "R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Customers.Api",
        "elements": [
          { "id": "customers", "type": "Service", "name": "CustomersController",
            "stereotypes": { "Http Service Settings": { "Route": "api/customers" } },
            "children": [
              { "id": "get-by-email", "type": "Operation", "name": "GetCustomerByEmail",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "by-email" } },
                "children": [{ "id": "email", "type": "Parameter", "name": "email", "typeRef": { "type": "string" } }] },
              { "id": "delete", "type": "Operation", "name": "DeleteCustomer",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "{id}" } },
                "children": [{ "id": "delete-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }] },
              { "id": "archive", "type": "Operation", "name": "Archive",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "{id}/archive" } },
                "children": [{ "id": "archive-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Customers", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Customers.Api/Controllers/CustomersController.cs
namespace Customers.Api.Controllers;

[RoutePrefix("api/customers")]
public class CustomersController : ApiController
{
    [Route("by-email")]
    public IHttpActionResult GetCustomerByEmail(string email) => Ok();

    [Route("{id}")]
    public IHttpActionResult DeleteCustomer(int id) => Ok();

    [Route("{id}/archive")]
    public IHttpActionResult Archive(int id) => Ok();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.verb", "element": "get-by-email", "outcome": "verified", "actual": "GET", "at": "src/Customers.Api/Controllers/CustomersController.cs:6" },
    { "kind": "operation.verb", "element": "delete", "outcome": "verified", "actual": "DELETE" },
    { "kind": "operation.verb", "element": "archive", "outcome": "contradicted", "expected": "PUT", "actual": "POST" }
  ],
  "findings": [
    { "rule": "claim/operation.verb", "severity": "error", "subject": "archive", "messageContains": "POST" }
  ]
}
```

## aspnetcore-action-serves-every-verb

An ASP.NET Core action with no verb attribute serves every verb, unlike Web API 2's, which takes one from its name. The designer needs one Verb, and the action serves it, so `HomeController.Index` modelled `GET` and `Error` modelled `POST` are both verified, as a convention the code doesn't state. `Privacy` has `[HttpGet]`, so `POST` is contradicted. The adapted code is eShop's `Identity.API` Quickstart `HomeController`.

```json case
{ "phase": 4, "kinds": ["operation.verb"], "requirements": ["R1", "R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Identity.API",
        "elements": [
          { "id": "home", "type": "Service", "name": "HomeController",
            "children": [
              { "id": "index", "type": "Operation", "name": "Index",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "~/Home/Index" } } },
              { "id": "error", "type": "Operation", "name": "Error",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Home/Error" } },
                "children": [{ "id": "error-id", "type": "Parameter", "name": "errorId", "typeRef": { "type": "string" } }] },
              { "id": "privacy", "type": "Operation", "name": "Privacy",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Home/Privacy" } } }
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
var app = WebApplication.CreateBuilder(args).Build();
app.MapDefaultControllerRoute();
app.Run();
```

```csharp file src/Identity.API/Quickstart/HomeController.cs
namespace Identity.API.Quickstart;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public Task<IActionResult> Error(string errorId) => Task.FromResult<IActionResult>(View());

    [HttpGet]
    public IActionResult Privacy() => View();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.verb", "element": "index", "outcome": "verified", "expected": "GET", "actual": "GET", "at": "src/Identity.API/Quickstart/HomeController.cs:5" },
    { "kind": "operation.verb", "element": "error", "outcome": "verified", "actual": "POST" },
    { "kind": "operation.verb", "element": "privacy", "outcome": "contradicted", "expected": "POST", "actual": "GET" }
  ],
  "findings": [
    { "rule": "claim/operation.verb", "severity": "error", "subject": "privacy", "messageContains": "GET" },
    { "rule": "claim/operation.verb", "subject": "index", "count": 0 }
  ]
}
```

## java-verified

The same claim over Spring MVC: `@PostMapping` states `POST`. Traces JAV-SVC-06 (the verb part).

```json case
{ "phase": 4, "kinds": ["operation.verb"], "traces": ["JAV-SVC-06"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
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
    { "kind": "operation.verb", "element": "cancel-order", "outcome": "verified", "expected": "POST", "actual": "POST",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:18" }
  ]
}
```
