# EntityFrameworkCore Module Context

> **Module:** `Intent.Modules.EntityFrameworkCore`
>
> **Purpose:** Durable architectural and implementation context for future work in this
> module. Read this before changing templates, factory extensions, or test applications
> related to EF Core.

---

## `HasDbTransaction` — External Transaction Co-existence

### What It Is

`HasDbTransaction()` is a method added by `UnitOfWorkExternalTransactionExtension` that lets
a dispatch stack's unit-of-work seam detect whether an external party (e.g. NServiceBus
`ITransactionalSession`, raw ADO.NET) has already enlisted an EF connection/transaction.
When it returns `true`, the seam skips wrapping in `TransactionScope` to avoid MSDTC
escalation. Two dispatch stacks are wired to this guard, held to the same behavioural bar
(run the handler, save, return — no scope):

- **MediatR** — `UnitOfWorkBehaviour.Handle`, the original implementation.
- **Wolverine** — `UnitOfWorkMiddleware.Before` (`ModifyUnitOfWorkMiddleware`, added alongside

  fixing the durable-outbox `TransactionMiddlewareMode.Lightweight` collision in
  `Intent.Eventing.Wolverine`). Wolverine's `Before` always opened an unconditional
  `TransactionScope`, unlike MediatR's guarded one, so it collided with an external
  transaction owner the same way MediatR's used to before this guard existed. `AfterAsync`
  already calls `SaveChangesAsync` unconditionally and null-guards the scope
  (`tx?.Complete()` / `tx?.Dispose()`), so `Before` returning `null` reproduces MediatR's
  three effects (run, save, no scope) without a second code path.

### Where the Method Must Appear

The method must be present in **three places** for a project to compile:

1. **The unit-of-work interface** — either `IUnitOfWork` or `IApplicationDbContext` depending
   on which interface the project exposes. Both are patched:
   - `AddHasDbTransactionToInterface` → targets `Intent.Entities.Repositories.Api.UnitOfWorkInterface`
   - `AddHasDbTransactionToDbContextInterface` → targets `TemplateRoles.Application.Common.DbContextInterface`

2. **Every EF DbContext class** — both primary and secondary DbContexts must implement it.
   `AddHasDbTransactionToDbContext` targets **both** template roles:
   - `TemplateRoles.Infrastructure.Data.DbContext` (primary / `IsApplicationDbContext = true`)
   - `TemplateRoles.Infrastructure.Data.ConnectionStringDbContext` (secondary / multi-DbContext)
   The method is added unconditionally to every DbContext — no interface guard. This is
   intentional: the interface check was too narrow (a DbContext may implement `IApplicationDbContext`
   rather than `IUnitOfWork`), and an extra method on a class that doesn't need it is harmless.

3. **`UnitOfWorkBehaviour.Handle`** (MediatR) **and/or `UnitOfWorkMiddleware.Before`**
   (Wolverine) — whichever dispatch template(s) the application has, each gets its own
   injected guard. See below.

### Guard: Injection Requires the EF Unit-of-Work Field to Exist

`ModifyUnitOfWorkBehaviour`/`ModifyUnitOfWorkMiddleware` must **not** inject the
`HasDbTransaction` guard into projects that don't use EF at all (e.g. Dapr, in-memory).
Those projects have no data source to call `HasDbTransaction()` on and the injected code
won't compile.

**Superseded guard (shipped 5.0.46 - 5.1.1):** skip injection if **no** template fulfills
either `TemplateRoles.Infrastructure.Data.DbContext` or
`TemplateRoles.Infrastructure.Data.ConnectionStringDbContext`.

**Why the role check was insufficient.** It answers "does an EF DbContext exist?", which is a
*proxy* for the question that actually decides whether the field exists — "did the EF
unit-of-work chain emit a field into *this* class?". The two diverge, because the component
that creates the field and the component that emitted the guard gate on different predicates:

| Component | Gate |
|---|---|
| Creates `_dataSource` — `PersistenceUnitOfWork.AddEntityFrameworkToChain` (in `Intent.Common.UnitOfWork`) | `TemplateRoles.Infrastructure.Data.DbContext` — **primary DbContext only** |
| Emitted `_dataSource.HasDbTransaction()` — `ModifyUnitOfWorkBehaviour` | `DbContext` **OR** `ConnectionStringDbContext` |

