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

## verified-local-from-out-argument

The endpoint sends a local that an `out` argument declares, `router.Prepare(..., out ChargePaymentCommand command)`, and no in-scope code implements the router. The argument types the local as the command, so the send is verified.

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
        router.Prepare(orderId, request.Total, out ChargePaymentCommand command);

        await messageSession.Send(command);

        return TypedResults.Ok(orderId);
    }
}
```

```csharp file src/Ordering.API/Apis/IPaymentRouter.cs
namespace Ordering.API.Apis;

public interface IPaymentRouter
{
    void Prepare(Guid orderId, decimal total, out ChargePaymentCommand command);

    bool TryRefund(Guid orderId, decimal total, out RefundPaymentCommand command);
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

## contradicted-local-from-out-argument

The endpoint sends a local that `out var` declares, `router.TryRefund(..., out var command)`. The router's parameter types it as `RefundPaymentCommand`, another command, so the modelled send isn't in the code.

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
        router.TryRefund(orderId, request.Total, out var command);

        await messageSession.Send(command);

        return TypedResults.Ok(orderId);
    }
}
```

```csharp file src/Ordering.API/Apis/IPaymentRouter.cs
namespace Ordering.API.Apis;

public interface IPaymentRouter
{
    void Prepare(Guid orderId, decimal total, out ChargePaymentCommand command);

    bool TryRefund(Guid orderId, decimal total, out RefundPaymentCommand command);
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

## verified-own-parameter-handed-to-logic

The endpoint takes the command as its request and hands it to the logic behind an interface, whose class sends it with `SendLocal(command)`, as one of ClientB's controllers does through its logic interface. The command comes from outside the code, so the endpoint's own parameter is the message it hands on, and the call that leads to the send is where it does: verified.

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

    public static async Task<Ok<Guid>> CreateOrderAsync(ChargePaymentCommand command, IOrderLogic logic)
    {
        await logic.SubmitAsync(command);

        return TypedResults.Ok(Guid.NewGuid());
    }
}
```

```csharp file src/Ordering.API/Apis/OrderLogic.cs
namespace Ordering.API.Apis;

public interface IOrderLogic
{
    Task SubmitAsync(ChargePaymentCommand command);
}

