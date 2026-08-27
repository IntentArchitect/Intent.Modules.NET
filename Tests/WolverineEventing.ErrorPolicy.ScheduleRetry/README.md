# WolverineEventing.ErrorPolicy.ScheduleRetry

Demonstrates `Intent.Eventing.Wolverine`'s **Error Handling Policy = Schedule retry**: a failed message handler is rescheduled for later delivery attempts (longer, coarser-grained delays than Retry with cooldown) before moving to the error queue.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = Local, Transactional Outbox = None, Error Handling Policy = Schedule retry.
- `CreateOrderCommand` publishes `OrderCreatedEvent` in-process — no external broker needed.
- The schedule delays are configurable via `Wolverine:ErrorHandling:ScheduleRetry:Delays` in `appsettings.json` (default `00:01:00, 00:05:00, 00:15:00`).

## Running it

```
dotnet run --project WolverineEventing.ErrorPolicy.ScheduleRetry.Api
```

`POST /api/orders` raises `OrderCreatedEvent`. A handler failure is rescheduled at the configured delays before moving to the error queue.