`DbContextTemplate` fulfils `ConnectionStringDbContext` unconditionally but `DbContext` only
when `Model.IsApplicationDbContext` — which requires
`ConnectionStringName == DefaultConnectionStringName`. So in an application where **every**
domain package sets a custom Connection String Name, no DbContext is primary: no field is
created, but the old guard still injected the code.

The timeline makes this a genuine supersession rather than an oversight. The `HasDbTransaction`
work shipped in **5.0.46**, when "no primary DbContext" was a far more obscure corner.
`Default Connection String Name` shipped later in **5.1.0** and made custom connection-string
names a first-class scenario.

**Current guard (5.1.2 onward)** — inspect the built artifact, not a proxy:

```csharp
var unitOfWorkField = @class.Fields.FirstOrDefault(x => x.Name == "_dataSource");
if (unitOfWorkField == null) return;   // EF unit-of-work chain did not run for this class
```

... and emit `unitOfWorkField.Name` rather than the literal, so the guard and the emitted code
cannot disagree. Precedent for this exact shape in the same module family:
`EntityFrameworkCore.Repositories/FactoryExtensions/CustomRepositoryMethodsExtension.cs`
(`@class.Fields.Any(x => x.Name == "_dbContext")`).

The same check also closes the `UnitOfWorkResolutionStrategy.ServiceProvider` hole, which emits
a *local* variable and no field.

**Materialising the field instead is NOT an alternative.**
`EntityFrameworkCore.Repositories/FactoryExtensions/DbContextInterfaceExtension.cs` publishes
the `IUnitOfWork` -> DbContext container registration **only when `IsApplicationDbContext`**
(verified: `Tests/EntityFrameworkCore.MultiDbContext.WithDefaultDbContext` has the registration
in `Infrastructure/DependencyInjection.cs`; `...NoDefaultDbContext` has none). Injecting the
field would trade a compile error for a startup `InvalidOperationException`. Skipping is
correct.

**Exposing the field name from `Intent.Common.UnitOfWork` is also rejected.**
`Intent.Modules.EntityFrameworkCore.csproj` has no reference to
`Intent.Modules.Common.UnitOfWork` and the `.imodspec` `<dependencies>` does not list it. That
would create a brand-new hard module dependency, plus the local-compile /
`MissingMethodException` trap described under "Dependency Floors" below — to obtain a *less*
reliable signal than reading the field.

### Callback Ordering — OnBuild Completes Globally Before Any AfterBuild

The field-existence check depends on the field already being there when this extension's
`AfterBuild` runs. That ordering is **guaranteed, not assumed**:
`ApplyUnitOfWorkImplementations` runs inside the `AddClass(...)` configure lambda, which
`CSharpFile` defers to the **OnBuild** phase at priority 0 — not at template construction.
`FileBuilderFactoryExtension` (`Order = int.MaxValue`) drains the entire OnBuild phase for
*all* templates before *any* `AfterBuild` callback runs. So a field created during OnBuild is
visible in `AfterBuild` at any priority. The `AfterBuild(..., 500)` priority is therefore not
load-bearing — this extension is the only cross-module mutator of that template.

### Accepted Coupling — the `_dataSource` Field Name

The check hardcodes the field name `_dataSource`. That name comes from
`fieldSuffix: "dataSource"` at
`Intent.Modules.Application.MediatR.Behaviours/Templates/UnitOfWorkBehaviour/UnitOfWorkBehaviourTemplatePartial.cs`
— the only non-default `fieldSuffix` among the 16 `ApplyUnitOfWorkImplementations` call sites,
and it lives in a different module. The coupling is accepted because the check **fails
closed**: if the name ever changes, injection is skipped and the build stays green (the
co-existence optimisation is silently lost) rather than emitting code that will not compile.

### Guard: MediatR `next()` vs `next(cancellationToken)`

Older MediatR versions (used in .NET 6 projects) use `next()` with no argument. Newer
versions use `next(cancellationToken)`. The injected block must match the existing call style
already present in the Handle method — do not hardcode `next(cancellationToken)`.

Detection: inspect `handleMethod.Statements` for `"next(cancellationToken)"` and fall back
to `"next()"` if not found.

### Multi-DbContext Projects

In projects with more than one DbContext, only the primary one fulfills
`TemplateRoles.Infrastructure.Data.DbContext`. Secondary DbContexts only fulfill
`TemplateRoles.Infrastructure.Data.ConnectionStringDbContext`. Both roles must be targeted
when adding `HasDbTransaction()` to the implementation — use `FindTemplateInstances` (plural)
over both roles, not `FindTemplateInstance` (singular) over the primary role only.

