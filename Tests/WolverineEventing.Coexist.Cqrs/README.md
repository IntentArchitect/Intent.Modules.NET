# WolverineEventing.Coexist.Cqrs

Demonstrates `Intent.Eventing.Wolverine` coexisting with `Intent.Application.Wolverine` (Wolverine CQRS dispatch) on **one shared Wolverine host**.

## What it demonstrates

- `CreateOrderCommand` is dispatched as a Wolverine CQRS command (via `Intent.Application.Wolverine`'s "Wolverine Dispatch" and `AspNetCore.Controllers.Dispatch.Wolverine`), handled by `CreateOrderCommandHandler`.
- The same handler publishes `OrderCreatedEvent` as a Wolverine-designated Integration Event (via `Intent.Eventing.Wolverine`), Transport = Local.
- Both modules' host contributions are combined into a **single** `builder.Host.UseWolverine(opts => { ... })` lambda in `Program.cs` — `WolverineConfiguration.Configure(opts)` (CQRS discovery/middleware) followed by `WolverineEventingConfiguration.ConfigureLocal(opts, builder.Configuration)` (eventing), via the shared `Intent.Wolverine.Common` host-registration broadcast mechanism. No `lambdaBlock.Statements.Clear()` collision between the two modules' contributions.
- Requires `Intent.Application.Wolverine` at `1.1.0-pre.0`+ (the version that removed the `Statements.Clear()` call) and `Intent.Wolverine.Common` installed explicitly alongside `Intent.Eventing.Wolverine` — see the "Known limitation" note below.

## Known limitation exercised by this app

`Intent.Eventing.Wolverine` does not yet formally declare a module dependency on `Intent.Wolverine.Common` in its `.imodspec` (same for `Intent.Application.Wolverine`) — a deferred packaging gap. Installing either module alone will fail Software Factory with a `FileNotFoundException` for `Intent.Modules.Wolverine.Common`. Install `Intent.Wolverine.Common` explicitly alongside them until that gap is closed.

## Running it

```
dotnet run --project WolverineEventing.Coexist.Cqrs.Api
```

`POST /api/orders` dispatches `CreateOrderCommand` through the Wolverine CQRS pipeline and publishes `OrderCreatedEvent` in-process — both paths served by the one Wolverine host.
