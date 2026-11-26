# operation.openApi.operationId

An Operation's `OpenAPI Settings` OperationId (Intent.Metadata.WebApi), with `{MethodName}` and `{ServiceName}` replaced by the Operation's and its Service's names, first letter upper, as Intent's ASP.NET Core modules write it. ASP.NET Core's OpenAPI document takes an endpoint's operationId from its route name, else its last endpoint name (`OpenApiDocumentService.GetOperationId`, as Swashbuckle's default does): a Minimal API endpoint's `.WithName(...)`, else its handler's `[EndpointName]`, unless an OpenAPI operation callback chained on it sets `OperationId` itself; a controller action's route attribute `Name`, else its `[EndpointName]`. `stated: whenSet`, compared exactly. An endpoint with no name states none, so `optin/unmodelled.value` doesn't read it as a value. An Operation its OpenAPI Settings, or its Service's, leave out of the description (Ignore) claims none: an ignored endpoint has no operation, and the designer hides the field.

The fixtures adapt eShop's `Catalog.API/Apis/CatalogApi.cs`.

## verified-with-name

`GetAllItems` is modelled with the OperationId `ListItems`, and its endpoint is `.WithName("ListItems")`.

```json case
{ "phase": 4, "kinds": ["operation.openApi.operationId"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" }, "OpenAPI Settings": { "OperationId": "ListItems", "Ignore": false } } },
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

        api.MapGet("/items", GetAllItems)
            .WithName("ListItems")
            .WithSummary("List catalog items");
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "outcome": "verified", "expected": "ListItems", "actual": "ListItems",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:10" },
    { "kind": "operation.openApi.operationId", "element": "delete-item", "absent": true }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.operationId", "count": 0 }
  ]
}
```

## verified-substituted

The OperationId is `{ServiceName}_{MethodName}`, which Intent's modules write as `CatalogApi_GetAllItems`.

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" }, "OpenAPI Settings": { "OperationId": "{ServiceName}_{MethodName}" } } }
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

        api.MapGet("/items", GetAllItems)
            .WithName("CatalogApi_GetAllItems");

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "outcome": "verified", "expected": "CatalogApi_GetAllItems", "actual": "CatalogApi_GetAllItems",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:10" }
  ]
}
```

## contradicted

The endpoint is named `ListAllItems`: one finding, both names shown, the OperationId named as the model's side (R11, R14).

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems)
            .WithName("ListAllItems")
            .WithSummary("List catalog items");
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "outcome": "contradicted", "expected": "ListItems", "actual": "ListAllItems",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:10" }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.operationId", "severity": "error", "subject": "get-all-items",
      "messageContains": "OpenAPI operationId: model `ListItems`, code `ListAllItems`. The model states it in the operation's OpenAPI Settings OperationId." }
  ]
}
```

## contradicted-not-stated

