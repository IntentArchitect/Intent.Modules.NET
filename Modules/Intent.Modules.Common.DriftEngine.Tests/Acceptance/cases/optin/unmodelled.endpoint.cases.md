# optin/unmodelled.endpoint

The opt-in rule that reports a Minimal API endpoint, a lambda or a method group the code maps, that no Operation's `endpoint.exists` claim located (rules.md, "Opt-in must be modelled rules"). It is named by its verb and route. A controller's actions are public methods, which [unmodelled.member](unmodelled.member.cases.md) reports, and a router or controller no Service describes is [unmodelled.type](unmodelled.type.cases.md)'s, so this rule reports only what neither can: an endpoint written as a lambda. By principle 1 it is off until the Root Folder's `Drift Check` lists it, it reads only in-scope code, and each finding can be excepted with a reason (R49).

## on

The `Orders` Service models `CancelOrder`, which `OrdersApi` maps to the method group `CancelOrderAsync`, so it is located. `GET /cardtypes` is a lambda no Operation describes: reported, named by its verb and route.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.endpoint"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "orders", "type": "Service", "name": "Orders",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.endpoint" } ] }
        } }]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapGet("/cardtypes", async (OrderServices services) => TypedResults.Ok(await services.Queries.GetCardTypesAsync()));

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
  "findings": [
    { "rule": "optin/unmodelled.endpoint", "severity": "error", "subject": "src/Ordering.API/Apis/OrdersApi.cs:10", "messageContains": "The endpoint `GET /api/orders/cardtypes` in `OrdersApi` isn't in the model." },
    { "rule": "optin/unmodelled.endpoint", "subject": "src/Ordering.API/Apis/OrdersApi.cs:9", "count": 0, "note": "Modelled, so located" }
  ]
}
```

## off-by-default

The same model and code with no `Opt-in Rules` row: nothing is reported.

```json case
{ "phase": 4, "base": "on", "cases": ["optin/unmodelled.endpoint"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "orders", "type": "Service", "name": "Orders",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.endpoint", "count": 0 }
  ]
}
```
