# endpoint.exists

Whether an Operation's endpoint is mapped. An Operation with `Http Settings` states an endpoint is there; an Operation without them states it isn't. The endpoint is found by its handler method's name, by `.WithName` on a lambda, by verb and route for an unnamed lambda, or, over Spring MVC, by the method `camel(Name)` carrying a `*Mapping` annotation. `endpoint.exists` is an existence kind: a modelled endpoint the code lacks is contradicted (R8), and the claims on its route, verb, status and parameters are blocked behind it.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and `Catalog.API/Apis/CatalogApi.cs`, and the Java spike's `OrdersController`.

## verified-by-handler-name

`CancelOrderAsync` is mapped by `MapPut`, naming the handler method. Traces ESC-SVC-19.

```json case
{ "phase": 4, "kinds": ["endpoint.exists"], "traces": ["ESC-SVC-19"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ]
}
```

## verified-by-with-name

A lambda endpoint has no method name to match; `.WithName("ListItems")` names it, and the Operation is `ListItems`. Traces ESC-SVC-19.

```json case
{ "phase": 4, "base": "verified-by-handler-name", "without": ["src/Ordering.API/Apis/OrdersApi.cs"], "kinds": ["endpoint.exists"], "traces": ["ESC-SVC-19"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "catalog-api", "type": "Service", "name": "CatalogApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/catalog" } },
            "children": [
              { "id": "list-items", "type": "Operation", "name": "ListItems",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" } } },
              { "id": "get-item-by-id", "type": "Operation", "name": "GetItemById",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items/{id}" } },
                "children": [
                  { "id": "get-item-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Catalog", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", async ([AsParameters] PaginationRequest paginationRequest, [AsParameters] CatalogServices services) =>
            await services.Queries.GetItemsAsync(paginationRequest))
            .WithName("ListItems");

        api.MapGet("/items/{id:int}", async (int id, [AsParameters] CatalogServices services) =>
            await services.Queries.GetItemAsync(id));

        return app;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "endpoint.exists", "element": "list-items", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:9" }
  ]
}
```

## verified-unnamed-lambda-by-verb-and-route

The second lambda has neither a method name nor `.WithName`. It is `GetItemById`'s endpoint because it is the one `GET` mapped on `items/{id}` (the `int` constraint dropped, as the `route` comparer drops it). Traces ESC-SVC-19.

```json case
{ "phase": 4, "base": "verified-by-with-name", "kinds": ["endpoint.exists"], "traces": ["ESC-SVC-19"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "endpoint.exists", "element": "get-item-by-id", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:13" }
  ]
}
```

## contradicted-missing

The router no longer maps `CancelOrderAsync`; only the handler method is left. The endpoint is contradicted, one finding saying it isn't in the code (R8). Traces ESC-SVC-19.

```json case
{ "phase": 4, "base": "verified-by-handler-name", "kinds": ["endpoint.exists"], "traces": ["ESC-SVC-19"], "requirements": ["R8", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/endpoint.exists", "severity": "error", "subject": "cancel-order", "messageContains": "isn't in the code" }
  ]
}
```

## contradicted-mapped-without-http-settings

The Operation has no `Http Settings`, so the model says it isn't an endpoint, but the router maps it. The claim is contradicted, with the model side `absent` and the code side `present` at the `MapPut`. Traces ESC-SVC-20.

```json case
{ "phase": 4, "base": "verified-by-handler-name", "kinds": ["endpoint.exists"], "traces": ["ESC-SVC-20"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync" }
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
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "contradicted", "expected": "absent", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/endpoint.exists", "severity": "error", "subject": "cancel-order" }
  ]
}
```

## blocked-when-router-missing

The router class itself isn't in the code. `type.exists` on the Service is contradicted, and the endpoint, which depends on its router, is can't verify, `blocked`: only the missing router is reported.