The endpoint has no name, so OpenAPI gives it none: the modelled OperationId is contradicted, "not stated" (R4).

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems)
            .WithSummary("List catalog items");
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "outcome": "contradicted", "expected": "ListItems", "actual": null, "fromSilence": false,
      "at": "src/Catalog.API/Apis/CatalogApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.operationId", "severity": "error", "subject": "get-all-items", "messageContains": "the code doesn't state it" }
  ]
}
```

## verified-lambda

A lambda endpoint is its Operation by its verb and route when its name isn't the Operation's: `GetBrands`'s endpoint is `.WithName("ListBrands")`, its OperationId.

```json case
{ "phase": 4, "kinds": ["operation.openApi.operationId", "member.exists"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
              { "id": "get-brands", "type": "Operation", "name": "GetBrands", "typeRef": { "type": "string", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "brands" }, "OpenAPI Settings": { "OperationId": "ListBrands" } } }
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

        api.MapGet("/brands", () => new[] { "a" })
            .WithName("ListBrands");

        return app;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "get-brands", "outcome": "verified" },
    { "kind": "operation.openApi.operationId", "element": "get-brands", "outcome": "verified", "expected": "ListBrands", "actual": "ListBrands",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:10" }
  ]
}
```

## lambda-renamed-is-one-finding

The lambda's name is neither its Operation's nor its OperationId, so the code has no endpoint for `GetBrands`: one finding, the missing operation's, and the OperationId waits on it.

```json case
{ "phase": 4, "base": "verified-lambda", "kinds": ["operation.openApi.operationId", "member.exists"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/brands", () => new[] { "a" })
            .WithName("ListAllBrands");

        return app;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "get-brands", "outcome": "contradicted" },
    { "kind": "operation.openApi.operationId", "element": "get-brands", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "get-brands" },
    { "rule": "claim/operation.openApi.operationId", "count": 0 }
  ]
}
```

## verified-openapi-callback

An OpenAPI operation callback sets the operationId over the endpoint's name: `.WithOpenApi(op => ...)`, and .NET 10's `AddOpenApiOperationTransformer`, whose callback runs later and so decides it.

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" }, "OpenAPI Settings": { "OperationId": "ListItems" } } },
              { "id": "delete-item", "type": "Operation", "name": "DeleteItemById",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "items/{id}" }, "OpenAPI Settings": { "OperationId": "DeleteItem" } } }
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

        api.MapGet("/items", GetAllItems)
            .WithName("GetAllItems")
            .WithOpenApi(operation =>
            {
                operation.OperationId = "ListItems";
                return operation;
            });
        api.MapDelete("/items/{id:int}", DeleteItemById)
            .WithOpenApi(operation => new(operation) { OperationId = "RemoveItem" })
            .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
            {
                operation.OperationId = "DeleteItem";
                return Task.CompletedTask;
            });

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "outcome": "verified", "expected": "ListItems", "actual": "ListItems",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:13" },
    { "kind": "operation.openApi.operationId", "element": "delete-item", "outcome": "verified", "expected": "DeleteItem", "actual": "DeleteItem",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:20" }
  ]
}
```

## verified-endpoint-name-attribute

The handler method has `[EndpointName("ListItems")]` and the endpoint no `.WithName`: the attribute is the endpoint's name metadata.

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems);
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    [EndpointName("ListItems")]
    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "outcome": "verified", "expected": "ListItems", "actual": "ListItems",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:15" }
  ]
}
```

## unreadable

The name is a value the code computes, so what it is can't be read without running the code.

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems)
            .WithName(Names.For("items"));
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:10" }
  ]
}
```

## verified-controller-route-name

A controller action's operationId is its route's name: `[HttpGet("{id}", Name = "GetItem")]`.

```json case
{ "phase": 4, "kinds": ["operation.openApi.operationId"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "items-service", "type": "Service", "name": "ItemsService",
            "stereotypes": { "Http Service Settings": { "Route": "api/items" } },
            "children": [
              { "id": "get-item", "type": "Operation", "name": "GetItem", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" }, "OpenAPI Settings": { "OperationId": "GetItem" } },
                "children": [ { "id": "get-item-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } } ] },
              { "id": "delete-item", "type": "Operation", "name": "DeleteItem",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "{id}" }, "OpenAPI Settings": { "OperationId": "{MethodName}" } },
                "children": [ { "id": "delete-item-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } } ] }
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
public class ItemsController : ControllerBase
{
    [HttpGet("{id}", Name = "GetItem")]
    public ActionResult<string> GetItem(int id) => Ok("item");

    [HttpDelete("{id}")]
    [EndpointName(Names.DeleteItem)]
    public IActionResult DeleteItem(int id) => NoContent();
}

public static class Names
{
    public const string DeleteItem = "DeleteItem";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-item", "outcome": "verified", "expected": "GetItem", "actual": "GetItem",
      "at": "src/Catalog.API/Controllers/ItemsController.cs:9" },
    { "kind": "operation.openApi.operationId", "element": "delete-item", "outcome": "verified", "expected": "DeleteItem", "actual": "DeleteItem",
      "at": "src/Catalog.API/Controllers/ItemsController.cs:13" }
  ]
}
```

## contradicted-controller-without-route-name

The action's route has no name, so OpenAPI gives it none.

