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
