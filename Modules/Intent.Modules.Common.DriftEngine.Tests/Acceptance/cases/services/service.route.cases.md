# service.route

A Service's `Http Service Settings` Route, against the route prefix its router states: the `MapGroup` prefix of a minimal API router, an ASP.NET controller's `[Route]` or Web API 2's `[RoutePrefix]` (else, for a controller a conventional route serves, that route's prefix up to its `{controller}`, as a default), or a Spring controller's class-level `@RequestMapping`. `service.route` is `stated: whenSet`: the model states a prefix only where the code states one (decision 4), and the `route` comparer ignores a leading or trailing `/`, case in literal segments and parameter constraints.

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

## web-api-2-conventional-route-verified

A Web API 2 controller with no `[RoutePrefix]` is served on the conventional route `MapHttpRoute` maps with a `{controller}`, `api/{controller}/{id}`, so `DiagnosticsController` is under `api/Diagnostics`, the Route the model gives its Service. That is the routing's convention, read as a default. Adapted from a migrated, OWIN self-hosted Web API 2 application, clean under Common.Drift v1.

```json case
{ "phase": 4, "kinds": ["service.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Workflow.Host",
        "elements": [
          { "id": "ping", "type": "DTO", "name": "Ping" },
          { "id": "diagnostics", "type": "Service", "name": "DiagnosticsController",
            "stereotypes": { "Http Service Settings": { "Route": "api/Diagnostics" } },
            "children": [
              { "id": "post-ping", "type": "Operation", "name": "PostPing",
                "stereotypes": { "Http Settings": { "Verb": "POST" } },
                "children": [{ "id": "post-ping-value", "type": "Parameter", "name": "value", "typeRef": { "element": "ping" } }] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Workflow", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Workflow.Host/Services/Startup.cs
namespace Workflow.Host;

internal class Startup
{
    public void Configuration(IAppBuilder appBuilder)
    {
        HttpConfiguration config = new HttpConfiguration();

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

        appBuilder.UseWebApi(config);
    }
}
```

```csharp file src/Workflow.Host/Controllers/DiagnosticsController.cs
namespace Workflow.Host.Controllers;

public class Ping
{
    public string SourceAddress { get; set; }
}

public class DiagnosticsController : ApiController
{
    [HttpPost]
    public Ping PostPing(Ping value) => value;
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "diagnostics", "outcome": "verified", "expected": "api/Diagnostics", "actual": "api/Diagnostics",
      "at": "src/Workflow.Host/Controllers/DiagnosticsController.cs:8" }
  ],
  "findings": [
    { "rule": "claim/service.route", "count": 0 },
    { "rule": "aspnetcore/conventional.route", "count": 0 }
  ]
}
```

## web-api-2-conventional-route-contradicted

The same code, with the Service's Route `api/diag`, where Web API 2 doesn't serve the controller. The finding names the route it does.

```json case
{ "phase": 4, "base": "web-api-2-conventional-route-verified", "kinds": ["service.route"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Workflow.Host",
        "elements": [
          { "id": "ping", "type": "DTO", "name": "Ping" },
          { "id": "diagnostics", "type": "Service", "name": "DiagnosticsController",
            "stereotypes": { "Http Service Settings": { "Route": "api/diag" } },
            "children": [
              { "id": "post-ping", "type": "Operation", "name": "PostPing",
                "stereotypes": { "Http Settings": { "Verb": "POST" } },
                "children": [{ "id": "post-ping-value", "type": "Parameter", "name": "value", "typeRef": { "element": "ping" } }] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Workflow", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "diagnostics", "outcome": "contradicted", "expected": "api/diag", "actual": "api/Diagnostics" }
  ],
  "findings": [
    { "rule": "claim/service.route", "severity": "error", "subject": "diagnostics", "messageContains": "conventional route" }
  ]
}
```

## web-api-2-conventional-route-not-reported-when-unset

The same code, with the Service's Route left unset and the `unmodelled.value` opt-in rule on. The conventional route is the routing's convention, not a value the code states for the controller, so the model leaving it unset isn't reported.

```json case
{ "phase": 4, "base": "web-api-2-conventional-route-verified", "kinds": ["service.route"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Workflow.Host",
        "elements": [
          { "id": "ping", "type": "DTO", "name": "Ping" },
          { "id": "diagnostics", "type": "Service", "name": "DiagnosticsController",
            "stereotypes": { "Http Service Settings": {} },
            "children": [
              { "id": "post-ping", "type": "Operation", "name": "PostPing",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/api/Diagnostics" } },
                "children": [{ "id": "post-ping-value", "type": "Parameter", "name": "value", "typeRef": { "element": "ping" } }] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Workflow", "type": "Root Folder",
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
    { "kind": "service.route", "element": "diagnostics", "absent": true }
  ],
  "findings": [
    { "rule": "claim/service.route", "count": 0 },
    { "rule": "optin/unmodelled.value", "subject": "src/Workflow.Host/Controllers/DiagnosticsController.cs:8", "count": 0 }
  ]
}
```

## web-api-2-attribute-routed-controller-not-stated

A Web API 2 controller with no `[RoutePrefix]` whose every action has a `[Route]` isn't served on the conventional route at all: attribute routing puts each action at its own route. So nothing states the controller's route, and the model's `api/Diagnostics` is contradicted as not stated.

```json case
{ "phase": 4, "base": "web-api-2-conventional-route-verified", "kinds": ["service.route"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Workflow.Host/Controllers/DiagnosticsController.cs
namespace Workflow.Host.Controllers;

public class Ping
{
    public string SourceAddress { get; set; }
}

public class DiagnosticsController : ApiController
{
    [HttpPost]
    [Route("api/diagnostics/ping")]
    public Ping PostPing(Ping value) => value;
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "diagnostics", "outcome": "contradicted", "expected": "api/Diagnostics" }
  ],
  "findings": [
    { "rule": "claim/service.route", "severity": "error", "subject": "diagnostics", "messageContains": "doesn't state it" }
  ]
}
```

## inherited-route

ASP.NET Core's `[Route]` is inherited: a controller with none of its own is at its base controller's, `[controller]` being its own name less `Controller`. An adopted application's `BaseController` carries `[Route("api/[controller]")]` for every controller.

```json case
{ "phase": 4, "kinds": ["service.route"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "get-orders", "type": "Operation", "name": "GetOrders",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Api/Controllers/BaseController.cs
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
}
```

```csharp file src/Api/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers;

public class OrdersController : BaseController
{
    [HttpGet]
    public IActionResult GetOrders() => Ok();
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-controller", "outcome": "verified", "expected": "api/orders", "at": "src/Api/Controllers/BaseController.cs:6" }
  ],
  "findings": [
    { "rule": "claim/service.route", "count": 0 },
    { "rule": "aspnetcore/conventional.route", "count": 0 }
  ]
}
```

## inherited-route-contradicted

The base controller's route moves to `v1`, and every controller deriving from it with it.

```json case
{ "phase": 4, "base": "inherited-route", "kinds": ["service.route"], "requirements": ["R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Api/Controllers/BaseController.cs
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public abstract class BaseController : ControllerBase
{
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-controller", "outcome": "contradicted", "expected": "api/orders", "actual": "v1/Orders", "at": "src/Api/Controllers/BaseController.cs:6" }
  ],
  "findings": [
    { "rule": "claim/service.route", "severity": "error", "subject": "orders-controller" }
  ]
}
```
