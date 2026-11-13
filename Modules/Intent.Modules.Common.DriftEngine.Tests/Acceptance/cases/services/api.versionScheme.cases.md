# api.versionScheme

How a versioned Service reads its API version from a request, against the `ApiVersionReader` the application configures in `AddApiVersioning`. eShopCustom models it with its own `Query String Api Version` stereotype, whose `Parameter` names the query string parameter (its default, `api-version`, counts as unset), so the house pack brings the vocabulary: a claim exists only where Parameter is set, valued `query: <parameter>`. Intent's stock `Api Version Settings` stereotype puts the version in the route instead, so the Services pack's vocabulary claims `url segment` for it. The code states it in one global setting in another file than the router, or with an `{x:apiVersion}` segment in the router's route, so its rule is a plug-in of the ASP.NET Core pack, and the kind arrives `later`. `api.versionScheme` is `stated: whenSet` (decision 4).

The fixtures adapt eShopCustom's `Ordering.API` (`Program.cs`, `Apis/OrdersApi.cs`).

## query-parameter-verified

The Service's Parameter is `version`, and `Program.cs` configures `new QueryStringApiVersionReader("version")`. Traces ESC-SVC-18 (the query reader's parameter).

```json case
{ "phase": "later", "kinds": ["api.versionScheme"], "traces": ["ESC-SVC-18"], "requirements": ["R2", "R28", "R48"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" }, "Query String Api Version": { "Versions": "1.0", "Parameter": "version" } },
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
          "kind": "api.versionScheme",
          "for": { "element": "Service", "where": "hasStereotype('Query String Api Version')" },
          "when": "exists(stereotype('Query String Api Version').Parameter)",
          "value": "'query: ' + stereotype('Query String Api Version').Parameter",
          "bindings": { "Service": "name" },
          "own": ["Service"],
          "source": "the Query String Api Version stereotype's Parameter"
        }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new QueryStringApiVersionReader("version");
});

var app = builder.Build();

app.MapOrdersApiV1();

app.Run();
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
    { "kind": "api.versionScheme", "element": "orders-api", "outcome": "verified", "expected": "query: version", "actual": "query: version",
      "at": "src/Ordering.API/Program.cs:5" }
  ]
}
```

## url-segment-contradicted

The application reads the version from a URL segment instead, so the query string parameter the model names is never read. Traces ESC-SVC-18 (the scheme).

```json case
{ "phase": "later", "base": "query-parameter-verified", "kinds": ["api.versionScheme"], "traces": ["ESC-SVC-18"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

app.MapOrdersApiV1();

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "api.versionScheme", "element": "orders-api", "outcome": "contradicted", "expected": "query: version", "actual": "url segment",
      "at": "src/Ordering.API/Program.cs:5" }
  ],
  "findings": [
    { "rule": "claim/api.versionScheme", "severity": "error", "subject": "orders-api" }
  ]
}
```

## not-stated-without-reader

`AddApiVersioning` sets no reader. Asp.Versioning's default reader would read `api-version`, not `version`, but either way the code states nothing, and a convention never verifies a `whenSet` value: contradicted, "not stated" (decision 4).

```json case
{ "phase": "later", "base": "query-parameter-verified", "kinds": ["api.versionScheme"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning();

var app = builder.Build();

app.MapOrdersApiV1();

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "api.versionScheme", "element": "orders-api", "outcome": "contradicted", "expected": "query: version", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/api.versionScheme", "severity": "error", "subject": "orders-api", "messageContains": "doesn't state" }
  ]
}
```

## api-version-settings-url-segment

The Service uses Intent's stock `Api Version Settings` instead, whose `Applicable Versions` holds the `1.0` Version of an `Api Version` element. That stereotype puts the version in the route, so it states the URL segment scheme; being stock, its vocabulary is the Services pack's, and no house pack is needed. `Program.cs` configures `new UrlSegmentApiVersionReader()`. Traces ESC-SVC-18 (Api Version Settings requires URL segment).

```json case
{ "phase": "later", "kinds": ["api.versionScheme"], "traces": ["ESC-SVC-18"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "ordering-api-version", "type": "Api Version", "name": "OrderingApiVersion",
            "children": [
              { "id": "version-1-0", "type": "Version", "name": "1.0" }
            ] },
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": {
              "Http Service Settings": { "Route": "api/v{version}/orders" },
              "Api Version Settings": { "Applicable Versions": [ { "ref": "version-1-0" } ] }
            },
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

```csharp file src/Ordering.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

app.MapOrdersApiV1();

app.Run();
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/v{version:apiVersion}/orders").HasApiVersion(1.0);

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
    { "kind": "api.versionScheme", "element": "orders-api", "outcome": "verified", "expected": "url segment", "actual": "url segment",
      "at": "src/Ordering.API/Program.cs:5" }
  ]
}
```

## default-reader-with-route-constraint

`AddApiVersioning` sets no reader, but the group's route carries the version as `{version:apiVersion}`. Asp.Versioning's default reader reads that segment, and the route states it explicitly, so the code states the URL segment scheme in the route. Traces ESC-SVC-18 (the default reader plus `{x:apiVersion}`).

```json case
{ "phase": "later", "base": "api-version-settings-url-segment", "kinds": ["api.versionScheme"], "traces": ["ESC-SVC-18"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning();

var app = builder.Build();

app.MapOrdersApiV1();

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "api.versionScheme", "element": "orders-api", "outcome": "verified", "expected": "url segment", "actual": "url segment",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:7" }
  ]
}
```
