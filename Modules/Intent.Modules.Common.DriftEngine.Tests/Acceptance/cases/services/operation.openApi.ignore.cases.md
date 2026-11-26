# operation.openApi.ignore

Whether an endpoint is left out of the API description: an Operation's `OpenAPI Settings` Ignore, else its Service's (Intent.Metadata.WebApi). A Minimal API endpoint is left out by `ExcludeFromDescription()` on itself, its group or its router's mapping call, or `[ExcludeFromDescription]` on its handler, the only metadata Minimal APIs' API description provider reads; a controller action by the first `[ApiExplorerSettings(IgnoreApi = ...)]` on the action, else its controller, as MVC's application model reads it. Silence is `false`: an endpoint is described unless something leaves it out. `stated: whenSet`, so an endpoint with no OpenAPI Settings on it or its Service claims nothing, and `optin/unmodelled.value` reports an exclusion the model doesn't state.

## verified-excluded

`GetHidden` is modelled with Ignore, and its endpoint is `.ExcludeFromDescription()`. `GetAllItems`' OpenAPI Settings leave Ignore off, and nothing excludes its endpoint: `false`, which the code states by saying nothing.

```json case
{ "phase": 4, "kinds": ["operation.openApi.ignore"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "get-all-items", "type": "Operation", "name": "GetAllItems",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" }, "OpenAPI Settings": { "Ignore": false } } },
              { "id": "get-hidden", "type": "Operation", "name": "GetHidden",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "hidden" }, "OpenAPI Settings": { "Ignore": true } } },
              { "id": "delete-item", "type": "Operation", "name": "DeleteItemById",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "items/{id}" } } }
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

        api.MapGet("/items", GetAllItems);
        api.MapGet("/hidden", GetHidden)
            .ExcludeFromDescription();
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static string GetHidden() => "";

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-all-items", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true,
      "at": "src/Catalog.API/Apis/CatalogApi.cs:9" },
    { "kind": "operation.openApi.ignore", "element": "get-hidden", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:11" },
    { "kind": "operation.openApi.ignore", "element": "delete-item", "absent": true }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.ignore", "count": 0 }
  ]
}
```

## contradicted-not-excluded

The endpoint the model hides is described: the code states `false` by excluding nothing (R3, R14).

```json case
{ "phase": 4, "base": "verified-excluded", "kinds": ["operation.openApi.ignore"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems);
        api.MapGet("/hidden", GetHidden);
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static string GetHidden() => "";

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-hidden", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true,
      "at": "src/Catalog.API/Apis/CatalogApi.cs:10" }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.ignore", "severity": "error", "subject": "get-hidden",
      "messageContains": "hidden from OpenAPI: model `true`, code `false`, which it states by saying nothing. The model states it in the operation's OpenAPI Settings Ignore." },
    { "rule": "claim/operation.openApi.ignore", "count": 1 }
  ]
}
```

## contradicted-excluded

The model describes `GetAllItems`, and its endpoint is left out by `[ExcludeFromDescription]` on its handler.

```json case
{ "phase": 4, "base": "verified-excluded", "kinds": ["operation.openApi.ignore"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems);
        api.MapGet("/hidden", GetHidden)
            .ExcludeFromDescription();
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    [ExcludeFromDescription]
    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static string GetHidden() => "";

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-all-items", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:17" }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.ignore", "severity": "error", "subject": "get-all-items", "messageContains": "model `false`, code `true`" }
  ]
}
```

## verified-group-and-service

The Service's OpenAPI Settings hide every Operation of it that has none of its own, and the group its endpoints are mapped on is excluded by a statement on the group's variable. A lambda endpoint's own `[ExcludeFromDescription]` excludes it too.

