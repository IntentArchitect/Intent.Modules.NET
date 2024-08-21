### Version 4.1.2

- Improvement: Updated module NuGet packages infrastructure.
- Improvement: Updated templates to use new NuGet package system.

### Version 4.1.1

- Improvement: Updated NuGet packages to latest stables.

### Version 4.1.0

- Improvement: Upgraded module to support new 4.1 SDK features.
- 
### Version 4.0.7

- Improvement: Updated to be compatible with .NET 8.

### Version 4.0.6

- Improvement: `AddApplication` extension method will now receive `IConfiguration` as a parameter.

### Version 4.0.5
- Improvement: Made the `DependencyInjectionTemplate` AfterBuild run later to better accommodate other templates wanting to interact with it during the AfterBuild step .

### Version 4.0.4
- Improvement: Removed various compiler warnings.

### Version 4.0.3

- Fixed: Under certain circumstances the generated solution would not compile due to the `Microsoft.Extensions.DependencyInjection` NuGet package not being installed.

### Version 4.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Fixed: Updated supported client version to [3.3.0-pre.0, 5.0.0).

### Version 4.0.0

- New Feature: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.10

- Fixed: Didn't take "requires namespaces" into account and logic from Infrastructure around Container registration has been aligned. 

### Version 3.3.8

- Improvement: Dependency injection configuration now receives event updates from `ServiceConfigurationRequest` and not just from `ContainerRegistrationRequest`.
