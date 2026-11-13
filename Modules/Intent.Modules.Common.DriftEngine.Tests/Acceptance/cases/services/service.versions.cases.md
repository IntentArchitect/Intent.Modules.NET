# service.versions

A Service's API versions, against the versions its router's groups declare with `HasApiVersion`. eShopCustom models versions with its own module's `Query String Api Version` stereotype (Versions, as text such as `1.0, 2.0`), which no stock pack knows, so each case's house pack brings the vocabulary for it (R28), splitting the text into its items; the ASP.NET Core pack reads the code. `service.versions` is `stated: whenSet` (decision 4) and compared as a set whose items compare as versions, so `1`, `v1` and `1.0` are one item: the versions a router declares are the union over its groups.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and `Catalog.API/Apis/CatalogApi.cs`.

## verified

The Service's Versions are `1.0` and the router's group says `HasApiVersion(1.0)`. Traces ESC-SVC-17.

```json case
{ "phase": 4, "kinds": ["service.versions"], "traces": ["ESC-SVC-17"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" }, "Query String Api Version": { "Versions": "1.0" } },
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

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.aspnetcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "service.versions",
          "for": { "element": "Service", "where": "hasStereotype('Query String Api Version')" },
          "value": "split(stereotype('Query String Api Version').Versions, ',')",
          "bindings": { "Service": "name" },
          "own": ["Service"],
          "dependsOn": "type.exists",
          "source": "the Query String Api Version stereotype's Versions"
        }
      ]
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
        var api = app.MapGroup("api/orders").HasApiVersion(1.0);

        api.MapPut("/cancel", CancelOrderAsync);

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
    { "kind": "service.versions", "element": "orders-api", "outcome": "verified", "expected": "1.0", "actual": "1.0",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ]
}
```

## verified-union-over-groups

As in eShopCustom's `CatalogApi`, one versioned API maps two groups on the same prefix, one per version, each written `HasApiVersion(major, minor)`. The router's versions are the union, `1.0, 2.0`, which is the model's set. Traces ESC-SVC-17.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Ordering.API/Apis/OrdersApi.cs"], "kinds": ["service.versions"], "traces": ["ESC-SVC-17"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "catalog-api", "type": "Service", "name": "CatalogApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/catalog" }, "Query String Api Version": { "Versions": "1.0, 2.0" } },
            "children": [
              { "id": "get-all-items-v1", "type": "Operation", "name": "GetAllItemsV1",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" } } },
              { "id": "get-all-items", "type": "Operation", "name": "GetAllItems",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" } } }
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
        var vApi = app.NewVersionedApi("Catalog");
        var v1 = vApi.MapGroup("api/catalog").HasApiVersion(1, 0);
        var v2 = vApi.MapGroup("api/catalog").HasApiVersion(2, 0);

        v1.MapGet("/items", GetAllItemsV1);
        v2.MapGet("/items", GetAllItems);

        return app;
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItemsV1([AsParameters] PaginationRequest paginationRequest, [AsParameters] CatalogServices services)
        => TypedResults.Ok(await services.Queries.GetItemsAsync(paginationRequest));

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItems([AsParameters] PaginationRequest paginationRequest, [AsParameters] CatalogServices services)
        => TypedResults.Ok(await services.Queries.GetItemsAsync(paginationRequest));
}
```

```json expect
{
  "claims": [
    { "kind": "service.versions", "element": "catalog-api", "outcome": "verified", "expected": "1.0, 2.0", "actual": "1.0, 2.0" }
  ]
}
```

## verified-version-forms

The model writes the version as `1`, and the group declares `HasApiVersion(1.0)`. Items compare as versions, ignoring a leading `v` and trailing `.0` parts, so eShopCustom's "1 = v1 = 1.0" holds and the claim is verified with each side shown as written. Traces ESC-SVC-17 (1 = v1 = 1.0).

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.versions"], "traces": ["ESC-SVC-17"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" }, "Query String Api Version": { "Versions": "1" } },
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
    { "kind": "service.versions", "element": "orders-api", "outcome": "verified", "expected": "1", "actual": "1.0",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ]
}
```

## contradicted

The model says `1.0, 2.0` and the router declares only `1.0`. Traces ESC-SVC-17.

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.versions"], "traces": ["ESC-SVC-17"], "requirements": ["R1", "R10", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" }, "Query String Api Version": { "Versions": "1.0, 2.0" } },
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
    { "kind": "service.versions", "element": "orders-api", "outcome": "contradicted", "expected": "1.0, 2.0", "actual": "1.0",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ],
  "findings": [
    { "rule": "claim/service.versions", "severity": "error", "subject": "orders-api", "messageContains": "2.0" }
  ]
}
```

## contradicted-not-stated

The router's group declares no version at all. Asp.Versioning would apply its default version, but the code states none, so the modelled `1.0` is contradicted with the code side "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.versions"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
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
    { "kind": "service.versions", "element": "orders-api", "outcome": "contradicted", "expected": "1.0", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/service.versions", "severity": "error", "subject": "orders-api", "messageContains": "doesn't state" }
  ]
}
```

## unreadable

The group's version is a constant from another class, which the rule can't read. eShopCustom warned about a `HasApiVersion` argument it couldn't read; the claim is can't verify, `unreadable`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["service.versions"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders").HasApiVersion(OrderingApiVersions.V1);

        api.MapPut("/cancel", CancelOrderAsync);

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
    { "kind": "service.versions", "element": "orders-api", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ],
  "findings": [
    { "rule": "claim/service.versions", "severity": "error", "subject": "orders-api", "messageContains": "can't read" }
  ]
}
```
