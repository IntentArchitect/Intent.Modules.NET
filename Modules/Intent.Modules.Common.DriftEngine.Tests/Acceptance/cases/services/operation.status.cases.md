# operation.status

An Operation's `Http Settings` Success Response Code, against the success status its endpoint returns: `TypedResults.Created` is 201, `TypedResults.Ok` 200, Spring's `ResponseEntity.noContent()` 204, and so on. `operation.status` is `stated: whenSet` and compared as a number: the model's `201 (Created)` is `201`. A handler that returns its data itself, not a result, is sent with the framework's default 200, which a modelled 200 verifies: Intent's controllers send 201 for a POST that states no status, so a model must state the 200 such a POST sends.

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

## contradicted-plain-value

The handler returns the draft itself, as eShopCustom's `CreateOrderDraftAsync` does, so ASP.NET Core sends it with its default 200: the modelled `201` is contradicted, the code side the default `200`.

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
    { "kind": "operation.status", "element": "create-order-draft", "outcome": "contradicted", "expected": "201", "actual": "200", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/operation.status", "severity": "error", "subject": "create-order-draft", "messageContains": "it returns `OrderDraftDTO` itself), so it is `200`" }
  ]
}
```

## verified-plain-value

BasisLink's test controller POSTs return a `string` and a `Task<string>` themselves, which ASP.NET Core sends with 200. Intent's controllers send 201 for a POST with no Success Response Code, so the model states `200 (OK)`, and it verifies. `Lookup` returns `ActionResult<string>` and gives it the `string` itself, which is sent with 200 too, so it verifies. `SendNothing` returns `Task` and sends no data, so its modelled 200 is contradicted as not stated.

```json case
{ "phase": 4, "kinds": ["operation.status"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "BasisLink.Api",
        "elements": [
          { "id": "test-controller", "type": "Service", "name": "TestController",
            "stereotypes": { "Http Service Settings": { "Route": "v1/Test" } },
            "children": [
              { "id": "send-user", "type": "Operation", "name": "SendUser", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "SendUser", "Success Response Code": "200 (OK)" } } },
              { "id": "test", "type": "Operation", "name": "Test", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "Test", "Success Response Code": "200 (OK)" } } },
              { "id": "send-nothing", "type": "Operation", "name": "SendNothing",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "SendNothing", "Success Response Code": "200 (OK)" } } },
              { "id": "lookup", "type": "Operation", "name": "Lookup", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "Lookup", "Success Response Code": "200 (OK)" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "BasisLink", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/BasisLink.Api/Controllers/TestController.cs
using Microsoft.AspNetCore.Mvc;

namespace BasisLink.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class TestController : ControllerBase
{
    [HttpPost("SendUser")]
    public async Task<string> SendUser()
    {
        await Task.Yield();
        return "sent";
    }

    [HttpGet("Test")]
    public string Test()
    {
        return "";
    }

    [HttpPost("SendNothing")]
    public async Task SendNothing()
    {
        await Task.Yield();
    }

    [HttpGet("Lookup")]
    public ActionResult<string> Lookup()
    {
        return "found";
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "send-user", "outcome": "verified", "expected": "200", "actual": "200" },
    { "kind": "operation.status", "element": "test", "outcome": "verified", "expected": "200", "actual": "200" },
    { "kind": "operation.status", "element": "send-nothing", "outcome": "contradicted", "expected": "200", "actual": null },
    { "kind": "operation.status", "element": "lookup", "outcome": "verified", "expected": "200", "actual": "200" }
  ],
  "findings": [
    { "rule": "claim/operation.status", "subject": "send-user", "count": 0 },
    { "rule": "claim/operation.status", "subject": "test", "count": 0 },
    { "rule": "claim/operation.status", "subject": "lookup", "count": 0 },
    { "rule": "claim/operation.status", "severity": "error", "subject": "send-nothing", "messageContains": "doesn't state" }
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

## web-api-2-service-builds-response

A Web API 2 action returns what a service, behind its interface, builds: a `ResponseMessageResult` of the request's `CreateResponse(HttpStatusCode.OK)`, kept in a local, with an error result on another path. The success status is the 200 the service states.

```json case
{ "phase": 4, "kinds": ["operation.status"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Documents",
        "elements": [
          { "id": "search-request", "type": "DTO", "name": "SearchRequest",
            "children": [
              { "id": "search-request-query", "type": "DTO-Field", "name": "Query", "typeRef": { "type": "string", "nullable": true } }
            ] },
          { "id": "document-controller", "type": "Service", "name": "DocumentController",
            "stereotypes": { "Http Service Settings": { "Route": "api/documents" } },
            "children": [
              { "id": "search", "type": "Operation", "name": "SearchAsync",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "search", "Success Response Code": "200 (Ok)" } },
                "children": [
                  { "id": "search-request-parameter", "type": "Parameter", "name": "request", "typeRef": { "element": "search-request" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Documents", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Documents/Controllers/DocumentController.cs
using System.Threading.Tasks;
using System.Web.Http;
using Documents.Services;

namespace Documents.Controllers;

[RoutePrefix("api/documents")]
public class DocumentController : ApiController
{
    private readonly IDocumentSearchService _search;

    public DocumentController(IDocumentSearchService search)
    {
        _search = search;
    }

    [HttpPost]
    [Route("search")]
    public async Task<IHttpActionResult> SearchAsync([FromBody] SearchRequest request)
    {
        return await _search.SearchAsync(request, Request);
    }
}
```

```csharp file src/Documents/Services/DocumentSearchService.cs
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Documents.Services;

public interface IDocumentSearchService
{
    Task<IHttpActionResult> SearchAsync(SearchRequest request, HttpRequestMessage httpRequest);
}

public class DocumentSearchService : IDocumentSearchService
{
    public async Task<IHttpActionResult> SearchAsync(SearchRequest request, HttpRequestMessage httpRequest)
    {
        if (request.Query is null)
        {
            return new BadRequestResult(httpRequest);
        }

        var response = httpRequest.CreateResponse(HttpStatusCode.OK);
        return new ResponseMessageResult(response);
    }
}

public class SearchRequest
{
    public string? Query { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "search", "outcome": "verified", "expected": "200", "actual": "200",
      "at": "src/Documents/Controllers/DocumentController.cs:21" }
  ]
}
```

## web-api-2-content-error-status

A Web API 2 action succeeds with `Content(HttpStatusCode.Created, model)` and fails with `Content(HttpStatusCode.ExpectationFailed, ex.Message)`. Any status .NET's `HttpStatusCode` names is read, so the 417 is an error, not the success status, and isn't taken for ASP.NET Core's `Content(text)`, a 200: the success status is the 201 alone.

```json case
{ "phase": 4, "kinds": ["operation.status"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "CrmBus.Crm",
        "elements": [
          { "id": "address-controller", "type": "Service", "name": "AddressController",
            "stereotypes": { "Http Service Settings": { "Route": "api/Address" } },
            "children": [
              { "id": "add-address", "type": "Operation", "name": "AddAddress", "typeRef": { "type": "int" },
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "AddAddress", "Success Response Code": "201 (Created)" } },
                "children": [ { "id": "add-address-line1", "type": "Parameter", "name": "line1", "typeRef": { "type": "string" } } ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "CrmBus", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/CrmBus.Crm/Controllers/AddressController.cs
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace CrmBus.Crm.Controllers;

public interface IAddressLogic
{
    Task<int> AddAddress(string line1);
}

[RoutePrefix("api/Address")]
public class AddressController : ApiController
{
    private readonly IAddressLogic _logic;

    public AddressController(IAddressLogic logic)
    {
        _logic = logic;
    }

    [HttpPost]
    [Route("AddAddress")]
    public async Task<IHttpActionResult> AddAddress(string line1)
    {
        try
        {
            var id = await _logic.AddAddress(line1);
            return Content(HttpStatusCode.Created, id);
        }
        catch (Exception ex)
        {
            return Content(HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.status", "element": "add-address", "outcome": "verified", "expected": "201", "actual": "201" }
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
