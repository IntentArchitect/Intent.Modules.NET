### Version 4.1.5

- Update: Fix up based on change made in `Intent.EntityFrameworkCore.DesignTimeDbContextFactory`.

### Version 4.1.3

- Update: All EF Core nuget packages are now updated to use the latest versions to date.

### Version 4.1.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- New: Integration with Swagger to add the Tentant Id HTTP Header if that Strategy is chosen.
- New: Multi-tenancy now supports Shared Database isolation.

### Version 4.0.1

- Fixed: When `Multitenancy Settings`' `Store` was set to `Entity Framework Core` and the `Database Settings`' `Database Provider` was set to something other than `In Memory`, a required NuGet package was not installed causing a compilation error.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.
