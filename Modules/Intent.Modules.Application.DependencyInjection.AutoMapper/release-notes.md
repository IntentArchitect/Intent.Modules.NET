### Version 4.0.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.0.1

- Improvement: Removed the `AutoMapper.Extensions.Microsoft.DependencyInjection` NuGet package and replaced it with `AutoMapper` version 13.0.1 which encapsulates the same functionality.

### Version 4.0.0

- Improvement: Updated the `AutoMapper.Extensions.Microsoft.DependencyInjection` NuGet package version to 12.0.1.

> ⚠️ **NOTE**
>
> This module update may cause a compilation break around legacy  NuGet package references to `AutoMapper.Extensions.Microsoft.DependencyInjection v7.0.*`, these references can simply be removed or upgraded to `12.0.1` if they are required.

### Version 3.4.5

- Improvement: Changed version of `AutoMapper.Extensions.Microsoft.DependencyInjection` NuGet package from `7.0.*` to `7.0.0` as floating versions are not supported by [Central Package Management](https://learn.microsoft.com/nuget/consume-packages/central-package-management).

### Version 3.4.4

- Reverted updates of `3.4.3` as no longer required.

### Version 3.4.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.1

- Updated supported client version to [3.3.0-pre.0, 5.0.0).
