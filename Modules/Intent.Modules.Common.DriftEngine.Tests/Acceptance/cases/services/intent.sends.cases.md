# intent.sends

A `Send Integration Command` from an Operation, Command or handler subscription to an Integration Command, against the code: the source's handler sends a new instance of that command, `Send(new X(...))`, here through NServiceBus's `IMessageSession`. The claim is the association's. `intent.sends` is an existence kind: a modelled send the code lacks is contradicted (R8). It comes from Common.Drift's recipes alone, with no eShopCustom or Java check of its own ("conversion only").

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs`, moved onto NServiceBus.

## verified

`CreateOrderAsync` sends `ChargePaymentCommand` through the injected `IMessageSession`.

```json case
{ "phase": 4, "kinds": ["intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Ordering.API",
          "elements": [
            { "id": "orders-api", "type": "Service", "name": "OrdersApi",
              "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
              "children": [
                { "id": "create-order", "type": "Operation", "name": "CreateOrderAsync",
                  "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/" } } }
              ] }
          ]
        },
        {
          "id": "eventing", "name": "Ordering.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "charge-payment", "type": "Integration Command", "name": "ChargePaymentCommand" }
          ]
        }
      ],
      "associations": [
        { "id": "create-order-sends-charge-payment", "type": "Send Integration Command",
          "source": { "element": "create-order" }, "target": { "element": "charge-payment" } }
      ]
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

        api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession)
    {
        var orderId = Guid.NewGuid();

        await messageSession.Send(new ChargePaymentCommand(orderId, request.Total));

        return TypedResults.Ok(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "create-order-sends-charge-payment", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:18" }
  ]
}
```

## contradicted-published-not-sent

The endpoint publishes `ChargePaymentCommand` instead of sending it. A publish goes to every subscriber, a send to the command's one receiver, so the modelled send isn't in the code.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.sends"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession)
    {
        var orderId = Guid.NewGuid();

        await messageSession.Publish(new ChargePaymentCommand(orderId, request.Total));

        return TypedResults.Ok(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "create-order-sends-charge-payment", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.sends", "severity": "error", "subject": "create-order-sends-charge-payment", "messageContains": "isn't in the code" }
  ]
}
```

## no-rule

The NServiceBus pack isn't active, and nothing else reads sends, so the claim is can't verify, `noRule`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.sends"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "create-order-sends-charge-payment", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/intent.sends", "severity": "error", "subject": "create-order-sends-charge-payment", "messageContains": "no rule" }
  ]
}
```
