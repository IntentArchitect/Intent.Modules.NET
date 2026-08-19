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
| MinimalHostingModel (uses both MultiTenancy + MassTransit) | `MinimalHostingModel` | **Build + SF diff verified** | Only app in this repo still carrying the genuine pre-upgrade `Body = Ignore` pattern; used as the live test case for the `ITemplateMigration` in item 6 below (separate-database scenario — retarget path, not the strip path). |
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
   corrected by hand in that test app at the time (in its `DependencyInjection.cs` and
   `MultiTenancyConfiguration.cs`), including a temporary `Body = Mode.Fully` override on
   `InitializeStore` to force it to regenerate. That hand-fix (and the `Body = Mode.Fully` override) is
   now superseded by item 6's `ITemplateMigration`, which was verified end-to-end against this exact
   app's original stale state — `InitializeStore` is back to its normal `Body = Mode.Ignore`.
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
6. **Pre-upgrade apps with a hand-locked `InitializeStore()` referencing `new TenantInfo() { ...,
   ConnectionString = "..." }` fail to compile** (`CS0117: 'TenantInfo' does not contain a definition
   for 'ConnectionString'`) the moment they update to this module version — `Body = Mode.Ignore` means
   that code never regenerates on its own, so no amount of template fixing helps existing output. Fixed
   with an `ITemplateMigration` on `MultiTenancyConfigurationTemplatePartial`
   (`AlignStaleTenantInfoReferencesMigration`, `TemplateMetadata` bumped `1.0` → `2.0`,
   `TemplateMigrationCriteria.Upgrade(1, 2)`): a real Roslyn AST pass (`CSharpSyntaxTree.ParseText` +
   `SyntaxEditor`, not string `Replace`). It first checks the file's own (always-regenerated)
   `services.AddMultiTenant<T>()` call to find the app's *current* tenant class, then branches across
   all three `GetTenantClass()`/`GetDefaultTenants()` scenarios:
   - `T == "TenantInfo"` (no extended type — shared-database, no named connection request): strips
     just the `ConnectionString` assignment from `new TenantInfo() {...}`, leaving `Id`/`Identifier`/
     `Name` intact — there's nowhere left to put the value.
   - `T` is an extended type and no named connection request exists (separate-database data isolation
     only): retargets `new TenantInfo()` → `new {T}()` and `IMultiTenantStore<TenantInfo>` →
     `IMultiTenantStore<{T}>`, **keeping** `ConnectionString` intact, since the extended type still
     carries it.
   - A named connection-string request exists (Cosmos/Mongo/MongoFramework/GoogleCloudStorage
     installed, matching item 4 above): the stale `ConnectionString` assignment — whether on a bare
     `TenantInfo` or an already-extended type that used to carry `ConnectionString` before the named
     property took over — is replaced with one assignment per registered
     `MultitenantConnectionStringRegistrationRequest` (`request.Name.ToCSharpIdentifier()`), substituting
     the object's own `Identifier` literal for the `{tenant}` placeholder in
     `request.ConnectionStringTemplate` — the same substitution `GetDefaultTenants()` uses for newly
     generated sample tenants. This is what item 4's manual fix (in
     `Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests`) should have been — the migration takes
     over that case now.
   The first version of this migration only handled the first two cases and always took the "keep
   `ConnectionString`" path whenever an extended type was already in use, regardless of whether a named
   connection request existed — for separate-database apps that also had a named connection request,
   that either silently deleted the connection string (when it hadn't retargeted yet) or left a
   property reference that no longer compiles (`ConnectionString` on a type that now only has
   `GoogleCloudStorageConnection`). Both gaps caught via user review of the staged diff, not by initial
   self-verification.
   Runs automatically the first time Software Factory executes after an app updates past this version —
   no manual edit needed. Verified against two real scenarios: `Tests/MinimalHostingModel`
   (separate-database only — retarget path: `TenantInfo` → `TenantExtendedInfo` on both the store type
   argument and both object creations, `ConnectionString` preserved) and
   `Tests/Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests` (named connection request on an
   already-extended type — reverted to its pre-fix stale state to re-test: staged diff showed
   `ConnectionString = "Tenant1Connection"` → `GoogleCloudStorageConnection = "JsonConnection-tenant1"`,
   matching exactly what a freshly generated app produces). Both apps build afterward. Note the criteria
   gotcha this took two tries to get right: `TemplateMigrationCriteria.UnversionedUpgrade(n)` only
   matches files with **no** `Version` attribute at all; this template's files have always carried an
   explicit `Version = "1.0"`, so `.Upgrade(1, 2)` is the correct criteria — `UnversionedUpgrade`
   silently bumped the version marker without touching content. See `.module-builder/RETROSPECTIVE.md`
   (2026-07-15 entry) for the full investigation trail.

   **Correction (6.0.0-pre.6):** the migration originally derived the target tenant class by parsing
   `services.AddMultiTenant<T>()` out of the file it was handed, with a comment asserting that call
   "is regenerated on every SF run, so its type argument reflects the app's current tenant class." That
   assumption is **wrong**: a template migration runs on the PREVIOUS output *before* regeneration, so in
   a genuine v5→v6 upgrade the file still says `AddMultiTenant<TenantInfo>`. The migration therefore read
   `currentTenantClass == "TenantInfo"` and took the shared-database **strip** path even for
   separate-database apps — leaving `IMultiTenantStore<TenantInfo>` / `new TenantInfo()` in the
   `Body = Ignore` `InitializeStore` while `AddMultiTenant<T>` regenerated to `TenantExtendedInfo`. Result:
   a file that compiles but throws `No service for type IMultiTenantStore<TenantInfo> has been registered`
   at startup (reported by user against a real v5→pre.5 upgrade). Fixed by passing `GetTenantClass`
   (a `Func<string>` — the class the template is *about to* generate) into the migration constructor and
   reconciling against that, instead of parsing the not-yet-regenerated file. **Why the earlier
   verification missed it:** both `MinimalHostingModel` and the GCS test were reverted to a hand-crafted
   stale state that *already* had `AddMultiTenant<TenantExtendedInfo>` in the file, so the parse happened
   to read the right type — the genuine Finbuckle-6-era shape (`AddMultiTenant<TenantInfo>`) was never
   exercised. Re-verified (pre.6) against `Finbuckle.SeparateDatabase.TestApplication` reverted to a true
   v1.0 / base-`TenantInfo` shape: staged diff retargets both the store lookup and the seeded tenants to
   `TenantExtendedInfo` and preserves `ConnectionString`, reproducing the committed file byte-for-byte;
   target builds clean. **Testing rule going forward: reproduce migrations from the genuine prior-version
   file shape, never a hand-doctored intermediate that already carries the post-upgrade type.**
7. **`AspNetCoreIntegrationExtension.GetSeparateDatabaseDataIsolationConfiguration()`'s `AddDbContext`
   registration silently fell back to `DefaultConnection` whenever no tenant was resolved** —
   `tenantInfo?.ConnectionString ?? configuration.GetConnectionString("DefaultConnection")`. The
   original comment justified this as "design-time safe" for EF Core CLI tooling (`dotnet ef
   migrations`), which runs outside any HTTP request so `tenantInfo` is legitimately null there.
   Problem: this repo's own `docs/README.md` already documents that migrations against a
   Finbuckle-managed app require installing `Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory`
   — which supplies its own `IDesignTimeDbContextFactory<T>`, picked up by `dotnet ef` *before* the
   app's DI/host is ever built, reading `appsettings.json` directly. When that module is installed (the
   documented, correct setup), this `AddDbContext` factory is **never invoked** at design time at all —
   so the `DefaultConnection` fallback was solving a problem that doesn't exist in the documented
   configuration, while simultaneously masking a real bug: if a tenant *is* resolved but its connection
   string is missing (misconfigured tenant, or a named-connection lookup miss), the same `?? DefaultConnection`
   silently reads/writes the wrong tenant's data instead of failing — a data-isolation defect, not a
   safe default. Fixed: removed the fallback entirely; now throws
   `Finbuckle.MultiTenant.MultiTenantException` whenever no tenant (or no connection string) is resolved,
   with a message that differs by environment — `IHostEnvironment.IsDevelopment()` gates whether the
   message includes the `DesignTimeDbContextFactory` install hint (for a developer who hit this by
   running `dotnet ef` without that module) or stays generic (production — don't leak internal
   module/architecture details to whatever surfaces the exception). Matches the existing
   `Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant ... connection information")`
   pattern already used by `Intent.Modules.MongoDb`/`MongoDb.MongoFramework`/`Google.CloudStorage`'s own
   `DependencyInjectionFactoryExtension.cs` — this brings the EF Core path in line with them. Required
   adding a new `Microsoft.Extensions.Hosting.Abstractions` package registration to this module's
   `NugetPackages.cs` (the Infrastructure class library project doesn't reference ASP.NET Core's shared
   framework, so `IHostEnvironment` isn't otherwise available there) and a matching
   `template.AddNugetDependency(...)` call. Verified against all three apps in this repo that hit this
   code path — `Finbuckle.SeparateDatabase.TestApplication`, `Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests`
   (which resolves via `tenantInfo?.Identifier`, not `ConnectionString` — see item 5 above — the fix
   composes cleanly with that variant), and `MassTransitFinbuckle.Test` — staged diffs inspected and
   applied, all three build clean afterward. Note: `TryGetMongoDbConfiguration` in this same file (a
   separate code path targeting an `Infrastructure.Configuration.MongoDb.MultiTenancy` template role)
   was investigated as part of this and found to be dead code in this repo — no template currently
   registers that role, so it was left untouched.

### Infra used for runtime verification (all local, Docker where noted)

- SQL Server: local `Server=.` Developer instance, Windows auth (Finbuckle EF apps).
- MongoDB: Docker `mongo:7`, container `finbuckle-mongo`, `localhost:27017`, no auth.
- Cosmos DB: Docker Linux emulator, container `finbuckle-cosmos`, `localhost:8081` — emulator
  regenerates its self-signed TLS cert on certain restarts; re-trust it if `.NET`/`curl` start
  rejecting it with `UntrustedRoot` (`certutil -addstore -user Root <exported-cert>`).
- MassTransit: in-memory transport (`x.UsingInMemory(...)`) — no broker required for any scenario
  tested here.
