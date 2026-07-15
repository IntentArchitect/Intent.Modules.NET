# Intent.Modules.AspNetCore.MultiTenancy — CONTEXT

## Finbuckle.MultiTenant 6.13.1 → 9.4.10 upgrade — what has been tested

This module is the root of a cross-module upgrade: `Intent.Modules.CosmosDB`,
`Intent.Modules.Eventing.MassTransit`, `Intent.Modules.MongoDb`, and
`Intent.Modules.MongoDb.MongoFramework` all depend on the tenant-resolution shape this module
generates (`IMultiTenantContextAccessor<T>` / `IMultiTenantContextSetter` replacing pre-v7 direct
`ITenantInfo` DI). Target version is **9.4.10**, deliberately not v10 (v10 targets `net10.0` only
and removes `ITenantInfo`/splits packages — out of scope). Full design rationale is in
`.module-builder/WORKING.md`'s session history; this section is the durable summary of what was
**actually exercised**, so a future session doesn't have to re-derive it from git log.

### Verification method

Every scenario below was verified by running the Software Factory against a real `Tests/` app,
inspecting the staged diff, applying it, building, and — where marked "runtime" — actually running
the app and observing behavior (HTTP requests, logs, or a database query), not just a green build.
"Build-only" means the generated code compiles but no request was made against it.

### Coverage matrix

