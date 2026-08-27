# WolverineEventing.ErrorPolicy.None

Demonstrates `Intent.Eventing.Wolverine`'s **Error Handling Policy = None**: a failed message handler is moved straight to the error queue with no retry.

## What it demonstrates

- `Wolverine Message Bus Settings`: Transport = Local, Transactional Outbox = None, Error Handling Policy = None.
- `CreateOrderCommand` publishes `OrderCreatedEvent` in-process — no external broker needed.

## Running it

```
dotnet run --project WolverineEventing.ErrorPolicy.None.Api
```

`POST /api/orders` raises `OrderCreatedEvent`. A handler failure moves the message directly to the error queue, with no retry attempts.
