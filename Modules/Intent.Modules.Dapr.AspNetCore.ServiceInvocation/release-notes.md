﻿### Version 2.2.0

- Improvement: Underlying proxy templates updated to support alternate Metadata models.

### Version 2.1.0

- Improvement: Added support for configuring headers in HttpClient requests either through request header propagation.
- Improvement: Dapr HttpClient construction is now done using the HttpClientBuilder paradigm, to better support extension.

### Version 2.0.8

- Fixed: Bug in HttpClient where the query string was not correctly handling a collection type.

### Version 2.0.7

- Improvement: Will now respect query string parameter names as introduced in [`Intent.Metadata.WebApi` version `4.3.2`](https://github.com/IntentArchitect/Intent.Modules/blob/development/Modules/Intent.Modules.Metadata.WebApi/release-notes.md#version-432).

### Version 2.0.6

- Improvement: Will now generate `PagedResult<TData>` if used by any mapped service proxies.

### Version 2.0.5

- Improvement: Enhanced the way the generated proxies are dealing with DTO parameters in the query to align with ASP.Net Core behaviour.

### Version 2.0.4

- Improvement: Changed default proxy encoding to UTF8 as Default can be inconsistent(https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.default).
- Improvement: Added better out the box support for generating proxies into `netstandard` projects.

### Version 2.0.3

- Improvement: Renamed the `request` variable as it would conflict with developers who would use that name for their service parameters.

### Version 2.0.2

- Fixed: `ReadToEndAsync` doesn't have a parameter in .NET 6 to receive a `CancellationToken`.

### Version 2.0.1

- Fixed: Nullability related compiler warnings.
- Fixed: Dependency on `Intent.Application.Contracts.Clients` was not for version `.5.0.0`.

### Version 2.0.0

> ⚠️ **NOTE**
>
> This module depends on a new major version of `Intent.Application.Contracts.Clients` which may cause breaking changes to your codebase. Refer to [its release notes](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.Application.Contracts.Clients/release-notes.md#version-500) for more information.

- Updated to use latest Service Proxy modules.

### Version 1.1.0

- Service references will allow operation selection for generating DAPR HTTP clients.

### Version 1.0.3

- Add nullable annotations to reduce C# warnings generated when compiling.

### Version 1.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.1

- Refactored to use `IHttpEndpoint` abstraction available from `Intent.Modelers.Types.ServiceProxies` version 4+.
