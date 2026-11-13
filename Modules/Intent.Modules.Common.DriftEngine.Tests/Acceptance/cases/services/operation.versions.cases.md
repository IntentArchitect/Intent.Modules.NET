# operation.versions

An Operation's API versions, against the versions its endpoint serves: its own `MapToApiVersion` or `HasApiVersion`, else those its group declares, which it inherits. As for [service.versions](service.versions.cases.md), the versions come from eShopCustom's own `Query String Api Version` stereotype, so the house pack brings the vocabulary and the ASP.NET Core pack reads the code. `operation.versions` is `stated: whenSet` (decision 4) and compared as a set whose items compare as versions (`1`, `v1` and `1.0` are one item).

The fixtures adapt eShopCustom's `Catalog.API/Apis/CatalogApi.cs`.

## verified-map-to-api-version

`GetItemsByIds` is modelled for `2.0` only, and its endpoint narrows its group's versions with `MapToApiVersion(2, 0)`. Traces ESC-SVC-17.

```json case
{ "phase": 4, "kinds": ["operation.versions"], "traces": ["ESC-SVC-17"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
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
              { "id": "get-all-items", "type": "Operation", "name": "GetAllItems",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items" }, "Query String Api Version": { "Versions": "1.0, 2.0" } } },
              { "id": "get-items-by-ids", "type": "Operation", "name": "GetItemsByIds",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items/by" }, "Query String Api Version": { "Versions": "2.0" } } }
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
        },
        {
          "kind": "operation.versions",
          "for": { "element": "Operation", "where": "hasStereotype('Query String Api Version')" },
          "value": "split(stereotype('Query String Api Version').Versions, ',')",
          "bindings": { "Service": "parent.name", "Operation": "name" },
          "own": ["Operation"],
          "dependsOn": "endpoint.exists",
          "source": "the Operation's Query String Api Version stereotype's Versions"
        }
      ]
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
        var api = app.NewVersionedApi("Catalog").MapGroup("api/catalog").HasApiVersion(1, 0).HasApiVersion(2, 0);

        api.MapGet("/items", GetAllItems);
        api.MapGet("/items/by", GetItemsByIds).MapToApiVersion(2, 0);

        return app;
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItems([AsParameters] PaginationRequest paginationRequest, [AsParameters] CatalogServices services)
        => TypedResults.Ok(await services.Queries.GetItemsAsync(paginationRequest));

    public static async Task<Ok<List<CatalogItem>>> GetItemsByIds([AsParameters] CatalogServices services, int[] ids)
        => TypedResults.Ok(await services.Queries.GetItemsByIdsAsync(ids));
}
```

```json expect
{
  "claims": [
    { "kind": "operation.versions", "element": "get-items-by-ids", "outcome": "verified", "expected": "2.0", "actual": "2.0",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:10" }
  ]
}
```

## verified-inherited-from-group

`GetAllItems`' endpoint says nothing about versions of its own, so it serves its group's `1.0, 2.0`: the reading is inherited from the group, which states it explicitly. Traces ESC-SVC-17.

```json case
{ "phase": 4, "base": "verified-map-to-api-version", "kinds": ["operation.versions"], "traces": ["ESC-SVC-17"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "operation.versions", "element": "get-all-items", "outcome": "verified", "expected": "1.0, 2.0", "actual": "1.0, 2.0",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:7" }
  ]
}
```

## contradicted-inherits-wider-group

`GetItemsByIds` loses its `MapToApiVersion`, so it inherits both of its group's versions while the model says `2.0` only. Traces ESC-SVC-17.

```json case
{ "phase": 4, "base": "verified-map-to-api-version", "kinds": ["operation.versions"], "traces": ["ESC-SVC-17"], "requirements": ["R1", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.NewVersionedApi("Catalog").MapGroup("api/catalog").HasApiVersion(1, 0).HasApiVersion(2, 0);

        api.MapGet("/items", GetAllItems);
        api.MapGet("/items/by", GetItemsByIds);

        return app;
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItems([AsParameters] PaginationRequest paginationRequest, [AsParameters] CatalogServices services)
        => TypedResults.Ok(await services.Queries.GetItemsAsync(paginationRequest));

    public static async Task<Ok<List<CatalogItem>>> GetItemsByIds([AsParameters] CatalogServices services, int[] ids)
        => TypedResults.Ok(await services.Queries.GetItemsByIdsAsync(ids));
}
```

```json expect
{
  "claims": [
    { "kind": "operation.versions", "element": "get-items-by-ids", "outcome": "contradicted", "expected": "2.0", "actual": "1.0, 2.0",
      "at": "src/Catalog.API/Apis/CatalogApi.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.versions", "severity": "error", "subject": "get-items-by-ids", "messageContains": "1.0" }
  ]
}
```

## contradicted-not-stated

Neither the group nor the endpoints declare any version. The modelled versions are contradicted with the code side "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified-map-to-api-version", "kinds": ["operation.versions"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems);
        api.MapGet("/items/by", GetItemsByIds);

        return app;
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItems([AsParameters] PaginationRequest paginationRequest, [AsParameters] CatalogServices services)
        => TypedResults.Ok(await services.Queries.GetItemsAsync(paginationRequest));

    public static async Task<Ok<List<CatalogItem>>> GetItemsByIds([AsParameters] CatalogServices services, int[] ids)
        => TypedResults.Ok(await services.Queries.GetItemsByIdsAsync(ids));
}
```

```json expect
{
  "claims": [
    { "kind": "operation.versions", "element": "get-items-by-ids", "outcome": "contradicted", "expected": "2.0", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/operation.versions", "severity": "error", "subject": "get-items-by-ids", "messageContains": "doesn't state" }
  ]
}
```

## blocked-when-endpoint-missing

`GetItemsByIds` isn't mapped. Its endpoint is contradicted, and its versions, which depend on it, are can't verify, `blocked`.

```json case
{ "phase": 4, "base": "verified-map-to-api-version", "kinds": ["operation.versions", "endpoint.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.NewVersionedApi("Catalog").MapGroup("api/catalog").HasApiVersion(1, 0).HasApiVersion(2, 0);

        api.MapGet("/items", GetAllItems);

        return app;
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItems([AsParameters] PaginationRequest paginationRequest, [AsParameters] CatalogServices services)
        => TypedResults.Ok(await services.Queries.GetItemsAsync(paginationRequest));

    public static async Task<Ok<List<CatalogItem>>> GetItemsByIds([AsParameters] CatalogServices services, int[] ids)
        => TypedResults.Ok(await services.Queries.GetItemsByIdsAsync(ids));
}
```

```json expect
{
  "claims": [
    { "kind": "endpoint.exists", "element": "get-items-by-ids", "outcome": "contradicted", "expected": "present", "actual": "absent" },
    { "kind": "operation.versions", "element": "get-items-by-ids", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "endpoint.exists" }
  ],
  "findings": [
    { "rule": "claim/endpoint.exists", "severity": "error", "subject": "get-items-by-ids", "messageContains": "isn't in the code" },
    { "rule": "claim/operation.versions", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```
