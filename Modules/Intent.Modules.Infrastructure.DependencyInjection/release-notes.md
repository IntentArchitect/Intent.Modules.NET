### Version 4.1.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.1.1

- Improvement: Updated NuGet packages to latest stables.

### Version 4.1.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 4.0.8

- Improvement: Updated to be compatible with .NET 8.

### Version 4.0.7

- Improvement: Updated nuget packages for .NET 8 framework readiness.

### Version 4.0.6

- Improvement: Added Nuget Dependency `Microsoft.Extensions.Configuration.Binder`.

### Version 4.0.5

- Fixed: Removed various compiler warnings.

### Version 4.0.4

- Fixed: Under certain circumstances the generated solution would not compile due to the `Microsoft.Extensions.DependencyInjection` NuGet package not being installed.

### Version 4.0.3

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.2

- Fixed: Added Nuget Dependency `Microsoft.Extensions.Configuration.Abstractions`.

### Version 4.0.1

- Fixed: Didn't take "requires namespaces" into account.

### Version 4.0.0

- New Feature: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.8

- Improvement: Dependency injection configuration now receives event updates from `ServiceConfigurationRequest` and not just from `ContainerRegistrationRequest`.
