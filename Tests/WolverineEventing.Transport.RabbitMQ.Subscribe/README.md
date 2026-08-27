# WolverineEventing.Transport.RabbitMQ.Subscribe

Demonstrates `Intent.Eventing.Wolverine`'s **RabbitMQ transport**, subscriber side — paired with `WolverineEventing.Transport.RabbitMQ.Publish`.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = RabbitMQ, Broker Topology = Auto-provision, Transactional Outbox = None.
- `OrderCreatedEventHandler` subscribes to `OrderCreatedEvent`, bound from the `order-created-event` exchange to this app's own Subscriber Queue (`wolverine-eventing.transport.rabbit-mq.subscribe-order-created-event`, the derived naming convention — never a module setting).
- Also listens directly to the `process-order-command` queue for `ProcessOrderCommand`, sent (not published) by the Publish app.

## Infrastructure requirements

- A RabbitMQ broker reachable via the `Wolverine:RabbitMq` configuration section in `appsettings.json`.

## Running it

```
dotnet run --project WolverineEventing.Transport.RabbitMQ.Subscribe.Api
```

Run alongside `WolverineEventing.Transport.RabbitMQ.Publish` to see `OrderCreatedEvent` and `ProcessOrderCommand` flow end to end.
