### Version 5.3.3

- Improvement: Updated NuGet package versions.

### Version 5.3.2

- Improvement: Updated module documentation to use centralized documentation site.

### Version 5.3.1

- Fixed: Removed unnecessary `AutoMapper` using directive for Dependency Injection.

### Version 5.3.0

- Improvement: Select whether to lock the version of the AutoMapper Nuget package to the one prior to the commercial version or proceed to use the commercial version accepting its license. Read the article [here](https://www.jimmybogard.com/automapper-and-mediatr-commercial-editions-launch-today/).

> ⚠️ NOTE
>
> If you decide to go with the commercial version you will need to obtain and specify the license key.
> This can be done by requesting one as indicated in the article above and then inserting it into the `appsettings.json` under `AutoMapper:LicenseKey` (or as an environment variable `AutoMapper__LicenseKey`).

### Version 5.2.2

- Improvement: Locked AutoMapper NuGet package version.

### Version 5.2.1

- Improvement: Added support for `Profile location` setting.

### Version 5.2.0

- Improvement: Updated NuGet package version, AutoMapper -> 14.0.

### Version 5.1.7

- Improvement: Added support for `Domain` and `Services` naming conventions for `Entities`, `Attributes` and `Operations`.

### Version 5.1.6

- Improvement: Updated NuGet package versions.

### Version 5.1.5

- Improvement: `MappingProfile` and `IMapFrom` files converted to use Builder pattern, while also improving the qualify of generated code.
- Fixed: `FindProjectToAsync` had the incorrect idenitifier name in some scenarios where the Primary Key name had been changed from `Id`.

### Version 5.1.4

- Improvement: Added additional `ProjectTo` operations on EF repositories.
- Fixed: Some ProjectTo overloads were not being generated on repositories.

### Version 5.1.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.1.2

- Improvement: Dependency bump.

### Version 5.1.1

- Improvement: Bumped AutoMapper to latest stable version `13.0.1`

### Version 5.1.0

- Improvement: Refactoring the pagination patterns to better suit both Repository and EF DbContext approaches.

### Version 5.0.0

- Improvement: Updated the `AutoMapper.Extensions.Microsoft.DependencyInjection` NuGet package version to 12.0.1.

> ⚠️ **NOTE**
>
> This module update may cause a compilation break around legacy NuGet package references to `AutoMapper.Extensions.Microsoft.DependencyInjection v7.0.*`, these references can simply be removed or upgraded to `12.0.1` if they are required.

- Improvement: Updated `AutoMapper` NuGet package to `12.0.1` 
- Improvement: Add an EF Core InterOp which adds AutoMapper  Projection Functionality to EF repositories.
- Improvement: Allow DTOs to be instantiated using a non-public constructor.

### Version 4.0.3

- Improvement: Removes nullability compiler warning in mapping profiles.

### Version 4.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Improvement: Updated supported client version to [3.4.0-pre.0, 5.0.0).

### Version 3.3.8

- Improvement: Updated `AutoMapper` NuGet package to `12.0.0` to resolve exception which occurs when running under .NET 7.0.
