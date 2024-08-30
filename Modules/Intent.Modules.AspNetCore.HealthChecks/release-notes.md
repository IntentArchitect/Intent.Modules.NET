### Version 2.0.9

- Fixed: Updated nuget `AspNetCore.HealthChecks.UI` to `8.0.2` which fixes various HealthChecks UI issues for .NET 8.

### Version 2.0.8

- Improvement: Updated module NuGet packages infrastructure.

### Version 2.0.7

- Improvement: Updated NuGet packages to latest stables.

### Version 2.0.6

- Improvement: Added support for Kafka.

### Version 2.0.5

- Fixed: Added warning regarding using Health Checks UI when running against .NET 8 due to the following issues:
    - [Icons missing after upgrading to v8.0.0](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/2130).
    - [[UI] Relative Address for HealthCheckEndpoint with Kestrel at http://0.0.0.0:0](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/410).

### Version 2.0.4

- Fixed: Disambiguation when multiple connection strings point to the same Persistence technology (i.e. SQL Server, Cosmos DB, etc.) by using the `connection string name` as the **health check entry name**. 

### Version 2.0.3

- Improvement: Updated Interoperable dependency versions.

### Version 2.0.2

- Improvement: Added support for Oracle databases.
- Improvement: Added support for Redis Stack.

### Version 2.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).
- Improvement: Updated to be compatible with .NET 8.

### Version 1.0.0

- New: Add Health checks to your ASP.NET Core app to monitor various aspects of your application to determine if is responding to requests normally.