```json case
{ "phase": 4, "kinds": ["operation.openApi.ignore"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "internal-api", "type": "Service", "name": "InternalApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/internal" }, "OpenAPI Settings": { "Ignore": true } },
            "children": [
              { "id": "get-stats", "type": "Operation", "name": "GetStats",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "stats" } } }
            ] },
          { "id": "tools-api", "type": "Service", "name": "ToolsApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/tools" } },
            "children": [
              { "id": "ping", "type": "Operation", "name": "Ping", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "ping" }, "OpenAPI Settings": { "Ignore": true } } }
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

```csharp file src/Catalog.API/Apis/InternalApi.cs
namespace Catalog.API.Apis;

public static class InternalApi
{
    public static IEndpointRouteBuilder MapInternalApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/internal");
        api.ExcludeFromDescription();

        api.MapGet("/stats", GetStats);

        return app;
    }

    public static string GetStats() => "";
}
```

```csharp file src/Catalog.API/Apis/ToolsApi.cs
namespace Catalog.API.Apis;

public static class ToolsApi
{
    public static IEndpointRouteBuilder MapToolsApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/tools");

        api.MapGet("/ping", [ExcludeFromDescription] () => "pong")
            .WithName("Ping");

        return app;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-stats", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Apis/InternalApi.cs:8" },
    { "kind": "operation.openApi.ignore", "element": "ping", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Apis/ToolsApi.cs:9" }
  ]
}
```

## verified-group-chain-and-metadata

The group is excluded where it is made (`app.MapGroup(...).ExcludeFromDescription()`), and a lambda endpoint by the metadata `ExcludeFromDescription()` adds (`.WithMetadata(new ExcludeFromDescriptionAttribute())`).

```json case
{ "phase": 4, "base": "verified-group-and-service", "kinds": ["operation.openApi.ignore"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/InternalApi.cs
namespace Catalog.API.Apis;

public static class InternalApi
{
    public static IEndpointRouteBuilder MapInternalApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/internal")
            .ExcludeFromDescription();

        api.MapGet("/stats", GetStats);

        return app;
    }

    public static string GetStats() => "";
}
```

```csharp file src/Catalog.API/Apis/ToolsApi.cs
namespace Catalog.API.Apis;

public static class ToolsApi
{
    public static IEndpointRouteBuilder MapToolsApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/tools");

        api.MapGet("/ping", () => "pong")
            .WithName("Ping")
            .WithMetadata(new ExcludeFromDescriptionAttribute());

        return app;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-stats", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Apis/InternalApi.cs:8" },
    { "kind": "operation.openApi.ignore", "element": "ping", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Apis/ToolsApi.cs:11" }
  ]
}
```

## contradicted-service-not-excluded

The group is no longer excluded, so the Service's hidden Operation is described. The finding says the Service's OpenAPI Settings state it.

```json case
{ "phase": 4, "base": "verified-group-and-service", "kinds": ["operation.openApi.ignore"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/InternalApi.cs
namespace Catalog.API.Apis;

public static class InternalApi
{
    public static IEndpointRouteBuilder MapInternalApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/internal");

        api.MapGet("/stats", GetStats).WithSummary("Statistics");

        return app;
    }

    public static string GetStats() => "";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-stats", "outcome": "contradicted", "expected": "true", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.ignore", "severity": "error", "subject": "get-stats", "messageContains": "its service's OpenAPI Settings Ignore" }
  ]
}
```

## verified-router-mapping

The router's mapping method is called with `.ExcludeFromDescription()` in Program.cs: the group it returns is excluded, so every endpoint the router maps is.

```json case
{ "phase": 4, "base": "verified-group-and-service", "kinds": ["operation.openApi.ignore"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/InternalApi.cs
namespace Catalog.API.Apis;

public static class InternalApi
{
    public static RouteGroupBuilder MapInternalApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/internal");

        api.MapGet("/stats", GetStats);

        return api;
    }

    public static string GetStats() => "";
}
```

```csharp file src/Catalog.API/Program.cs
using Catalog.API.Apis;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapInternalApi()
    .ExcludeFromDescription();
app.MapToolsApi();

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-stats", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Program.cs:7" }
  ]
}
```

## verified-controller

A controller action's `[ApiExplorerSettings(IgnoreApi = true)]` hides it. `ItemsController`'s own `IgnoreApi = true` hides its actions but one whose own settings override it: `[ApiExplorerSettings(GroupName = "v1")]` leaves IgnoreApi `false`.

```json case
{ "phase": 4, "kinds": ["operation.openApi.ignore"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "items-service", "type": "Service", "name": "ItemsService",
            "stereotypes": { "Http Service Settings": { "Route": "api/items" }, "OpenAPI Settings": { "Ignore": true } },
            "children": [
              { "id": "get-items", "type": "Operation", "name": "GetItems", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "" } } },
              { "id": "get-item", "type": "Operation", "name": "GetItem", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" }, "OpenAPI Settings": { "Ignore": false } },
                "children": [ { "id": "get-item-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } } ] }
            ] },
          { "id": "orders-service", "type": "Service", "name": "OrdersService",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "purge-orders", "type": "Operation", "name": "PurgeOrders",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "" }, "OpenAPI Settings": { "Ignore": true } } }
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

