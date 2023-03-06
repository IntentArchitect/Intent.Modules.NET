### Version 4.1.0

- New: Integration with Swagger to add the Tentant Id HTTP Header if that Strategy is chosen.
- New: Multi-tenancy now supports Shared Database isolation.

### Version 4.0.1

- Fixed: When `Multitenancy Settings`' `Store` was set to `Entity Framework Core` and the `Database Settings`' `Database Provider` was set to something other than `In Memory`, a required NuGet package was not installed causing a compilation error.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.
