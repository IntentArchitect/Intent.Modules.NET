# WolverineEventing.Transport.AzureServiceBus

Demonstrates `Intent.Eventing.Wolverine`'s **Azure Service Bus transport**, including the module's fail-fast configuration guard.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = Azure Service Bus, Broker Topology = Auto-provision, Transactional Outbox = None.
- `CreateOrderCommand` publishes `OrderCreatedEvent` to an Azure Service Bus topic (`order-created-event`).
- If `Wolverine:AzureServiceBus:ConnectionString` is missing from configuration, startup fails fast with an `InvalidOperationException` naming the missing key — rather than failing later, obscurely, on first publish.

## Infrastructure requirements

- An Azure Service Bus namespace reachable via `Wolverine:AzureServiceBus:ConnectionString` in `appsettings.json`.

## Running it

```
dotnet run --project WolverineEventing.Transport.AzureServiceBus.Api
```

`POST /api/orders` raises `OrderCreatedEvent`, published to the configured Azure Service Bus topic.
