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
