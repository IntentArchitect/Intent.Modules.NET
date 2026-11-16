# type.registered

The type is wired into the application: a hosted service registered with `AddHostedService<T>()`, a gRPC service mapped with `MapGrpcService<T>()`, an integration event subscription wired with `AddSubscription<TEvent, THandler>()`. It is an existence kind with a `later` phase: no phase gate needs it yet, and no stock pack reads it. These cases show what eShopCustom's Services module checked, stated by its house pack. The registration kind (or, for a subscription, the event) is the claim's discriminator, so a type can be registered in several ways.

One claim reader of the house's module (`house.readers`) reads every registration: the vocabulary binds `Registration` to the call as C#'s canonical text writes it (no whitespace between tokens), and the reader finds the call whose text contains it (R34, R46). In a chain such as `AddRabbitMqEventBus(...).AddSubscription<...>()`, the outer call's text holds the inner's too, so the shortest is the registration.

## hosted-service-registered

`GracePeriodManagerService` is a Hosted Service, and the order processor registers it.

```json case
{ "phase": "later", "kinds": ["type.registered"], "traces": ["ESC-SVC-71"], "requirements": ["R2", "R28", "R34", "R46"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "type.registered",
          "for": { "element": "Service", "where": "hasStereotype('Hosted Service')" },
          "discriminator": "'AddHostedService'",
          "bindings": { "Type": "name", "Registration": "'AddHostedService<' + name + '>'" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Hosted Service stereotype",
          "fixHint": "Register the service with AddHostedService."
        },
        {
          "kind": "type.registered",
          "for": { "element": "Service", "where": "hasStereotype('gRPC Service')" },
          "discriminator": "'MapGrpcService'",
          "bindings": { "Type": "name", "Registration": "'MapGrpcService<' + name + '>'" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the gRPC Service stereotype",
          "fixHint": "Map the service with MapGrpcService."
        },
        {
          "kind": "type.registered",
          "for": { "element": "Integration Event Handler" },
          "each": "associations('Subscribe Integration Event')",
          "discriminator": "item.element.name",
          "bindings": { "Type": "name", "Registration": "'AddSubscription<' + item.element.name + ',' + name + '>'" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the handler's Subscribe Integration Event association",
          "fixHint": "Wire the subscription with AddSubscription."
        }
      ]
    }
  },
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["src/**"], "note": "eShopCustom: a Service with no interface is described by its class" },
        "Integration Event Handler": { "typeKinds": ["class"] }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "OrderProcessor",
        "elements": [
          { "id": "grace-period", "type": "Service", "name": "GracePeriodManagerService",
            "stereotypes": { "Hosted Service": {} } }
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

```csharp file src/OrderProcessor/Services/GracePeriodManagerService.cs
namespace eShop.OrderProcessor.Services;

