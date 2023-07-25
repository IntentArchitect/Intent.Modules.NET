### Version 4.0.5
- Update: Made the `DependencyInjectionTemplate` AfterBuild run later to better accommodate other templates wanting to interact with it during the AfterBuild step .


### Version 4.0.4
- Update: Removed various compiler warnings.

### Version 4.0.3

- Fixed: Under certain circumstances the generated solution would not compile due to the `Microsoft.Extensions.DependencyInjection` NuGet package not being installed.

### Version 4.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Updated supported client version to [3.3.0-pre.0, 5.0.0).

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.10

- Fixed: Didn't take "requires namespaces" into account and logic from Infrastructure around Container registration has been aligned. 

### Version 3.3.8

- Update: Dependency injection configuration now receives event updates from `ServiceConfigurationRequest` and not just from `ContainerRegistrationRequest`.