### Known Hazards — Found While Fixing the Guard in 5.1.2, Deliberately NOT Fixed There

These are real, verified, and **still ship**. Each was left out of 5.1.2 because it belongs in
a different module or needs its own fixture. Do not rediscover them; do not assume they are
fixed.

1. **The injected early-return bypasses other unit-of-work providers in the chain.** Verified
   in `Tests/AdvancedMappingCrud.Repositories.Tests/.../UnitOfWorkBehaviour.cs`: the guard
   returns before `using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())`, so that
   provider's `SaveChangesAsync` never runs when an external EF transaction is active — a
   silent loss of distributed-cache unit-of-work writes. Fixing it properly belongs in
   `Intent.Common.UnitOfWork`: make only the `TransactionScope` conditional at runtime, rather
   than early-returning past the whole composed chain.

2. **The injected block ignores `Automatically Persist Unit Of Work`.** It always emits
   `await _dataSource.SaveChangesAsync(cancellationToken);`, while all ten provider chains in
   `PersistenceUnitOfWork.cs` guard on `config.AutomaticallyPersistUnitOfWork` (setting
   `d6338b7c-b0f9-46bd-8dbb-3c745d5f8623`). Apps with automatic persistence off get an
   unwanted `SaveChangesAsync` on that path.

3. **`IUnitOfWork.HasDbTransaction()` + `Intent.MongoDb.MongoFramework` -> `CS0535`.**
   `AddHasDbTransactionToInterface` has no guard, and `AddHasDbTransactionToDbContext`
   implements the method only on the two EF DbContext roles. MongoFramework's
   `IMongoDbUnitOfWork` extends `IUnitOfWork` and its `ApplicationMongoDbContext` implements
   that interface — so the class would inherit an unimplemented member. **Zero current
   exposure:** no test app installs `Intent.MongoDb.MongoFramework`, and the non-framework
   `Intent.MongoDb` has its `ExtendsInterface` call commented out (verified against
   `Tests/CleanArchitecture.SingleFiles`, which runs EF + Mongo and compiles). Every cheap fix
   costs more than the bug: a C# 8 default interface member changes `IUnitOfWork` for all
   consumers and breaks netstandard2.0 domain projects, and hardcoding MongoFramework's role
   into this module inverts the dependency. Close it separately, with its own fixture.

---

## `Default Connection String Name` — Overriding the "Primary DbContext" Identifier

### What It Is

`DbContextManager` used to hardcode the literal `"DefaultConnection"` as the connection string
name that identifies the "primary" DbContext — the one named `ApplicationDbContext`, that
participates in the `HasDbTransaction` co-existence logic above, and that keeps a package out of
multi-DbContext mode. The `Default Connection String Name` module setting (Database Settings
group, setting id `ad9681ea-9388-4415-9b94-de2ced2b7307`) lets a developer override that
identifier.

Design decisions (already settled — do not re-litigate without an explicit new user request):
- **No `Database Settings` stereotype changes.** `ConnectionStringName`, `DatabaseProvider`, etc.
  are untouched; this setting only changes what counts as "primary."
- **No `Intent.Modules.ModularMonolith.Module` changes.** Not needed for this feature.
- **Blank default = zero behavior change.** Unset resolves to the literal `"DefaultConnection"`,
  identical to pre-feature behavior.
- **Confirmed no dependency, left untouched:** Hangfire, NServiceBus, Dapper,
  Blazor.Authentication depend only on the `TemplateRoles.Infrastructure.Data.DbContext` /
  `.ConnectionStringDbContext` role abstraction, not the connection-string-name literal.

Resolution lives in `DbContextInstance.ResolveDefaultConnectionStringName(IApplicationSettingsProvider)`,
which reads `settings.GetDatabaseSettings().DefaultConnectionStringName()` and falls back to the
`DefaultConnection` const when blank. `DbContextName`/`IsApplicationDbContext` compare
`ConnectionStringName` against this resolved value instead of a hardcoded literal.

Renamed from `Primary Connection String Name` on 2026-07-29 (user request) — the setting, its
generated extension method, and the internal `DbContextManager.cs` property/method/params were
all renamed together (`DefaultConnectionStringName` throughout) to stay consistent.

