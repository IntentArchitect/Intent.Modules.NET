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