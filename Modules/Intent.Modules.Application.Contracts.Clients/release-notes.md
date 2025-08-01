### Version 5.1.2

- Improvement: Will now generate service contracts for direct invocations of services in other applications.

### Version 5.1.1

- Fixed: `Create` method on `DtoContracts` will now used the default values from the original `Command/Query`.

### Version 5.1.0

- Fixed: Introduce the new WebAPI module to make use of the Secure by Default setting for HTTP Clients but also to incorporate it for Controllers.

### Version 5.0.10

- Improvement: Updated module icon

### Version 5.0.9

- Fixed: Possible PagedResult duplication through DTOs as opposed to only using the PagedResult template. 

### Version 5.0.8

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.7

- Improvement: Added support for gathering Service Proxy information from the Services designer too.

### Version 5.0.6

- Improvement: Improved internal processes for working with Service Proxies.

### Version 5.0.5

- Improvement: Updating various modules to use the more sophisticated CSharp mapping resolution systems.

### Version 5.0.4

- Improvement: Updated version for `Intent.Metadata.WebApi` dependency.

### Version 5.0.3

- Improvement: Will now generate `PagedResult<TData>` if used by any mapped service proxies.

### Version 5.0.2

- Improvement: Description Attributes can be applied to `Enum` literals through the usage of the Description Stereotype.

### Version 5.0.1

- Fixed: Nullability related compiler warnings.

### Version 5.0.0

> ⚠️ **NOTE**
>
> This module update will change the locations and namespaces of already generated service proxy `DTO`s and `Enum`s. If you are using these `DTO`s or `Enum`s in files not generated by this module, then you will likely encounter compilation errors due to now out of date using directives.

- Updated to use latest Service Proxy modules.
- Because it's possible for multiple service proxies to use a particular `DTO` or `Enum`, they are now generated into a folder structure per their source package.
- The `Template Output`s for `DTO` and `Enum` templates will now by default be placed in a `Contracts` folder in the designer. If you are upgrading this module from an earlier version then you may want to consider manually moving these `Template Output`s into a `Contracts` folder in the `Visual Studio` Designer.
- Service Interfaces will no longer be suffixed with `Service` or have suffixes automatically removed.


### Version 4.1.0

- Service references will allow operation selection for generating proxy client contracts.

### Version 4.0.2

- Removed dependency on Domain module.
- Migrated remaining templates to use CSharpFileBuilder.
- Add nullable annotations to reduce C# warnings generated when compiling.

### Version 4.0.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.0

- Updated to work with `Intent.Metadata.WebApi` version 4+.

### Version 3.3.11

- Enums will now generate comments captured in designers.

### Version 3.3.10

- Generated DTOs now respect the `Serialization Settings` stereotype.
- Used Enums will now also be generated.

### Version 3.3.8

- Fixed: Operation name suffix "Async" added.

### Version 3.3.7

- New: HttpClientRequestException added which contains its own response content in error scenarios.
- Update: Leveraging new internal interface for obtaining service proxy information.

### Version 3.3.6

- Update: Generates code from referenced services modeled in `Service Proxies` designer in the form of a Service Interface (that is fully `async`/`await` enabled) and DTO classes (including Commands and Queries).