public class OrderLogic(IMessageSession messageSession) : IOrderLogic
{
    public async Task SubmitAsync(ChargePaymentCommand command)
    {
        await messageSession.SendLocal(command);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "create-order-sends-charge-payment", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:16" }
  ]
}
```

## contradicted-own-parameter-not-handed-on

The logic now only reads the command it is given and sends nothing, so the endpoint's parameter is never handed on: its type alone isn't a send, and the modelled send isn't in the code.

```json case
{ "phase": 4, "base": "verified-own-parameter-handed-to-logic", "kinds": ["intent.sends"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrderLogic.cs
namespace Ordering.API.Apis;

public interface IOrderLogic
{
    Task SubmitAsync(ChargePaymentCommand command);
}

public class OrderLogic : IOrderLogic
{
    public decimal LastTotal { get; private set; }

    public Task SubmitAsync(ChargePaymentCommand command)
    {
        LastTotal = command.Total;
        return Task.CompletedTask;
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

## verified-in-callback-library-makes

A hosted service's `ExecuteAsync` runs a file sync from a library and hands it the service itself as its progress, `sync.SyncDirectory(..., this)`. The library reports each file through the service's explicit `IProgress<FileSyncProgress>.Report`, and that sends `ImportPrices` for it. A library calls back what it is handed only through a type it knows, so the service's explicit implementation of a library interface runs from that call: the send is verified, where `ExecuteAsync` calls the method that hands the service over.

```json case
{ "phase": 4, "kinds": ["intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Prices.Host",
          "elements": [
            { "id": "price-file-downloads", "type": "Service", "name": "PriceFileDownloads",
              "children": [
                { "id": "execute", "type": "Operation", "name": "ExecuteAsync" }
              ] }
          ]
        },
        {
          "id": "eventing", "name": "Prices.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "import-prices", "type": "Integration Command", "name": "ImportPrices" }
          ]
        }
      ],
      "associations": [
        { "id": "execute-sends-import-prices", "type": "Send Integration Command",
          "source": { "element": "execute" }, "target": { "element": "import-prices" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Prices", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Prices.Host/PriceFileDownloads.cs
namespace Prices.Host;

public class PriceFileDownloads(IMessageSession session) : IProgress<FileSyncProgress>
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => Sync(cancellationToken));
    }

    private void Sync(CancellationToken cancellationToken)
    {
        var sync = new FileSync();
        sync.SyncDirectory(new Uri("ftp://prices/daily"), "C:/prices", cancellationToken, this);
    }

    void IProgress<FileSyncProgress>.Report(FileSyncProgress value)
    {
        Task.Run(async () => await OnFileDownloaded(value.LocalFileName));
    }

    private async Task OnFileDownloaded(string fileName)
    {
        await session.SendLocal(new ImportPrices { FileName = fileName });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "execute-sends-import-prices", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Prices.Host/PriceFileDownloads.cs:7" }
  ]
}
```

## contradicted-callback-never-handed-over

The service now hands the sync no progress of its own (`null`), so nothing calls its `Report`: the send there isn't `ExecuteAsync`'s, and the modelled send isn't in the code.

```json case
{ "phase": 4, "base": "verified-in-callback-library-makes", "kinds": ["intent.sends"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Prices.Host/PriceFileDownloads.cs
namespace Prices.Host;

public class PriceFileDownloads(IMessageSession session) : IProgress<FileSyncProgress>
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => Sync(cancellationToken));
    }

    private void Sync(CancellationToken cancellationToken)
    {
        var sync = new FileSync();
        sync.SyncDirectory(new Uri("ftp://prices/daily"), "C:/prices", cancellationToken, null);
    }

    void IProgress<FileSyncProgress>.Report(FileSyncProgress value)
    {
        Task.Run(async () => await OnFileDownloaded(value.LocalFileName));
    }

    private async Task OnFileDownloaded(string fileName)
    {
        await session.SendLocal(new ImportPrices { FileName = fileName });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "execute-sends-import-prices", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.sends", "severity": "error", "subject": "execute-sends-import-prices", "messageContains": "isn't in the code" }
  ]
}
```

## verified-library-message-of-an-own-commands-name

The application owns and models `PlaceOrder`, which `OrderHandler` handles, sending `ChargePayment`. It also keeps an old, unwired handler of a contracts package's `PlaceOrder`, which sends `ChargePayment` too. That handler's file imports the package's namespace and not the application's commands namespace, and its project adds no imports of its own, so its `PlaceOrder` is the package's: it is no handler of the modelled command, and its send is nothing the diagram can show. A migrated NServiceBus application, clean under Common.Drift v1, had two sends reported, each asking for the old handler to be modelled as a handler of the application's own command.

```json case
{ "phase": 4, "kinds": ["intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Shop.Pricing",
          "elements": [
            { "id": "order-handler", "type": "Integration Event Handler", "name": "OrderHandler" }
          ]
        },
        {
          "id": "eventing", "name": "Shop.Pricing.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "place-order", "type": "Integration Command", "name": "PlaceOrder" },
            { "id": "charge-payment", "type": "Integration Command", "name": "ChargePayment" }
          ]
        }
      ],
      "associations": [
        { "id": "handler-subscribes-place-order", "type": "Subscribe Integration Command",
          "source": { "element": "order-handler" }, "target": { "element": "place-order" } },
        { "id": "subscription-sends-charge-payment", "type": "Send Integration Command",
          "source": { "element": "handler-subscribes-place-order" }, "target": { "element": "charge-payment" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Shop.Pricing/Shop.Pricing.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```csharp file src/Shop.Pricing/Commands/PlaceOrder.cs
using NServiceBus;

namespace Shop.Pricing.Commands;

public class PlaceOrder : ICommand
{
    public string OrderCode { get; set; }
}
```

```csharp file src/Shop.Pricing/Commands/ChargePayment.cs
using NServiceBus;

namespace Shop.Pricing.Commands;

public class ChargePayment : ICommand
{
    public string OrderCode { get; set; }
}
```

```csharp file src/Shop.Pricing/Orders/OrderHandler.cs
using NServiceBus;
using Shop.Pricing.Commands;

namespace Shop.Pricing.Orders;

public class OrderHandler : IHandleMessages<PlaceOrder>
{
    public Task Handle(PlaceOrder message, IMessageHandlerContext context) =>
        context.Send(new ChargePayment { OrderCode = message.OrderCode });
}
```

```csharp file src/Shop.Pricing/Orders/LegacyOrderHandler.cs
using NServiceBus;
using Shop.Contracts.Orders;

namespace Shop.Pricing.Orders;

public class LegacyOrderHandler
{
    public Task HandleImplementationAsync(PlaceOrder message, IMessageHandlerContext context) =>
        context.Send(new Shop.Pricing.Commands.ChargePayment { OrderCode = message.OrderCode });
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "subscription-sends-charge-payment", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Shop.Pricing/Orders/OrderHandler.cs:9" }
  ],
  "findings": [
    { "rule": "messaging/unmodelled.send", "count": 0 }
  ]
}
```

## unmodelled-handler-of-an-own-command

The old handler's file now imports the application's commands namespace, so its `PlaceOrder` is the modelled command: it is a handler of it the diagram doesn't show, and its send is drift.

```json case
{ "phase": 4, "base": "verified-library-message-of-an-own-commands-name", "kinds": ["intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Shop.Pricing/Orders/LegacyOrderHandler.cs
using NServiceBus;
using Shop.Pricing.Commands;

namespace Shop.Pricing.Orders;

public class LegacyOrderHandler
{
    public Task HandleImplementationAsync(PlaceOrder message, IMessageHandlerContext context) =>
        context.Send(new ChargePayment { OrderCode = message.OrderCode });
}
```

```json expect
{
  "claims": [
    { "kind": "intent.sends", "element": "subscription-sends-charge-payment", "outcome": "verified", "expected": "present", "actual": "present" }
  ],
  "findings": [
    { "rule": "messaging/unmodelled.send", "severity": "error", "messageContains": "Model `LegacyOrderHandler` as a handler of `PlaceOrder`, then add a Send Integration Command link to `ChargePayment` from its subscription." }
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