### Backward Compatibility — `DbContextManager` / `DbContextInstance` Are Public

Both types are `public`, so modules outside this repo (or any not yet rebuilt against a newer
`Intent.Modules.EntityFrameworkCore`) may call them directly. Making `GetDbContexts`/
`GetDbContext`/the `DbContextInstance` constructor *require* an `IApplicationSettingsProvider`
would be a binary-breaking change for those callers — `MissingMethodException` at SF runtime,
the same failure mode as the "local-compile trap."

Fix: the pre-feature signatures (`GetDbContexts(string, IMetadataManager)`,
`GetDbContext(ClassModel)`, `DbContextInstance(DomainPackageModel)`) are kept as genuine
overloads — **not optional parameters**, since a default parameter value is baked into the
*caller's* IL at compile time, not the callee's, so it does not add back a missing method for
already-compiled callers. The old overloads are `[Obsolete]` (warning, not error) and hardcode
`"DefaultConnection"` internally via a shared private constructor / `ValidateAndReturn` helper.

**When touching these signatures again:** preserve this pattern — add a new overload, keep the
old one working via delegation, mark the old one `[Obsolete]`. Don't delete an obsolete overload
without an explicit user decision to accept the break (e.g. at a major version bump).

### Dependency Floors Protect Install-Order Safety — Keep Them in Sync

`install_or_update_modules` auto-resolves dependencies based on each module's own declared
`<dependency id="Intent.EntityFrameworkCore" version="X" />` floor in its `.imodspec` (a bare
version string, interpreted as a NuGet-style minimum — no module in this repo uses bracket-range
syntax for `<dependency>`, unlike `supportedClientVersions`). If a module updates its own code to
call a *new* capability from `Intent.EntityFrameworkCore` (e.g. the settings-aware
`GetDbContext`/`GetDbContexts` overloads), its declared floor **must** be bumped to at least the
version that introduced that capability in the same change — otherwise IA has no signal to
cascade-upgrade an older already-installed `Intent.EntityFrameworkCore`, and the dependent
module's compiled call to the missing overload throws `MissingMethodException` at SF runtime if
someone updates the dependent before the core module.

Caught a real instance of this omission on 2026-07-29: `AspNetCore.OData.EntityFramework`'s call
site was updated to the settings-aware overload but its imodspec still declared
`<dependency id="Intent.EntityFrameworkCore" version="5.0.20" />` (pre-feature). Fixed to match
the other three dependents' floor. **When bumping `Intent.Modules.EntityFrameworkCore`
because of an API surface change, grep the whole repo for
`dependency id="Intent.EntityFrameworkCore"` and re-check every hit** — not just the modules you
remember touching.

The seven other modules depending on `Intent.EntityFrameworkCore` in this repo (`AspNetCore.Identity`,
`AspNetCore.IdentityService`, `AspNetCore.Identity.AccountController`, `AzureFunctions.EntityFrameworkCore`,
`EntityFrameworkCore.DataMasking`, `SharedKernel`, `Eventing.MassTransit.EntityFrameworkCore`) were
correctly left at their pre-feature floors — they only call the preserved, backward-compatible
old overloads, so they need no floor bump and are safe to install/update in any order relative to
`Intent.EntityFrameworkCore`.

### In-Repo Call Sites Threaded Through `IApplicationSettingsProvider`

`EntityFrameworkCore` (`DependencyInjectionExtension`, `DbContextTemplatePartial`,
`DbContextTemplateRegistration`, `DbContextInterfaceTemplateRegistration`,
`DbMigrationsReadMeTemplatePartial` ×2), `EntityFrameworkCore.Interop.DomainEvents`
(`DomainEventsDbContextExtension`), `EntityFrameworkCore.DesignTimeDbContextFactory`
(`DesignTimeDbContextFactoryTemplatePartial`), `AspNetCore.OData.EntityFramework`
(`ODataAggregateControllerTemplatePartial`), and `EntityFrameworkCore.Repositories`
(`RepositoryTemplatePartial`, `EntityFrameworkRepositoryHelpers` ×3 constructor sites,
`CustomRepositoryMethodsExtension`, `DataContractExtensionMethodsTemplateRegistration`) all use
the new settings-aware overloads. Each of these four dependent modules had its
`Intent.Modules.EntityFrameworkCore` dependency bumped to match and its own version bumped,
since each has a real generated-output-affecting code change.
