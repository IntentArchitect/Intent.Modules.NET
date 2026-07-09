# Finbuckle.MultiTenant Upgrade: v6.13.1 → v10.x

## Context

`Intent.Modules.AspNetCore.MultiTenancy` and its dependents (`Intent.Modules.CosmosDB`,
`Intent.Modules.Eventing.MassTransit`, `Intent.Modules.MongoDb`/`MongoDb.MongoFramework`) are
Intent Architect code generators that emit Finbuckle-based multi-tenancy code into target
applications. All of them currently pin `Finbuckle.MultiTenant` at **6.13.1 (locked)** — four
major Finbuckle releases behind. `Intent.Modules.MongoDb` already drifted ahead independently
(its `NugetPackages.cs` has a `net10→10.0.2` lane) via a mechanical "bump packages to .NET 10"
commit, but it has **zero actual Finbuckle API call sites**, so that drift proves nothing about
whether the generated code is compatible — it isn't proof of a working ladder, just proof a
ladder is cheap when there's no code to break.

A full codebase inventory (Explore agent) plus a verified breaking-changes timeline (WebFetch of
official Finbuckle docs, cross-checked directly against this repo's code) found that **every**
Finbuckle call site in this repo is still on the pre-v7 API shape, so it will fail at the first
compile the moment the package version moves past v6 — this is not a "jump to v10 introduces
new problems" situation, it's "the v7 break already applies today and has just never been paid."
The goal of this work is to bring the generated code up to the v10.x-compatible shape while
keeping net8/net9 target apps on a still-supported intermediate version, per the user's decision
below.

**Decision confirmed with user:** two-bracket TFM strategy (not a hard net10-only cutover, not a
5-way ladder matching MongoDb's cosmetic pattern). See §2.

## Known unknowns — verify before writing template code

Web research on Finbuckle's exact v10.x API surface produced **internally contradictory results**
across official doc pages (one page implies `ITenantInfo` still exists at v10.1.0, the
history/changelog page says it was removed in v10.0.0 in favor of `TenantInfo` as a record).
Further doc-fetching is not reliable enough to resolve this. Before any template file is edited,
do a throwaway spike:

1. Create a scratch console project, `dotnet add package Finbuckle.MultiTenant --version <latest 10.x>`
   (+ `.AspNetCore`, `.EntityFrameworkCore` as needed).
2. Use Go-to-Definition/decompilation to confirm, as ground truth:
   - Does `ITenantInfo` still exist, and is it still usable as a DI/cast target?
   - `IMultiTenantDbContext.TenantInfo` property's exact declared type.
   - `EFCoreStoreDbContext<T>` constructor signature and generic constraint on `T`.
   - `IMultiTenantStore<T>` member signatures (specifically `TryAddAsync`).
   - Exact latest patch versions to pin for the 9.x and 10.x lanes.
3. Record findings before touching step 1 of the increment list below — several remediation
   shapes in §3 are contingent on this (flagged inline).

## 1. Sequencing

Work through modules in dependency order — later modules either literally cannot run without the
earlier ones' templates (MassTransit's Finbuckle filters gate on
`AspNetCore.MultiTenancy.MultiTenancyConfiguration` existing) or conceptually depend on the same
pinned version being consistent across the generated app:

1. **`Intent.Modules.AspNetCore.MultiTenancy`** (root — do this first, most call sites, everything else depends on its shape)
2. **`Intent.Modules.CosmosDB`** (independent call sites, but factory extension checks for the root module's template)
3. **`Intent.Modules.Eventing.MassTransit`** (highest-risk — cross-process tenant propagation; templates literally can't run without step 1's output existing)
4. **`Intent.Modules.MongoDb` / `Intent.Modules.MongoDb.MongoFramework`** (no code changes — no Finbuckle call sites exist here — just reconcile its `NugetPackages.cs` version numbers against whatever gets pinned in step 1, and bump the `Intent.Modules.AspNetCore.MultiTenancy` minimum-version `<detect>` entry in its `.imodspec`)

Each module change follows this repo's mandatory **SF iteration cycle** per AGENTS.md/
`module-increment-loop`: edit template/factory-extension → `dotnet build` module → reinstall into
target app → run Software Factory → inspect staged diff → apply → build target → run. Update
`docs/README.md` and `release-notes.md` in the *same turn* as each template edit, not batched at
the end. Finish each module with `module-wrap-up` (version bump, `CONTEXT.md`) once all its
increments are verified — do this per-module, not once globally at the very end.

## 2. TFM strategy (confirmed: two-bracket)

- **Bracket A — net6.0: frozen.** Keep `6.13.1 (locked: true)` exactly as-is. No template
  changes are reachable for net6 output.
- **Bracket B — net7.0/net8.0/net9.0: mandatory-fix baseline, pinned to latest 9.x.** The
  DI-resolution and `ConnectionString`-removal fixes in §3 are not v10-specific — they're
  required as of v7, so net7/8/9 need the same fixed code regardless of the v10 decision. Once
  fixed, pin all three lanes to the same latest 9.x patch (confirm exact patch via the spike in
  §0).
- **Bracket C — net10.0: v10.x, layered on the same fixed baseline.** Apply the v10-specific
  deltas (package/namespace split, named EF Core query filter constant, any `EFCoreStoreDbContext`/
  `IMultiTenantStore` signature changes surfaced by the spike) on top of Bracket B's code, gated
  by the same `outputTarget.GetMaxNetAppVersion()` pattern `NugetPackages.cs` already uses for
  version selection — reuse it for code-shape branching in the factory extensions where the two
  brackets diverge.

This means most factory-extension edits are a single corrected code shape (Bracket B), with a
small number of `if (targetsNet10) { ... } else { ... }`-style branches only where v10 genuinely
diverges (§3, items 1's `IMultiTenantDbContext.TenantInfo` property type if the spike confirms a
type change, and item 9's possible namespace move).

## 3. Per-risk remediation (module → file → shape)

**Standardize on the concrete `TenantInfo` type (or the repo's own `TenantExtendedInfo : TenantInfo`)
everywhere instead of the `ITenantInfo` interface**, resolved via the generic
`IMultiTenantContextAccessor<T>` instead of raw constructor injection or `IServiceProvider.GetService<T>()`.
This one convention change removes the version-sensitivity of nearly every call site at once — it's
already how `Intent.Modules.CosmosDB.FactoryExtensions.MultiTenancyFactoryExtension` does it
correctly today, so that file is the reference shape for the rest.

| # | File | Current shape | Target shape |
|---|---|---|---|
| 1 | `Modules/Intent.Modules.AspNetCore.MultiTenancy/FactoryExtensions/AspNetCoreIntegrationExtension.cs:150-153` | Shared-DB `ApplicationDbContext` ctor takes `ITenantInfo tenantInfo`, stores on `ITenantInfo TenantInfo { get; private set; }` to satisfy `IMultiTenantDbContext` | Ctor takes `IMultiTenantContextAccessor<{ConcreteTenantType}>`, resolves `.MultiTenantContext?.TenantInfo`. **Property type on `IMultiTenantDbContext.TenantInfo` is spike-dependent (§0) — may need a Bracket B/C branch here specifically.** |
| 2 | Same file, line 235 | `sp.GetService<ITenantInfo>()` inside `AddDbContext` options lambda (separate-DB mode) | `sp.GetRequiredService<IMultiTenantContextAccessor<{ConcreteTenantType}>>().MultiTenantContext?.TenantInfo`, throw existing `MultiTenantException` when null |
| 3 | `Modules/Intent.Modules.AspNetCore.MultiTenancy/Templates/TenantConnectionsInterface/TenantConnectionsInterfaceTemplatePartial.cs:70-72` | `provider.GetService<ITenantInfo>() as {TenantExtendedInfo}` | Same accessor-based resolution as #2 |
| 4 | `Modules/Intent.Modules.Eventing.MassTransit/Templates/FinbucklePublishingFilter/FinbucklePublishingFilterTemplatePartial.cs:37` (+ sibling `FinbuckleSendingFilter`) | Ctor field `ITenantInfo tenant`, reads `.Identifier` | Ctor takes `IMultiTenantContextAccessor<{ConcreteTenantType}>`, reads `.MultiTenantContext?.TenantInfo?.Identifier` |
| 5 | `Modules/Intent.Modules.AspNetCore.Mvc/Templates/MvcController/MvcControllerTemplatePartial.cs:109`, `Modules/Intent.Modules.AspNetCore.Controllers/Templates/Controller/ControllerTemplatePartial.cs:125` | Action parameter typed `Finbuckle.MultiTenant.ITenantInfo` | Swap to concrete tenant type — trivial, no interface-specific behavior used here |
| 6 | `Modules/Intent.Modules.CosmosDB/Templates/CosmosDBMultiTenantMiddleware/CosmosDBMultiTenantMiddlewareTemplatePartial.cs:56` | `scope.ServiceProvider.GetService<TenantInfo>()` (concrete type, but still direct DI resolution — breaks at v7 too) | `scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor<TenantInfo>>().MultiTenantContext?.TenantInfo` |
| 7 | `Modules/Intent.Modules.CosmosDB/FactoryExtensions/MultiTenancyFactoryExtension.cs:124` | Already correct shape (`IMultiTenantContextAccessor<TenantInfo>`, reads `.MultiTenantContext?.TenantInfo?.Id`) | No change. Reference implementation for the rest of this table. |
| 8 | `Modules/Intent.Modules.Eventing.MassTransit/Templates/FinbuckleConsumingFilter/FinbuckleConsumingFilterTemplatePartial.cs:43,60` | Ctor takes untyped `IMultiTenantContextAccessor accessor`, sets `_accessor.MultiTenantContext = multiTenantContext;` after `ResolveAsync` | Make the accessor generic (`IMultiTenantContextAccessor<{ConcreteTenantType}>`), matching #7. The mutation line itself stays — the accessor's `MultiTenantContext` setter remains valid even under v10's context-immutability change (it assigns a whole new instance, which is the supported pattern) |
| 9 | `Modules/Intent.Modules.AspNetCore.MultiTenancy/Templates/MultiTenantStoreDbContext/MultiTenantStoreDbContextTemplatePartial.cs:34-35` | `WithBaseType("EFCoreStoreDbContext<TenantInfo>")`, ctor forwards `DbContextOptions` | Unchanged unless the §0 spike reveals a constructor/generic-arity change in v10's `EFCoreStoreDbContext` — **do not touch without spike confirmation** |
| 10 | `Modules/Intent.Modules.AspNetCore.MultiTenancy/Templates/MultiTenancyConfiguration/MultiTenancyConfigurationTemplatePartial.cs` (`InitializeStore`/seed data, ~line 145) | `new {TenantClass}() { Id = ..., Identifier = ..., Name = ..., ConnectionString = ... }` | Drop `ConnectionString` from the seed object initializer (removed from `TenantInfo` since v7) — multi-connection-string mode already uses this repo's own `TenantExtendedInfo.ConnectionStrings` collection, unaffected |
| 11 | Same file — `WithInMemoryStore`/`WithEFCoreStore`/`WithConfigurationStore`/`WithHttpRemoteStore`, `WithHeaderStrategy`/`WithClaimStrategy`/`WithHostStrategy`/`WithRouteStrategy` builder chain | Stable API today | No forced change — stable through v10. Optional follow-up (not part of the compile-fix critical path): expose `useTenantAmbientRouteValue` on `WithRouteStrategy` as a new module setting, Bracket C only |
| 12 | All `Finbuckle.MultiTenant.MultiTenantException` throw sites (`AspNetCoreIntegrationExtension.cs:235`, `TenantConnectionsInterfaceTemplatePartial.cs:63/67`, Google.CloudStorage module) | Fully-qualified `Finbuckle.MultiTenant.MultiTenantException` | No change expected; confirm via spike whether the type moves to `Finbuckle.MultiTenant.Abstractions` namespace for Bracket C |
| 13 | `Modules/Intent.Modules.AspNetCore.MultiTenancy/NugetPackages.cs`, `Modules/Intent.Modules.CosmosDB/NugetPackages.cs` | Both hard-pin `6.13.1 (locked: true)` with no ladder (`CosmosDB` has *no* switch arms below `>= 8`) | Add the two-bracket ladder from §2 to both — this is new code for CosmosDB, not an extension |
| 14 | `Modules/Intent.Modules.MongoDb/NugetPackages.cs` | Independent ladder already pins `9.4.5`/`10.0.2` | Reconcile these version numbers to exactly match whatever gets pinned in `AspNetCore.MultiTenancy`'s ladder — they describe the same package in the same generated app and must not drift |
| 15 | `Intent.MongoDb.imodspec`, `Intent.Eventing.MassTransit.imodspec` — `<detect id="Intent.Modules.AspNetCore.MultiTenancy">` minimum version | Currently `5.1.5` | Bump to whatever version `AspNetCore.MultiTenancy` ships as part of this change (module-wrap-up step) |

No custom `IMultiTenantStore` implementation exists in the repo (only built-in stores are used),
and `FinbuckleMessageHeaderStrategy`'s `IMultiTenantStrategy.GetIdentifierAsync(object context)`
signature is confirmed stable through v10 — neither needs remediation.

## 4. Verification plan

**Canary 1 — `Finbuckle.SeparateDatabase.TestApplication`.** Exercises the most surface area in
one app: EF Core separate-DB isolation (table rows 2, 9, 10), `MultiTenancyConfiguration`/
`MultiTenantStoreDbContext`. Single connection string, so `TenantConnectionsInterface` (row 3)
isn't exercised here — that's fine, it gets covered by canary 2 or the MongoDb/CosmosDB rollout.

**Canary 2 — `MassTransitFinbuckle.Test`.** Must be validated separately — it's the only app
exercising cross-process header propagation (rows 4, 8). A single-app EF pass cannot substitute
for this; publish a message as tenant A, confirm the header carries `.Identifier`, consume it,
confirm `IMultiTenantContextAccessor.MultiTenantContext.TenantInfo.Identifier` matches inside the
consumer and correctly scopes DB access there.

**Per-canary pass (repeat for each, per `module-increment-loop`):**
1. `dotnet build` the touched module project(s).
2. Reinstall the built module DLL into the target test app.
3. Run Software Factory; inspect the staged diff file-by-file before applying.
4. Apply, `dotnet build` the target app — first compile is the primary signal for rows 1-6.
5. `dotnet run`, hit an endpoint requiring tenant resolution; confirm correct connection
   string/database (separate-DB) or correct query-filter scoping (shared-DB — re-verify with
   `.IgnoreQueryFilters(Abstractions.Constants.TenantToken)` if the named-filter change applies).
6. Update `docs/README.md` + `release-notes.md` in the same turn as the template edit that
   caused the behavior change.

**Rollout:** only after both canaries are fully green, apply the identical template changes to
the remaining 6 test apps (`CosmosDBMultiTenancy`, `CosmosDB.MultiTenancy.SeperateDB`,
`MongoDb.MultiTenancy.SeperateDb`, `Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests`,
`MinimalHostingModel`, `Finbuckle.SharedDatabase.TestApplication`) — these should mostly confirm
rather than discover new issues, since the canaries already exercised the shared template code.

Finish with `module-wrap-up` per module (version bump per the imodspec/csproj/designer precedence
rule, `CONTEXT.md`, confirm SF clean) once all of that module's increments are verified.

## Critical files

- `Modules/Intent.Modules.AspNetCore.MultiTenancy/FactoryExtensions/AspNetCoreIntegrationExtension.cs`
- `Modules/Intent.Modules.AspNetCore.MultiTenancy/NugetPackages.cs`
- `Modules/Intent.Modules.AspNetCore.MultiTenancy/Templates/MultiTenantStoreDbContext/MultiTenantStoreDbContextTemplatePartial.cs`
- `Modules/Intent.Modules.AspNetCore.MultiTenancy/Templates/MultiTenancyConfiguration/MultiTenancyConfigurationTemplatePartial.cs`
- `Modules/Intent.Modules.AspNetCore.MultiTenancy/Templates/TenantConnectionsInterface/TenantConnectionsInterfaceTemplatePartial.cs`
- `Modules/Intent.Modules.CosmosDB/NugetPackages.cs`
- `Modules/Intent.Modules.CosmosDB/Templates/CosmosDBMultiTenantMiddleware/CosmosDBMultiTenantMiddlewareTemplatePartial.cs`
- `Modules/Intent.Modules.CosmosDB/FactoryExtensions/MultiTenancyFactoryExtension.cs` (reference shape, no change)
- `Modules/Intent.Modules.Eventing.MassTransit/Templates/FinbuckleConsumingFilter/FinbuckleConsumingFilterTemplatePartial.cs`
- `Modules/Intent.Modules.Eventing.MassTransit/Templates/FinbucklePublishingFilter/FinbucklePublishingFilterTemplatePartial.cs` (+ sibling `FinbuckleSendingFilter`)
- `Modules/Intent.Modules.MongoDb/NugetPackages.cs`
- `Modules/Intent.Modules.AspNetCore.Mvc/Templates/MvcController/MvcControllerTemplatePartial.cs`, `Modules/Intent.Modules.AspNetCore.Controllers/Templates/Controller/ControllerTemplatePartial.cs`
