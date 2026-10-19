# Context: Intent.Integration.HttpClients

## Purpose
Generates typed HttpClient implementations of service proxy contracts, plus the `HttpClientConfiguration` DI registration, and wires the selected authorization setup (none, header provider, client access token management, transmittable access token) into each client.

## Architectural Decisions
- **Client Access Token Management registers its own `IDistributedCache` (since 6.2.0).** On .NET 8+, `HttpClientHeaderConfiguratorExtension` emits Duende.AccessTokenManagement, which caches tokens in `IDistributedCache` but does not register one; without one the app fails at startup. The module adds `services.AddDistributedMemoryCache()` plus the `Microsoft.Extensions.Caching.Memory` package itself, guarded on `Intent.AspNetCore.DistributedCaching` not being installed.
  - **Rejected: a hard dependency on `Intent.AspNetCore.DistributedCaching`** (the 6.0.1 fix). That module generates `IDistributedCacheWithUnitOfWork`, which the shared UoW helpers (`Common.UnitOfWork` `PersistenceUnitOfWork`) treat as a persistence store. Every dispatcher then got `EnableUnitOfWork`/`SaveChangesAsync` wrapping, and `UnitOfWorkBehaviour` was generated in apps with no database. **Do not reintroduce this dependency.**
  - **Why the guard:** when DistributedCaching is installed it emits its own registration (memory or Redis). The guard avoids a duplicate line, and means existing apps that keep that module see no diff. Coexistence would be safe even without the guard, because `AddDistributedMemoryCache` uses `TryAdd` and Redis uses `Add`.
  - **Rejected for now: Duende 4.x / `HybridCache`.** It is deferred because it means a package major-version move.
- **The pre-.NET 8 IdentityModel.AspNetCore path is deliberately untouched.** That library manages its own token cache, and it worked before 6.0.1. Adding Caching.Memory there would also risk netstandard2.1 package issues.

## Invariants & Constraints
- The NuGet dependency and the statement are added inside the `HttpClientConfiguration` `OnBuild` callback (priority 1000), following the existing Duende pattern. If the package ever stops appearing in generated `.csproj` files, move both registrations into the HttpClientConfiguration template constructor.
- `Microsoft.Extensions.Caching.Memory` versions are modelled in the Module Builder designer to match `Intent.AspNetCore.DistributedCaching`'s. Keep them aligned to avoid NU1605 downgrade conflicts when both modules are installed.

## Module Interactions
- **Intent.AspNetCore.DistributedCaching** — optional. Detected through `application.InstalledModules`, with no type references. When installed, it owns the `IDistributedCache` registration.
- **Intent.Common.UnitOfWork** — interoperability install only. HttpClients must not pull in anything that UoW treats as a persistence store.
