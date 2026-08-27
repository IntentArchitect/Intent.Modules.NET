# WolverineEventing.MultiTenancy

Demonstrates `Intent.Eventing.Wolverine` alongside Finbuckle multi-tenancy (`Intent.Modules.AspNetCore.MultiTenancy`) — the sole evidence for R12.

## What it demonstrates

- `CreateOrderCommand` publishes `OrderCreatedEvent` over Wolverine's Local transport, designated `Wolverine Message`.
- `WolverineTenantHeaderStrategy` reads/writes the tenant identifier header on inbound/outbound messages.
- `WolverineTenantMiddleware` establishes and restores the Finbuckle `IMultiTenantContext` around handler invocation, registered once at host scope via `opts.Policies.AddMiddleware(typeof(WolverineTenantMiddleware))` — applied to every listener/handler, never per-message.
- `WolverineFinbuckleConfiguratorExtension` wires both templates in only because Finbuckle is installed; an app without it gets neither file.

## Bugs this app found and fixed

Building this app — the first time `Intent.Eventing.Wolverine` and Finbuckle multi-tenancy were ever installed together — surfaced two real defects:

1. **`Intent.Eventing.Wolverine`**: `WolverineFinbuckleConfiguratorExtension` registered the generated (static) `WolverineTenantMiddleware` via the generic `opts.Policies.AddMiddleware<T>()`, which fails to compile (`CS0718`, static types cannot be type arguments). Fixed by switching to Wolverine's own `Type`-based overload, `AddMiddleware(typeof(T))`. Documented in the module's `CONTEXT.md` and `release-notes.md`.
2. **`Intent.Modules.AspNetCore.MultiTenancy`** (out of this spec's scope, and not part of this repo's currently-open solution): pins `Microsoft.Extensions.Hosting.Abstractions` to exactly `8.0.0` regardless of target framework, which downgrade-conflicts (`NU1605`) with `WolverineFx`'s floor of `>= 10.0.0` on a net10.0 app. Worked around with a direct `.csproj` version correction per this repo's documented NU1605 gotcha; a future fix belongs in that module's own NuGet Package model (add a net10-floor `Package Version`, the pattern already used elsewhere in this repo).

## Infrastructure requirements

None — Transport is Local (in-process), so no external broker is required to exercise the tenancy path.

## Running it

```
dotnet run --project WolverineEventing.MultiTenancy.Api
```

`POST /api/orders` (with a tenant identifier header set) raises `OrderCreatedEvent` in-process, with the Finbuckle tenant context established for the duration of the handler.
