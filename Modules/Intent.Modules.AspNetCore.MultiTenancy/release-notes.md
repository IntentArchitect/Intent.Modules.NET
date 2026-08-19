### Version 6.0.0

> ⚠️ **BREAKING CHANGES**
>
> - Upgraded Finbuckle.MultiTenant 6.13.1 → 9.4.10. **.NET 8+ only** — 6/7 no longer supported.
> - `ITenantInfo`/`TenantInfo` DI injection replaced by `IMultiTenantContextAccessor<T>` (read) / `IMultiTenantContextSetter` (write), matching Finbuckle 9.x. Namespaces moved: `ITenantInfo`, `IMultiTenantContext(Accessor)<T>`, `IMultiTenantStore<T>`, `ITenantResolver<T>` → `Finbuckle.MultiTenant.Abstractions`; `InMemoryStoreOptions<T>` → `Finbuckle.MultiTenant.Stores.InMemoryStore`; EF Core store base → `Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore`.
> - `TenantInfo`/`TenantExtendedInfo` no longer carry a bare `ConnectionString` (removed upstream at Finbuckle v7+). Separate-database apps now resolve connections via `ITenantConnections`/named connection-string properties instead.

- Fixed: separate-database `AddDbContext` no longer silently falls back to `DefaultConnection` when no tenant (or no connection string) is resolved — throws `MultiTenantException` instead, since a legitimate tenant request silently hitting the wrong database is a data-isolation bug, not a safe default. For EF Core CLI commands (e.g. `dotnet ef migrations`), install `Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory` as already documented — that factory is picked up before this registration ever runs. The exception message includes that hint only when `IHostEnvironment.IsDevelopment()` is true; production stays generic.
- Fixed: `InitializeStore` for **new** apps no longer seeds a stray `ConnectionString` when a named connection property (or none) is what's actually on the tenant class.
- Fixed: pre-upgrade apps with a hand-locked `InitializeStore()` referencing a bare `ConnectionString` are auto-migrated the next time the Software Factory runs, matching whatever the app actually needs: retargeted to the extended tenant type — on both the `IMultiTenantStore<T>` lookup and the seeded `new T { … }` tenants — with the connection string preserved (separate-database only), rewritten to the app's named connection-string property (Cosmos/Mongo/MongoFramework/GoogleCloudStorage installed), or removed outright (no extended type applies). The migration now targets the tenant class the Software Factory is about to (re)generate for the app's current settings, rather than the type still present in the not-yet-regenerated file — so a genuine v5→v6 separate-database upgrade correctly retargets `TenantInfo` → `TenantExtendedInfo` and keeps the connection string instead of stripping it.

### Version 6.0.0-pre.6

- Fixed: `TenantExtendedInfo` and `MultiTenantStoreDbContext` templates now emit `using Finbuckle.MultiTenant.Abstractions;` alongside `using Finbuckle.MultiTenant;`. In Finbuckle 9.x, `TenantInfo` moved from `Finbuckle.MultiTenant` to `Finbuckle.MultiTenant.Abstractions`, causing `CS0246` compilation errors in separate-database scenarios where `TenantExtendedInfo` extends `TenantInfo` and implements `ITenantConnections`.

### Version 5.2.2

- Improvement: Updated NuGet package versions.

### Version 5.2.1

- Improvement: Updated setting name and hint descriptions.

### Version 5.2.0

- Improvement: Updated to work with Microsoft.OpenApi (2.4.1 and up) library version.
- Improvement: Supported module updates.

### Version 5.1.21

- Improvement: Updated NuGet package versions.

### Version 5.1.20

- Improvement: Improved error message around unsupported scenarios.

### Version 5.1.19

- Improvement: Updated NuGet package versions.

### Version 5.1.18

- Fixed: ProjectUrl link.

### Version 5.1.17

- Improvement: Updated module documentation to use centralized documentation site.

### Version 5.1.16

- Improvement: Updated NuGet package versions.

### Version 5.1.15

- Improvement: Updated NuGet package versions.
- Improvement: Updated documentatopn topics `tags` format

### Version 5.1.14

- Improvement: Updated documentation added help topic.

### Version 5.1.13

- Improvement: Updated NuGet package versions.

### Version 5.1.12

- Improvement: Updated NuGet package versions.

### Version 5.1.11

- Improvement: Updated NuGet package versions.
- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 5.1.10

- Improvement: Updated NuGet package versions.
- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5.

### Version 5.1.9

- Improvement: Updated NuGet package versions.

### Version 5.1.8

- Improvement: Updated NuGet package versions.

### Version 5.1.7

- Improvement: Included module help topic.

### Version 5.1.6

- Improvement: Updated NuGet package versions.

### Version 5.1.5

- Improvement: Added support for multitenancy `route strategy`

### Version 5.1.4

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.1.3

- Improvement: Updated NuGet packages to latest stables.

### Version 5.1.2

- Improvement: Updated Interoperable dependency versions.

### Version 5.1.1

- Fixed: Various hosting registration issues for minimal hosting model.

### Version 5.1.0

- Improvement: Raises errors when EF model contain `Multi Tenant` stereotypes on composite/owned entities with advice on how to correct the model.
- Improvement: Updated to be compatible with .NET 8.

### Version 5.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).

### Version 4.1.8

- Improvement: Removed code to inject a `null` parameter value when the `Intent.EntityFrameworkCore.DesignTimeDbContextFactory` is present.

### Version 4.1.7

- Improvement: Upgraded Finbuckle NuGet package versions to `6.12.0`.
- Improvement: `HTTP Remote` is now available as a store option, see [here](https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#http-remote-store) for more information.

### Version 4.1.5

- Improvement: Fix up based on change made in `Intent.EntityFrameworkCore.DesignTimeDbContextFactory`.

### Version 4.1.3

- Improvement: All EF Core nuget packages are now updated to use the latest versions to date.

### Version 4.1.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- New Feature: Integration with Swagger to add the Tentant Id HTTP Header if that Strategy is chosen.
- New Feature: Multi-tenancy now supports Shared Database isolation.

### Version 4.0.1

- Fixed: When `Multitenancy Settings`' `Store` was set to `Entity Framework Core` and the `Database Settings`' `Database Provider` was set to something other than `In Memory`, a required NuGet package was not installed causing a compilation error.

### Version 4.0.0

- New Feature: Upgraded Templates to use new Builder Pattern paradigm.
