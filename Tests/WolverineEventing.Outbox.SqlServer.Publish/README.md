# WolverineEventing.Outbox.SqlServer.Publish

Demonstrates `Intent.Eventing.Wolverine`'s **Durable Transactional Outbox** backed by **SQL Server**, on the publishing side of a RabbitMQ-transported event.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = RabbitMQ, Broker Topology = Auto-provision, Transactional Outbox = Durable.
- Durable storage technology is derived from the modelled Database Provider (SQL Server here) — there is no separate module setting for it.
- `CreateOrderCommand` publishes `OrderCreatedEvent` through the outbox so the message and the database transaction that raises it commit atomically.

## Infrastructure requirements

- A SQL Server instance reachable via the connection string in `appsettings.json`.
- A RabbitMQ broker reachable via the `Wolverine:RabbitMq` configuration section in `appsettings.json`.

## Running it

```
dotnet run --project WolverineEventing.Outbox.SqlServer.Publish.Api
```

`POST /api/orders` raises `OrderCreatedEvent`, durably stored in SQL Server until Wolverine's outbox flushes it to RabbitMQ.
