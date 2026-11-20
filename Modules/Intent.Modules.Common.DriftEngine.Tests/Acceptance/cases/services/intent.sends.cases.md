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

## verified-built-by-own-factory

The endpoint sends what the code's own `PaymentCommands.ChargeAsync` returns once awaited, and that method returns a new `ChargePaymentCommand`. The reading follows the awaited call into the method, so the send is verified, located at the send.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
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

        await messageSession.Send(await PaymentCommands.ChargeAsync(orderId, request.Total));

        return TypedResults.Ok(orderId);
    }
}
```

```csharp file src/Ordering.API/Apis/PaymentCommands.cs
namespace Ordering.API.Apis;

public static class PaymentCommands
{
    public static async Task<ChargePaymentCommand> ChargeAsync(Guid orderId, decimal total)
    {
        await Task.Yield();
        return new ChargePaymentCommand(orderId, total);
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

## verified-local-from-awaited-call

The endpoint sends a local it is given by awaiting an injected router, `var command = await router.NextAsync(...).ConfigureAwait(false)`, and no in-scope code implements the router, so nothing shows the command being created. The code types the local as the router's `Task<ChargePaymentCommand>` gives it, and a value typed as the message is the message, so the send is verified.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
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

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession, IPaymentRouter router)
    {
        var orderId = Guid.NewGuid();
        var command = await router.NextAsync(orderId, request.Total).ConfigureAwait(false);

        await messageSession.Send(command);

        return TypedResults.Ok(orderId);
    }
}
```

```csharp file src/Ordering.API/Apis/IPaymentRouter.cs
namespace Ordering.API.Apis;

public interface IPaymentRouter
{
    Task<ChargePaymentCommand> NextAsync(Guid orderId, decimal total);

    Task<RefundPaymentCommand> RefundAsync(Guid orderId, decimal total);

    Task<PaymentOutputs> PrepareAsync(Guid orderId, decimal total);
}

public class PaymentOutputs
{
    public ChargePaymentCommand Charge { get; set; } = null!;

    public RefundPaymentCommand Refund { get; set; } = null!;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "create-order-sends-charge-payment", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:19" }
  ]
}
```

## contradicted-local-from-awaited-call

The router now gives a `RefundPaymentCommand`, so the local the endpoint sends is typed as another command and the modelled send isn't in the code.

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

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession, IPaymentRouter router)
    {
        var orderId = Guid.NewGuid();
        var command = await router.RefundAsync(orderId, request.Total).ConfigureAwait(false);

        await messageSession.Send(command);

        return TypedResults.Ok(orderId);
    }
}
```

```csharp file src/Ordering.API/Apis/IPaymentRouter.cs
namespace Ordering.API.Apis;

public interface IPaymentRouter
{
    Task<ChargePaymentCommand> NextAsync(Guid orderId, decimal total);

    Task<RefundPaymentCommand> RefundAsync(Guid orderId, decimal total);

    Task<PaymentOutputs> PrepareAsync(Guid orderId, decimal total);
}

public class PaymentOutputs
{
    public ChargePaymentCommand Charge { get; set; } = null!;

    public RefundPaymentCommand Refund { get; set; } = null!;
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

## verified-property-of-returned-object

The endpoint sends a property of what the router returns, `outputs.Charge`, which `PaymentOutputs` declares as a `ChargePaymentCommand`: the send is verified.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
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

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession, IPaymentRouter router)
    {
        var orderId = Guid.NewGuid();
        var outputs = await router.PrepareAsync(orderId, request.Total);

        await messageSession.Send(outputs.Charge);

        return TypedResults.Ok(orderId);
    }
}
```

```csharp file src/Ordering.API/Apis/IPaymentRouter.cs
namespace Ordering.API.Apis;

public interface IPaymentRouter
{
    Task<ChargePaymentCommand> NextAsync(Guid orderId, decimal total);

    Task<RefundPaymentCommand> RefundAsync(Guid orderId, decimal total);

    Task<PaymentOutputs> PrepareAsync(Guid orderId, decimal total);
}

public class PaymentOutputs
{
    public ChargePaymentCommand Charge { get; set; } = null!;

    public RefundPaymentCommand Refund { get; set; } = null!;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "create-order-sends-charge-payment", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:19" }
  ]
}
```

## contradicted-property-of-returned-object

The endpoint now sends `outputs.Refund`, a `RefundPaymentCommand`: the modelled send isn't in the code.

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

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession, IPaymentRouter router)
    {
        var orderId = Guid.NewGuid();
        var outputs = await router.PrepareAsync(orderId, request.Total);

        await messageSession.Send(outputs.Refund);

        return TypedResults.Ok(orderId);
    }
}
```

```csharp file src/Ordering.API/Apis/IPaymentRouter.cs
namespace Ordering.API.Apis;

public interface IPaymentRouter
{
    Task<ChargePaymentCommand> NextAsync(Guid orderId, decimal total);

    Task<RefundPaymentCommand> RefundAsync(Guid orderId, decimal total);

    Task<PaymentOutputs> PrepareAsync(Guid orderId, decimal total);
}

public class PaymentOutputs
{
    public ChargePaymentCommand Charge { get; set; } = null!;

    public RefundPaymentCommand Refund { get; set; } = null!;
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

No framework's extension is active. The messaging extension, which states the send's claim and reads it, comes with each, so nothing claims the link at all: the run reports it as modelled and unchecked (R7) rather than leaving it out silently.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.sends"], "cases": ["engine/noVocabulary"], "requirements": ["R7"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json expect
{
  "findings": [
    { "rule": "engine/noVocabulary", "severity": "error", "subject": "create-order-sends-charge-payment", "messageContains": "Send Integration Command" },
    { "rule": "claim/intent.sends", "count": 0 }
  ]
}
```

## unreadable

The endpoint sends a command a factory method built. A send is there, but the reader can't tell which command it is, so the claim is can't verify, `unreadable`: neither verified nor reported missing.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.sends"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
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
        var command = PaymentCommands.Charge(orderId, request.Total);

        await messageSession.Send(command);

        return TypedResults.Ok(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "create-order-sends-charge-payment", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:19" }
  ],
  "findings": [
    { "rule": "claim/intent.sends", "severity": "error", "subject": "create-order-sends-charge-payment", "messageContains": "can't read" }
  ]
}
```