| Scenario | Test app | Verified | Evidence |
|---|---|---|---|
| Separate-DB, EF Core, InMemory tenant store | `Finbuckle.SeparateDatabase.TestApplication` | **Build + runtime** | SQL Server (`Server=.`). POST/GET as tenant1 visible only to tenant1; tenant2 empty. Per-tenant `TenantExtendedInfo.ConnectionString` resolution confirmed. |
| Shared-DB, EF Core, global query filter (`IsMultiTenant()`/`EnforceMultiTenant()`) | `Finbuckle.SharedDatabase.TestApplication` | **Build + runtime** | Real SQL Server. POST as tenant1; GET as tenant1 → own rows only; GET as tenant2 → no tenant1 rows. Confirms `Finbuckle.MultiTenant` (not `.EntityFrameworkCore`) is the correct namespace for `IsMultiTenant()`. |
| Multi-connection `ITenantConnections` DI (`TenantConnectionsInterfaceTemplatePartial`) | `MongoDb.MultiTenancy.SeperateDb` | **Build + SF diff verified**, runtime tenant-resolution confirmed | Generated `IMultiTenantContextAccessor<TenantExtendedInfo>` resolution correct; see MongoDb row below for the isolation caveat. |
| `EFCoreStoreDbContext<T>` store option (`MultiTenantStoreDbContextTemplatePartial`) | — | **Build-only** | No app in this repo sets `Store: EFCore` — every multitenancy app here uses `in-memory` or `http-remote`. Namespace fix (`Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore`) confirmed correct against the real 9.4.10 DLL via reflection, but unexercised end-to-end. |
| Cross-process tenant propagation via MassTransit (`FinbuckleConsumingFilter`/`Publishing`/`SendingFilter`) | `MassTransitFinbuckle.Test` | **Build + runtime** | In-memory transport. Tenant-tagged publish/consume round trip (tenant1 then tenant2, confirmed via consumer-side logs). Request/response round trip under `UseInMemoryOutbox` confirmed working. Publishing/sending is tenant-optional: `Publishing`/`SendingFilter` only set the tenant header when a tenant is resolved and never throw, so genuine no-tenant Publish/Send now sends an untagged message instead of failing fast — see this module's sibling `Intent.Modules.Eventing.MassTransit` template history. |
| CosmosDB separate-database multi-tenancy | `CosmosDB.MultiTenancy.SeperateDB` | **Build + runtime** | Cosmos DB Linux emulator (Docker, `localhost:8081`). Created customers as tenant1/tenant2 (201/201); same-tenant fetch 200; cross-tenant fetch 404 both directions; direct Cosmos REST query confirmed data landed in two separate databases, `DbTenant1`/`DbTenant2`. Required generalizing `Intent.Modules.CosmosDB` to publish a `MultitenantConnectionStringRegistrationRequest` (named `CosmosDbConnection` property on `TenantExtendedInfo`) instead of assuming it's the only separate-database module installed. |
| CosmosDB shared-container multi-tenancy | `CosmosDBMultiTenancy` | **Build + SF diff (0 changes)** | Confirmed genuinely unaffected by the `CosmosDbConnection` rename — this app uses shared-container/partition-key isolation, not separate-database, so it never referenced the renamed property. |
| MongoDb separate-database multi-tenancy | `MongoDb.MultiTenancy.SeperateDb` | **Build + runtime**, isolation confirmed | Docker `mongo:7` (`finbuckle-mongo`, `localhost:27017`, no auth). Finbuckle tenant resolution itself confirmed correct (debug-traced `ITenantConnections`). Full isolation was initially blocked by a **pre-existing, Finbuckle-unrelated** captive-dependency bug in `Intent.Modules.MongoDb.AddMongoCollection<T>` (Singleton collection capturing a Scoped `IMongoDatabase`) — fixed (Scoped registration), documented in `Modules/Intent.Modules.MongoDb/CONTEXT.md`, re-verified after the fix. |
| MongoDb.MongoFramework | *(no dedicated test app in this repo)* | **Build-only** | `NugetPackages.cs` package-ladder reconciled (removed a stray `>=10 → 10.0.2` Finbuckle arm that would have put v10 alongside this module's root-pinned 9.4.10 — a version conflict). Never exercised end-to-end; no MongoFramework-specific test app exists here. |
| `Intent.Modules.EntityFrameworkCore` package floor (`>= 8.0.28` / `>= 9.0.17`, required transitively by `Finbuckle.MultiTenant.EntityFrameworkCore` 9.4.10) | — | **Build-only** | This module is not part of the IA solution these test apps live in; fix is source-only, confirmed against the real 9.4.10 `.nuspec` dependency groups, not SF/runtime-verified against any target app in this repo. |
| Google Cloud Storage separate-account multi-tenancy | `Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests` | **Build-only** | `Intent.Modules.Google.CloudStorage` needed no changes for Finbuckle 9.4.10 *by itself* — it has no direct Finbuckle API usage and resolves tenants purely through this module's `ITenantConnections`/named-connection abstraction. It did need one fix for a *combination* issue: this test app also has an EF Core `DbContext`, and claiming its own named `GoogleCloudStorageConnection` meant `AspNetCoreIntegrationExtension`'s EF Core integration (item 5 below) could no longer assume `ConnectionString` exists — fixed in `Intent.Modules.Google.CloudStorage` itself (see that module's own `CONTEXT.md`), not here. What else broke was the app's own stale hand-written code (fixed by hand) plus the `GetDefaultTenants()` bug (item 4 below, fixed in this module). Not runtime-tested — requires developer-provided GCS credentials. |
| MinimalHostingModel (uses both MultiTenancy + MassTransit) | `MinimalHostingModel` | **Not tested** | Still on pre-upgrade module versions (`AspNetCore.MultiTenancy` `5.2.2-pre.0`, `Eventing.MassTransit` `7.1.3-pre.0`). Never updated or exercised this upgrade. |
| net9.0 / net10.0 targets | — | **Not tested** | Every test app in this repo targets `net8.0`. The `>= 8` NuGet arm (all pinned to `9.4.10`, no separate `>=10` arm) is unexercised on net9/net10 by any real app — confirmed by source/package-floor reasoning only. |

### Known-fixed bugs discovered *during* this testing (not pre-existing upstream issues)

1. **MongoDb `AddMongoCollection<T>` Singleton/Scoped captive dependency** — see
   `Modules/Intent.Modules.MongoDb/CONTEXT.md`. Pre-existing, Finbuckle-version-independent, but only
   surfaced because separate-database MongoDb multi-tenancy was exercised end-to-end for the first
   time during this upgrade's runtime verification.
2. **MassTransit tenant header is optional, not enforced** — see this module's sibling
   `Intent.Modules.Eventing.MassTransit` template history (`FinbucklePublishingFilter`/
   `FinbuckleSendingFilter`). An earlier iteration made these filters fail fast with a
   `MultiTenantException` when no tenant was resolved; that was reverted because a message may
   legitimately be published/sent without a tenant even inside a multi-tenant application (not just
   MassTransit-generated replies/faults). The filters now only set the tenant header when a tenant is
   resolved and never throw.
3. **CosmosDB assumed it was the only separate-database module installed** — fixed by generalizing
   `Intent.Modules.CosmosDB` to request a named connection-string property
   (`MultitenantConnectionStringRegistrationRequest`) instead of relying on a bare
   `TenantExtendedInfo.ConnectionString`, matching the pattern MongoDb/MongoFramework/
   GoogleCloudStorage already used.
4. **`MultiTenancyConfigurationTemplatePartial.GetDefaultTenants()` unconditionally seeded a generic
   `ConnectionString`** in the sample `InitializeStore` tenant data, even when a named connection
   request existed (or neither applied) — mismatched with `TenantExtendedInfoTemplate`, which only adds
   `ConnectionString` when there are *no* named connection requests. Surfaced when
   `Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests` failed to build (`TenantExtendedInfo` has
   no `ConnectionString`, only `GoogleCloudStorageConnection`). Fixed to mirror the same
   named-vs-generic-vs-neither condition in both places. Note: this only fixes **newly generated**
   `InitializeStore` bodies — existing ones are `Body = Ignore` (permanently hand-owned) and were
   corrected by hand in that test app instead (see its own `DependencyInjection.cs` and
   `MultiTenancyConfiguration.cs`).
5. **`AspNetCoreIntegrationExtension.GetSeparateDatabaseDataIsolationConfiguration()` (the factory extension
   that injects tenant-aware EF Core `AddDbContext` code into `Infrastructure.DependencyInjection`)
   unconditionally assumes `tenantInfo?.ConnectionString` exists** — this only fires when an app has
   **both** an EF Core `DbContext` *and* another separate-database module using the named-connection
   mechanism (Cosmos/Mongo apps in this repo don't have an EF Core `DbContext` at all, so they never hit
   it). This module deliberately does **not** know about any of its downstream consumers (CosmosDB/MongoDb/
   MongoFramework/GoogleCloudStorage) — the dependency only runs one way (they depend on this module, not
   the reverse) — so this method is intentionally left as-is. **The fix lives in
   `Intent.Modules.Google.CloudStorage`** instead, see that module's own `CONTEXT.md`/`GoogleCloudStorage
   ConfigurationTemplatePartial.cs`: it patches this statement after the fact via a late-priority (1000)
   `AfterBuild` find-and-replace on `Infrastructure.DependencyInjection`'s own generated file, since Google.
   CloudStorage already depends on this module and is the one that causes the `ConnectionString` property
   to disappear from the tenant class in the first place. If CosmosDB/MongoDb/MongoFramework are ever
   combined with EF Core in the same app and hit the same issue, the fix belongs in *that* module using the
   same pattern — never here.

### Infra used for runtime verification (all local, Docker where noted)

- SQL Server: local `Server=.` Developer instance, Windows auth (Finbuckle EF apps).
- MongoDB: Docker `mongo:7`, container `finbuckle-mongo`, `localhost:27017`, no auth.
- Cosmos DB: Docker Linux emulator, container `finbuckle-cosmos`, `localhost:8081` — emulator
  regenerates its self-signed TLS cert on certain restarts; re-trust it if `.NET`/`curl` start
  rejecting it with `UntrustedRoot` (`certutil -addstore -user Root <exported-cert>`).
- MassTransit: in-memory transport (`x.UsingInMemory(...)`) — no broker required for any scenario
  tested here.
