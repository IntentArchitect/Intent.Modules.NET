# WolverineEventing.ErrorPolicy.Retry

Demonstrates `Intent.Eventing.Wolverine`'s **Error Handling Policy = Retry**: a failed message handler is retried a fixed number of times (immediate retries, no cooldown) before moving to the error queue.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = Local, Transactional Outbox = None, Error Handling Policy = Retry.
- `CreateOrderCommand` publishes `OrderCreatedEvent` in-process — no external broker needed.
- The retry attempt count is configurable via `Wolverine:ErrorHandling:Retry:Attempts` in `appsettings.json` (default 3).

## Running it

```
dotnet run --project WolverineEventing.ErrorPolicy.Retry.Api
```

`POST /api/orders` raises `OrderCreatedEvent`. A handler failure is retried immediately up to the configured attempt count before moving to the error queue.
