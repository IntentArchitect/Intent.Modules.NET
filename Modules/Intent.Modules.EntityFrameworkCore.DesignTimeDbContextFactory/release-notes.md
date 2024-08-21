### Version 4.2.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.2.1

- Improvement: Updated NuGet packages to latest stables.

### Version 4.2.0

- Improvement: Added support for Oracle db configurations.
- Improvement: Only installs when ApplicationDbContext is present.

### Version 4.1.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 4.0.7

- Improvement: Updated to be compatible with .NET 8.
- Improvement: Extended ConfigurationBuilder to additionally load settings from environment variables, or user secrets. (with thanks to [@shainegordon](https://github.com/shainegordon) for their [PR](https://github.com/IntentArchitect/Intent.Modules.NET/pull/13) for this).


### Version 4.0.6

- Improvement: Now listens for `DbMigrationsReadMeCreatedEvent` and mutates the MIGRATION_README file instead of generating its own DESIGN_TIME_MIGRATION_README.
- Improvement: Automatically adds `null` parameters to instantiating the `ApplicationDbContext` where the constructor expects arguments to be supplied which got introduced by Intent Architect modules.

### Version 4.0.5

- Improvement: Module updates.

### Version 4.0.4

- Fixed: The DbContext can be created dynamically.

### Version 4.0.3

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.2

- Improvement: Updated supported client version to [3.4.0-pre.0, 5.0.0).

### Version 4.0.1

- Improvement: Added support for MySQL as DB provider.

### Version 4.0.0

- New Feature: Added ability to still run EF Core migration scripts when your startup application is hindering you to do so.

