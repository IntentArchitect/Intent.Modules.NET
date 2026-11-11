### Version 1.0.0

- Fixed: when an application combines a separate-storage-account Google Cloud Storage configuration with an EF Core `DbContext` under separate-database multi-tenancy, the generated `DependencyInjection.AddInfrastructure` no longer fails to compile. Claiming `GoogleCloudStorageConnection` as the tenant class's named connection means `AspNetCore.MultiTenancy`'s own EF Core integration can no longer assume a generic `ConnectionString` property exists on that class; this module now patches that generated statement to use the tenant `Identifier` as the per-tenant database key instead.
- Improvement: Updated NuGet package versions:
  - `Google.Cloud.Storage.V1` 4.15.0 → 5.0.0

  Google.Cloud.Storage.V1 5.0 removes `UploadValidationException` and `UploadValidationMode.ThrowOnly` and changes the default upload validation mode. The generated code does not use them; custom code that does should be checked against [the change](https://github.com/googleapis/google-cloud-dotnet/pull/15769).
- Improvement: Included module help topic. New Feature: Module release.