```json case
{ "phase": 4, "base": "verified-by-handler-name", "without": ["src/Ordering.API/Apis/OrdersApi.cs"], "kinds": ["endpoint.exists", "type.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "orders-api", "outcome": "contradicted" },
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "orders-api", "messageContains": "isn't in the code" },
    { "rule": "claim/endpoint.exists", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## java-verified

Over Spring MVC, the Operation `CancelOrder` is the controller method `cancelOrder` (its name in camel case) with a `@PostMapping`. The method's node starts at its first annotation. Traces JAV-SVC-04.

```json case
{ "phase": 4, "kinds": ["endpoint.exists"], "traces": ["JAV-SVC-04"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
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
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:18" }
  ]
}
```

## java-contradicted-without-mapping

The method `cancelOrder` is still there, but nothing maps it: without a `*Mapping` annotation it isn't an endpoint, so the claim is contradicted. Traces JAV-SVC-04.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["endpoint.exists"], "traces": ["JAV-SVC-04"], "requirements": ["R8", "R33"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderService orders;

    public OrdersController(OrderService orders) {
        this.orders = orders;
    }

    public ResponseEntity<Void> cancelOrder(UUID orderId) {
        orders.cancel(orderId);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "endpoint.exists", "element": "cancel-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/endpoint.exists", "severity": "error", "subject": "cancel-order", "messageContains": "isn't in the code" }
  ]
}
```

## top-level-statements

Minimal API endpoints mapped in C# top-level statements belong to the `Program` class the compiler makes of them, which the code never declares. A Service named `Program` is that class, and its Operation `Root` the lambda `.WithName("Root")` names, read as any router's endpoint is: its verb, its route and what the lambda returns.

```json case
{ "phase": 4, "kinds": ["type.exists", "member.exists", "endpoint.exists", "operation.verb", "operation.route", "operation.returns"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "program", "type": "Service", "name": "Program",
            "children": [
              { "id": "root", "type": "Operation", "name": "Root", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "/" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root-folder", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Shop.Api/Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "Hello World!")
    .WithName("Root");

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "program", "outcome": "verified", "at": "src/Shop.Api/Program.cs:1" },
    { "kind": "member.exists", "element": "root", "outcome": "verified", "at": "src/Shop.Api/Program.cs:6" },
    { "kind": "endpoint.exists", "element": "root", "outcome": "verified", "at": "src/Shop.Api/Program.cs:6" },
    { "kind": "operation.verb", "element": "root", "outcome": "verified", "actual": "GET" },
    { "kind": "operation.route", "element": "root", "outcome": "verified", "actual": "" },
    { "kind": "operation.returns", "element": "root", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "count": 0 },
    { "rule": "claim/operation.route", "count": 0 }
  ]
}
```

## top-level-statements-contradicted

The top-level `MapGet` moves to `/hello`, which the Operation's route contradicts.

```json case
{ "phase": 4, "base": "top-level-statements", "kinds": ["operation.route"], "requirements": ["R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Shop.Api/Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapControllers();

app.MapGet("/hello", () => "Hello World!")
    .WithName("Root");

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "operation.route", "element": "root", "outcome": "contradicted", "actual": "hello", "at": "src/Shop.Api/Program.cs:6" }
  ],
  "findings": [
    { "rule": "claim/operation.route", "severity": "error", "subject": "root" }
  ]
}
```

## top-level-statements-with-partial-program

A test project's `WebApplicationFactory<Program>` needs `Program` public, so the code adds `public partial class Program { }` to its top-level statements: the endpoints are that declared class's.

```json case
{ "phase": 4, "base": "top-level-statements", "kinds": ["type.exists", "endpoint.exists"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Shop.Api/Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!")
    .WithName("Root");

app.Run();

public partial class Program { }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "program", "outcome": "verified", "at": "src/Shop.Api/Program.cs:9" },
    { "kind": "endpoint.exists", "element": "root", "outcome": "verified", "at": "src/Shop.Api/Program.cs:4" }
  ]
}
```
