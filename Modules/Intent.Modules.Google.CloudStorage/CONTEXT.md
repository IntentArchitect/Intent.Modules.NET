# Intent.Modules.Google.CloudStorage — CONTEXT

## EF Core + separate-storage-account coexistence fix-up

When `Google Cloud Storage Data Isolation` is `separate-storage-account`, this module publishes a
`MultitenantConnectionStringRegistrationRequest("GoogleCloudStorageConnection", ...)` so
`Intent.Modules.AspNetCore.MultiTenancy`'s `TenantExtendedInfoTemplate` adds a named
`GoogleCloudStorageConnection` property to the tenant class instead of the generic `ConnectionString`
property (the two are mutually exclusive by that template's design).

This breaks `AspNetCore.MultiTenancy`'s own EF Core integration
(`AspNetCoreIntegrationExtension.GetSeparateDatabaseDataIsolationConfiguration`, which injects tenant-aware
`AddDbContext` code into `Infrastructure.DependencyInjection`) whenever an app **also** has an EF Core
`DbContext` under separate-database isolation — that integration unconditionally assumes
`tenantInfo?.ConnectionString` exists, since it has no idea this module (or any other) might have claimed a
named connection instead.

**The fix intentionally lives here, not in `AspNetCore.MultiTenancy`.** That module depends on nothing
downstream of it (CosmosDB/MongoDb/MongoFramework/GoogleCloudStorage all depend on *it*, never the reverse),
and it must stay that way — hardcoding awareness of this module's specific setting into the shared
multitenancy module was tried and reverted; see `Intent.Modules.AspNetCore.MultiTenancy/CONTEXT.md` item 5.
Since this module already depends on `AspNetCore.MultiTenancy` and is the one that introduces the
conflict, `GoogleCloudStorageConfigurationTemplatePartial.FixUpEntityFrameworkCoreConnectionResolution()`
patches the generated statement after the fact:

- Finds `Infrastructure.DependencyInjection`'s own `AddInfrastructure` method (only when an EF Core
  `DbContext` template instance also exists and `DataIsolation` is `separate-database`).
- Registers an `AfterBuild` callback at **priority 1000** (the "Final" band) on that file, so it runs after
  `AspNetCore.MultiTenancy`'s own default-priority `AfterBuild` has already inserted the
  `tenantInfo?.ConnectionString` statement.
- Find-and-replaces both the connection expression (`ConnectionString` → `Identifier`, since the EF InMemory
  DbContext just needs *a* per-tenant key, not a real connection string) and the preceding comment, which
  would otherwise describe the wrong behavior.

If a future app combines EF Core with CosmosDB/MongoDb/MongoFramework's own named-connection isolation
instead, the same pattern (a fix-up living in *that* module) should be used — never taught to
`AspNetCore.MultiTenancy` directly.

**Verified**: `Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests` (the combined EF Core + separate
storage-account scenario) and `Finbuckle.SeparateDatabase.TestApplication` (pure EF Core, confirms no
regression) both build clean with zero outstanding Software Factory changes. Not runtime-tested — requires
developer-provided GCS credentials.
