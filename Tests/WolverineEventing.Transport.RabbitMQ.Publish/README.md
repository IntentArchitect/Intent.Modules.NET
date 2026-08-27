# WolverineEventing.Transport.RabbitMQ.Publish

Demonstrates `Intent.Eventing.Wolverine`'s **RabbitMQ transport**, publisher side — the module's Golden Sample path, modelled as a standalone publish/subscribe pair with `WolverineEventing.Transport.RabbitMQ.Subscribe`.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = RabbitMQ, Broker Topology = Auto-provision, Transactional Outbox = None.
- `CreateOrderCommand` publishes `OrderCreatedEvent` to a RabbitMQ exchange (`order-created-event`) and sends `ProcessOrderCommand` to a RabbitMQ queue (`process-order-command`), exercising both the publish-rule and send-rule paths.

## Infrastructure requirements

- A RabbitMQ broker reachable via the `Wolverine:RabbitMq` configuration section in `appsettings.json`.

## Running it

```
dotnet run --project WolverineEventing.Transport.RabbitMQ.Publish.Api
```

Run alongside `WolverineEventing.Transport.RabbitMQ.Subscribe` to see `OrderCreatedEvent` flow end to end across a process boundary — a subscriber-side discovery surface only appears once publisher and subscriber are separate processes.
