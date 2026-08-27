# WolverineEventing.Outbox.SqlServer.Subscribe

Demonstrates `Intent.Eventing.Wolverine`'s **Durable Transactional Outbox** backed by **SQL Server**, on the subscribing side of a RabbitMQ-transported event.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = RabbitMQ, Broker Topology = Auto-provision, Transactional Outbox = Durable.
- `OrderCreatedEventHandler` subscribes to `OrderCreatedEvent`, published by `WolverineEventing.Outbox.SqlServer.Publish`.

## Infrastructure requirements

- A SQL Server instance reachable via the connection string in `appsettings.json`.
- A RabbitMQ broker reachable via the `Wolverine:RabbitMq` configuration section in `appsettings.json`.

## Running it

```
dotnet run --project WolverineEventing.Outbox.SqlServer.Subscribe.Api
```

Run alongside `WolverineEventing.Outbox.SqlServer.Publish` to see `OrderCreatedEvent` flow end to end through the durable outbox.
