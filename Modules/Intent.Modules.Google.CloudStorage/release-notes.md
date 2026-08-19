### Version 1.0.0

- Fixed: when an application combines a separate-storage-account Google Cloud Storage configuration with an EF Core `DbContext` under separate-database multi-tenancy, the generated `DependencyInjection.AddInfrastructure` no longer fails to compile. Claiming `GoogleCloudStorageConnection` as the tenant class's named connection means `AspNetCore.MultiTenancy`'s own EF Core integration can no longer assume a generic `ConnectionString` property exists on that class; this module now patches that generated statement to use the tenant `Identifier` as the per-tenant database key instead.
- Improvement: Updated NuGet package versions.
- Improvement: Included module help topic.
- Improvement: Updated NuGet package versions.
New Feature: Module release.
