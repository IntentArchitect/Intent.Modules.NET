# WolverineEventing.ErrorPolicy.RetryWithCooldown

Demonstrates `Intent.Eventing.Wolverine`'s **Error Handling Policy = Retry with cooldown** (the module default): a failed message handler is retried with increasing delays between attempts before moving to the error queue.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = Local, Transactional Outbox = None, Error Handling Policy = Retry with cooldown.
- `CreateOrderCommand` publishes `OrderCreatedEvent` in-process — no external broker needed.
- The cooldown delays are configurable via `Wolverine:ErrorHandling:RetryWithCooldown:Delays` in `appsettings.json` (default `00:00:01, 00:00:05, 00:00:15`).

## Running it

```
dotnet run --project WolverineEventing.ErrorPolicy.RetryWithCooldown.Api
```

`POST /api/orders` raises `OrderCreatedEvent`. A handler failure is retried with the configured cooldown delays before moving to the error queue.
