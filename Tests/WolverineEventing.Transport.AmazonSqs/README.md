# WolverineEventing.Transport.AmazonSqs

Demonstrates `Intent.Eventing.Wolverine`'s **Amazon SQS transport**, including the module's fail-fast configuration guard.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = Amazon SQS, Broker Topology = Auto-provision, Transactional Outbox = None.
- `CreateOrderCommand` publishes `OrderCreatedEvent` to an SQS queue (`order-created-event`).
- If `Wolverine:AmazonSqs:Region` is missing from configuration, startup fails fast with an `InvalidOperationException` naming the missing key — rather than failing later, obscurely, on first publish.

## Infrastructure requirements

- An AWS account/region reachable via `Wolverine:AmazonSqs:Region` in `appsettings.json` (and standard AWS credential resolution).

## Running it

```
dotnet run --project WolverineEventing.Transport.AmazonSqs.Api
```

`POST /api/orders` raises `OrderCreatedEvent`, published to the configured SQS queue.
