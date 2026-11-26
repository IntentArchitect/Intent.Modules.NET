# Context: Intent.AspNetCore.DistributedCaching

## Purpose
Registers an `IDistributedCache` provider (in-memory or StackExchange Redis) and generates
`IDistributedCacheWithUnitOfWork` / `DistributedCacheWithUnitOfWork`, a wrapper that can defer cache
writes until a unit of work commits, plus typed `Get`/`Set` extension methods over it.

## Invariants & Constraints
- **A deferred (queued) cache call must capture every argument of the immediate call, `options` included.**
  Inside `EnableUnitOfWork()`, `Set`/`SetAsync`/`Refresh*`/`Remove*` enqueue a lambda that runs on
  `SaveChangesAsync()`. `Microsoft.Extensions.Caching.Abstractions` has convenience overloads
  (`SetAsync(key, value, token)`) that compile without `options` and silently substitute an empty
  `DistributedCacheEntryOptions` (no expiry). Up to 1.1.0 both queued `SetAsync` calls hit that overload,
  so entries written inside a unit of work never expired; fixed in 1.1.1. Nothing at compile time catches
  this, so the behaviour is guarded by the regression tests in the
  `Tests/EfCore.SecondLevelCaching` test app (`EfCore.SecondLevelCaching.Application.UnitTests/Caching`).
- Queued writes are always flushed through the async API, so a synchronous `Set` inside a unit of work
  reaches the cache as `SetAsync`. This is by design (the queue holds `Func<CancellationToken, Task>`).

## Module Interactions
- **Unit of work behaviours** — the shared `PersistenceUnitOfWork` code (`Intent.Modules.Common.UnitOfWork`,
  `Intent.Modules.UnitOfWork.Persistence.Shared`) that the unit of work behaviours are built from looks up
  this module's templates by id, and when present wraps every command in `EnableUnitOfWork()` … `SaveChangesAsync()`. So the deferred path is the normal
  path for cache writes made in command handlers, not an opt-in edge case, and any defect in it affects
  every consumer that has both modules installed.
