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