```csharp file src/Catalog.API/Controllers/ItemsController.cs
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/items")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ItemsController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> GetItems() => Ok("items");

    [HttpGet("{id}")]
    [ApiExplorerSettings(GroupName = "v1")]
    public ActionResult<string> GetItem(int id) => Ok("item");
}
```

```csharp file src/Catalog.API/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpDelete]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult PurgeOrders() => NoContent();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "get-items", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Controllers/ItemsController.cs:7" },
    { "kind": "operation.openApi.ignore", "element": "get-item", "outcome": "verified", "expected": "false", "actual": "false",
      "at": "src/Catalog.API/Controllers/ItemsController.cs:14" },
    { "kind": "operation.openApi.ignore", "element": "purge-orders", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Controllers/OrdersController.cs:10" }
  ]
}
```

## contradicted-controller

`PurgeOrders` loses its `[ApiExplorerSettings]`, so MVC describes it.

```json case
{ "phase": 4, "base": "verified-controller", "kinds": ["operation.openApi.ignore"], "requirements": ["R1"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpDelete]
    public IActionResult PurgeOrders() => NoContent();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "purge-orders", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.ignore", "severity": "error", "subject": "purge-orders" }
  ]
}
```

## unreadable

`IgnoreApi` is given a value the code computes, so whether the action is described can't be read.

```json case
{ "phase": 4, "base": "verified-controller", "kinds": ["operation.openApi.ignore"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpDelete]
    [ApiExplorerSettings(IgnoreApi = Features.HideAdmin)]
    public IActionResult PurgeOrders() => NoContent();
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.ignore", "element": "purge-orders", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Catalog.API/Controllers/OrdersController.cs:10" }
  ]
}
```

## unmodelled-value-reports-exclusion

With `optin/unmodelled.value` on, an exclusion the model doesn't state is reported where the code states it. An endpoint the code describes isn't: `false` is what the model says by leaving it unset.

```json case
{ "phase": 4, "base": "verified-excluded", "kinds": ["operation.openApi.ignore"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "get-all-items", "type": "Operation", "name": "GetAllItems",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" } } },
              { "id": "get-hidden", "type": "Operation", "name": "GetHidden",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "hidden" } } },
              { "id": "delete-item", "type": "Operation", "name": "DeleteItemById",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "items/{id}" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Catalog", "type": "Root Folder",
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
    { "kind": "operation.openApi.ignore", "element": "get-hidden", "absent": true }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Catalog.API/Apis/CatalogApi.cs:11", "messageContains": "hidden from OpenAPI: the code states `true`" },
    { "rule": "optin/unmodelled.value", "messageContains": "hidden from OpenAPI", "count": 1 }
  ]
}
```
