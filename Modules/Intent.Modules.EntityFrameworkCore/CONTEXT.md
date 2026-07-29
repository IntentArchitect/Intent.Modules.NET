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
the MediatR `UnitOfWorkBehaviour` detect whether an external party (e.g. NServiceBus
`ITransactionalSession`, raw ADO.NET) has already enlisted an EF connection/transaction.
When it returns `true`, the behaviour skips wrapping in `TransactionScope` to avoid MSDTC
escalation.

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

3. **`UnitOfWorkBehaviour.Handle`** — the injected early-return guard. See below.

### Guard: Non-EF Unit-of-Work Backends

`ModifyUnitOfWorkBehaviour` must **not** inject the `HasDbTransaction` guard into projects
that don't use EF at all (e.g. Dapr, in-memory). Those projects have no `_dataSource` field
and the injected code won't compile.

Guard condition: skip injection if **no** template fulfills either
`TemplateRoles.Infrastructure.Data.DbContext` or
`TemplateRoles.Infrastructure.Data.ConnectionStringDbContext`.

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

---

## `Primary Connection String Name` — Overriding the "Primary DbContext" Identifier

### What It Is

`DbContextManager` used to hardcode the literal `"DefaultConnection"` as the connection string
name that identifies the "primary" DbContext — the one named `ApplicationDbContext`, that
participates in the `HasDbTransaction` co-existence logic above, and that keeps a package out of
multi-DbContext mode. The `Primary Connection String Name` module setting (Database Settings
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

Resolution lives in `DbContextInstance.ResolvePrimaryConnectionStringName(IApplicationSettingsProvider)`,
which reads `settings.GetDatabaseSettings().PrimaryConnectionStringName()` and falls back to the
`DefaultConnection` const when blank. `DbContextName`/`IsApplicationDbContext` compare
`ConnectionStringName` against this resolved value instead of a hardcoded literal.

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
the other three dependents (`5.1.0-pre.2`). **When bumping `Intent.Modules.EntityFrameworkCore`
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