```json case
{ "phase": 4, "base": "verified-controller-route-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Controllers/ItemsController.cs
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<string> GetItem(int id) => Ok("item");

    [HttpDelete("{id}")]
    [EndpointName(Names.DeleteItem)]
    public IActionResult DeleteItem(int id) => NoContent();
}

public static class Names
{
    public const string DeleteItem = "DeleteItem";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-item", "outcome": "contradicted", "expected": "GetItem", "actual": null }
  ],
  "findings": [
    { "rule": "claim/operation.openApi.operationId", "severity": "error", "subject": "get-item", "messageContains": "the code doesn't state it" }
  ]
}
```

## library-attribute-cant-verify

Swashbuckle's `[SwaggerOperation(OperationId = ...)]` names the operationId only with Swashbuckle's annotations turned on, which isn't in the code read, and ASP.NET Core's own operationId is the route's name: can't verify, never a guess either way.

```json case
{ "phase": 4, "base": "verified-controller-route-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Controllers/ItemsController.cs
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    [HttpGet("{id}")]
    [SwaggerOperation(OperationId = "GetItem")]
    public ActionResult<string> GetItem(int id) => Ok("item");

    [HttpDelete("{id}")]
    [EndpointName(Names.DeleteItem)]
    public IActionResult DeleteItem(int id) => NoContent();
}

public static class Names
{
    public const string DeleteItem = "DeleteItem";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-item", "outcome": "cantVerify", "reason": "unresolved",
      "at": "src/Catalog.API/Controllers/ItemsController.cs:11" }
  ]
}
```

## web-api-2-cant-verify

ASP.NET Web API 2 has no OpenAPI of its own: a route's name isn't an operationId, so what an OpenAPI generator names the action isn't in the code.

```json case
{ "phase": 4, "kinds": ["operation.openApi.operationId"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "orders-service", "type": "Service", "name": "OrdersService",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "get-order", "type": "Operation", "name": "GetOrder", "typeRef": { "type": "string" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" }, "OpenAPI Settings": { "OperationId": "GetOrder" } },
                "children": [ { "id": "get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } } ] }
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

```csharp file src/Shop.Api/Controllers/OrdersController.cs
using System.Web.Http;

namespace Shop.Api.Controllers;

[RoutePrefix("api/orders")]
public class OrdersController : ApiController
{
    [HttpGet]
    [Route("{id}", Name = "GetOrder")]
    public string GetOrder(int id) => "order";
}
```

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-order", "outcome": "cantVerify", "reason": "unresolved",
      "at": "src/Shop.Api/Controllers/OrdersController.cs:8" }
  ]
}
```

## not-claimed-when-ignored

The Operation's OpenAPI Settings leave it out of the description, and the designer hides the OperationId: an ignored endpoint has no operation, so a stale OperationId claims nothing.

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" }, "OpenAPI Settings": { "OperationId": "Other", "Ignore": true } } }
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

```json expect
{
  "claims": [
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "absent": true }
  ]
}
```

## unmodelled-value-reports-a-name

With `optin/unmodelled.value` on, an endpoint's name the model leaves unset is reported where the code states it.

```json case
{ "phase": 4, "base": "verified-with-name", "kinds": ["operation.openApi.operationId"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
    { "kind": "operation.openApi.operationId", "element": "get-all-items", "absent": true }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Catalog.API/Apis/CatalogApi.cs:10", "messageContains": "OpenAPI operationId: the code states `ListItems`" },
    { "rule": "optin/unmodelled.value", "messageContains": "OpenAPI operationId", "count": 1 }
  ]
}
```

## unmodelled-value-endpoint-without-name

An endpoint with no name states no operationId, so the rule has nothing to report for `DeleteItemById` or a renamed-away `GetAllItems`: no name is silence, not the value `none`.

```json case
{ "phase": 4, "base": "unmodelled-value-reports-a-name", "kinds": ["operation.openApi.operationId"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems)
            .WithSummary("List catalog items");
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static Task<Ok<string[]>> GetAllItems() => Task.FromResult(TypedResults.Ok(new[] { "a" }));

    public static Task<NoContent> DeleteItemById(int id) => Task.FromResult(TypedResults.NoContent());
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "messageContains": "OpenAPI operationId", "count": 0, "note": "No name is silence" }
  ]
}
```
