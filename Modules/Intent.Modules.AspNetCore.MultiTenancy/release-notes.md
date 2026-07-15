### Version 6.0.0

- **Breaking change**: Upgraded Finbuckle.MultiTenant from 6.13.1 to 9.4.10. .NET 6 and .NET 7 are no longer supported by generated multi-tenancy code — applications must target .NET 8 or later.
- **Breaking change**: Direct DI injection of `ITenantInfo`/`TenantInfo` is replaced by `IMultiTenantContextAccessor<T>` (read) and `IMultiTenantContextSetter` (write) throughout generated code, matching Finbuckle 9.x's API shape. Namespaces reorganized: `ITenantInfo`, `IMultiTenantContext(Accessor)<T>`, `IMultiTenantStore<T>`, and `ITenantResolver<T>` now live in `Finbuckle.MultiTenant.Abstractions`; `InMemoryStoreOptions<T>` moved to `Finbuckle.MultiTenant.Stores.InMemoryStore`; the EF Core store DbContext base moved to `Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore`.
- **Breaking change**: `TenantInfo`/`TenantExtendedInfo` no longer carry a bare `ConnectionString` property (removed upstream by Finbuckle at v7+). Separate-database applications now resolve per-tenant connections through `ITenantConnections`/named connection-string properties instead.
- Manual migration note: existing applications' hand-owned tenant seed data (`InitializeStore`/`SetupInMemoryStore`, generated with `Body = Ignore`) will not auto-migrate — any `ConnectionString = "..."` initializer on the seeded tenant object must be updated by hand to the new named connection-string property after updating this module and running the Software Factory.
- Fixed: separate-database `AddDbContext` tenant-connection resolution no longer throws at EF Core design time (e.g. during `dotnet ef migrations`) when no tenant is resolved; it now falls back to the `DefaultConnection` connection string in that case, matching the pre-upgrade design-time behavior.
- Fixed: the InMemory-store sample tenant seed (`InitializeStore`) generated for **new** applications no longer includes a stray `ConnectionString` initializer when a named connection-string property (or none at all) is what's actually generated on the tenant class. Previously `GetDefaultTenants()` always seeded `ConnectionString`, which failed to compile whenever a separate-database module (CosmosDB, MongoDb, MongoFramework, Google Cloud Storage) registered its own named connection instead.

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
