# security.roles

The `Secured` stereotype's Roles on an Operation (or inherited from its Service), against the roles the endpoint's authorization requires: `RequireAuthorization(new AuthorizeAttribute { Roles = ... })`, `RequireRole(...)` or `[Authorize(Roles = ...)]`. `security.roles` is `stated: whenSet` (decision 4) and compared as a set: the same roles in any order. It comes from Common.Drift's recipes alone, with no eShopCustom or Java check of its own ("conversion only"); eShopCustom's ESC-SVC-15 covers roles inside its `security` check, which these cases also show.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs`.

## verified-in-any-order

The model's Roles are `Admin, Manager` and the endpoint requires `Manager,Admin`. As sets they are the same, and both sides' canonical text is sorted: `Admin, Manager`. The endpoint is secured too. Traces ESC-SVC-15 (its roles, including `new AuthorizeAttribute{Roles}`).

```json case
{ "phase": 4, "kinds": ["security.roles", "security"], "traces": ["ESC-SVC-15"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" }, "Secured": { "Roles": "Admin, Manager" } } }
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

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization(new AuthorizeAttribute { Roles = "Manager,Admin" });

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
    { "kind": "security.roles", "element": "cancel-order", "outcome": "verified", "expected": "Admin, Manager", "actual": "Admin, Manager",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" },
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ]
}
```

## contradicted

The endpoint requires only `Admin`. A set with a role missing is a different set.

```json case
{ "phase": 4, "base": "verified-in-any-order", "kinds": ["security.roles"], "requirements": ["R1", "R10", "R14"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

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
    { "kind": "security.roles", "element": "cancel-order", "outcome": "contradicted", "expected": "Admin, Manager", "actual": "Admin",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/security.roles", "severity": "error", "subject": "cancel-order", "messageContains": "Manager" }
  ]
}
```

## contradicted-not-stated

The endpoint calls `RequireAuthorization()` with no roles. It is secured, as the model says, but it states no roles, so the modelled roles are contradicted with the code side "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified-in-any-order", "kinds": ["security.roles", "security"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization();

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
    { "kind": "security.roles", "element": "cancel-order", "outcome": "contradicted", "expected": "Admin, Manager", "actual": null, "fromSilence": false },
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured" }
  ],
  "findings": [
    { "rule": "claim/security.roles", "severity": "error", "subject": "cancel-order", "messageContains": "doesn't state" }
  ]
}
```

## unreadable

The roles are a constant from another class. The rule can't read them as a set of roles, so the claim is can't verify, `unreadable`.

```json case
{ "phase": 4, "base": "verified-in-any-order", "kinds": ["security.roles"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync).RequireAuthorization(new AuthorizeAttribute { Roles = OrderRoles.Cancellers });

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
    { "kind": "security.roles", "element": "cancel-order", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/security.roles", "severity": "error", "subject": "cancel-order", "messageContains": "can't read" }
  ]
}
```

## not-claimed-when-unset

The Operation is `Secured` with no Roles, and the endpoint requires `Manager,Admin`. No roles claim exists, and nothing is reported for roles (principle 1); the endpoint's security is still checked.

```json case
{ "phase": 4, "base": "verified-in-any-order", "kinds": ["security.roles", "security"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
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
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" }, "Secured": {} } }
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
    { "kind": "security.roles", "element": "cancel-order", "absent": true },
    { "kind": "security", "element": "cancel-order", "outcome": "verified", "expected": "Secured", "actual": "Secured" }
  ],
  "findings": [
    { "rule": "claim/security.roles", "count": 0 }
  ]
}
```