public class GracePeriodManagerService(ILogger<GracePeriodManagerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("GracePeriodManagerService is starting.");
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
    }
}
```

```csharp file src/OrderProcessor/Extensions/Extensions.cs
namespace eShop.OrderProcessor.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<BackgroundTaskOptions>()
            .BindConfiguration(nameof(BackgroundTaskOptions));

        builder.Services.AddHostedService<GracePeriodManagerService>();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.registered", "element": "grace-period", "key": "AddHostedService", "outcome": "verified",
      "at": "src/OrderProcessor/Extensions/Extensions.cs:10" }
  ]
}
```

## hosted-service-unregistered

The registration was removed. The class is still there, so `type.exists` is verified, and the registration it needs is missing: contradicted (R8).

```json case
{ "phase": "later", "base": "hosted-service-registered", "kinds": ["type.exists", "type.registered"], "traces": ["ESC-SVC-71"], "requirements": ["R1", "R8", "R11"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```csharp file src/OrderProcessor/Extensions/Extensions.cs
namespace eShop.OrderProcessor.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<BackgroundTaskOptions>()
            .BindConfiguration(nameof(BackgroundTaskOptions));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "grace-period", "outcome": "verified" },
    { "kind": "type.registered", "element": "grace-period", "key": "AddHostedService", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.registered", "severity": "error", "subject": "grace-period", "messageContains": "GracePeriodManagerService" }
  ]
}
```

## grpc-mapped

`BasketService` is a gRPC Service, and the basket API maps it.

```json case
{ "phase": "later", "base": "hosted-service-registered", "without": ["src/OrderProcessor/Services/GracePeriodManagerService.cs", "src/OrderProcessor/Extensions/Extensions.cs"], "kinds": ["type.registered"], "traces": ["ESC-SVC-77"], "requirements": ["R2", "R46"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-grpc", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Basket.API/Proto/basket.proto", "Proto Service": "Basket" } } }
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

```csharp file src/Basket.API/Grpc/BasketService.cs
using eShop.Basket.API.Repositories;

namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    private readonly IBasketRepository _repository = repository;
}
```

```csharp file src/Basket.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<BasketService>();

app.Run();
```

```json expect
{
  "claims": [
    { "kind": "type.registered", "element": "basket-grpc", "key": "MapGrpcService", "outcome": "verified",
      "at": "src/Basket.API/Program.cs:8" }
  ]
}
```

## subscription-wired

The basket API's handler subscribes to `OrderStartedIntegrationEvent`, and the event bus wires that subscription. One claim per subscribed event, keyed by the event's name.

```json case
{ "phase": "later", "base": "hosted-service-registered", "without": ["src/OrderProcessor/Services/GracePeriodManagerService.cs", "src/OrderProcessor/Extensions/Extensions.cs"], "kinds": ["type.registered"], "traces": ["ESC-SVC-62"], "requirements": ["R2", "R30", "R46"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "order-started", "type": "Message", "name": "OrderStartedIntegrationEvent" },
          { "id": "order-started-handler", "type": "Integration Event Handler", "name": "OrderStartedIntegrationEventHandler" }
        ]
      }],
      "associations": [
        { "id": "order-started-subscription", "type": "Subscribe Integration Event",
          "source": { "element": "order-started-handler" },
          "target": { "element": "order-started" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Basket.API/IntegrationEvents/EventHandling/OrderStartedIntegrationEventHandler.cs
namespace eShop.Basket.API.IntegrationEvents.EventHandling;

public class OrderStartedIntegrationEventHandler(IBasketRepository repository) : IIntegrationEventHandler<OrderStartedIntegrationEvent>
{
    public async Task Handle(OrderStartedIntegrationEvent @event)
    {
        await repository.DeleteBasketAsync(@event.UserId);
    }
}
```

```csharp file src/Basket.API/Extensions/Extensions.cs
namespace eShop.Basket.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddRabbitMqEventBus("eventbus")
               .AddSubscription<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.registered", "element": "order-started-handler", "key": "OrderStartedIntegrationEvent", "outcome": "verified" }
  ]
}
```

## no-rule

Until a pack reads registrations, a registration the model states is still a claim, and it can't be verified: `noRule`, reported as an error rather than skipped (P2). Here the house pack states the claim and has no rule for it.

```json case
{ "phase": 2, "kinds": ["type.registered"], "traces": ["ESC-SVC-71"], "requirements": ["R1", "R5", "R45"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "type.registered",
          "for": { "element": "Service", "where": "hasStereotype('Hosted Service')" },
          "discriminator": "'AddHostedService'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "source": "the Hosted Service stereotype"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "OrderProcessor",
        "elements": [
          { "id": "grace-period", "type": "Service", "name": "GracePeriodManagerService",
            "stereotypes": { "Hosted Service": {} } }
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

```csharp file src/OrderProcessor/Extensions/Extensions.cs
namespace eShop.OrderProcessor.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHostedService<GracePeriodManagerService>();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.registered", "element": "grace-period", "key": "AddHostedService", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/type.registered", "severity": "error", "subject": "grace-period", "messageContains": "no rule" }
  ]
}
```
