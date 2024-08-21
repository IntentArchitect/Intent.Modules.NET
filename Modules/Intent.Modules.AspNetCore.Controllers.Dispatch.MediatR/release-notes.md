### Version 5.5.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.5.2

- Improvement: Support for modeled security `Role`s and `Policy`s
- Improvement: Controllers and their operations will now generate `[ApiExplorerSettings(IgnoreApi = <value>]` attributes based on the _OpenAPI Settings_ Stereotype's _Ignore_ property's value.

### Version 5.5.1

- Improvement: File transfer ( upload / download ) support.

### Version 5.5.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 5.4.2

- Improvement: Added Support for `+` in roles, to describe `and` relationships between roles e.g. `Admin,Manager` (or) vs `Admin+Manager` (and)

### Version 5.4.1

- Improvement: Will now respect query string parameter names as introduced in [`Intent.Metadata.WebApi` version `4.3.2`](https://github.com/IntentArchitect/Intent.Modules/blob/development/Modules/Intent.Modules.Metadata.WebApi/release-notes.md#version-432).

### Version 5.4.0

- Improvement: When Controller parameters match with Update Command fields, we've made consuming these endpoints easier. Now, you don't need to populate the fields on the Command that are already populated via a Route parameter.

### Version 5.3.0

- Fixed: Controller methods which return nullable types will no longer return `NotFound()` for `null` results. To force that a `404 Not Found` HTTP response be returned, a `throw new NotFoundException("<message>")` can be performed as appropriate.
- Fixed: Controller methods with a _Return Type Mediatype_ of `application/json` would under certain circumstances not wrap scalar types in a `JsonResponse<T>`.

### Version 5.2.2

- Fix: CQRS `Authorization` Stereotypes now adding configured roles to the Controller too.

### Version 5.2.0

- Updated Controller generation so that nested folders are also taken into account for the controller name.
- Updated internal components to allow versioning information to be specified for AspNetCore Controllers for Commands and Queries.

### Version 5.1.1

- Updated: using constructors to instantiate `Command`s and `Query`s. This was a change in pattern to reduce compiler warnings.

### Version 5.1.0

- Support default values parameters for Domain Entities, Domain Services, and Service Controllers.

### Version 5.0.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 5.0.3

- Refactored to use `IHttpEndpoint` abstraction available from `Intent.Modelers.Types.ServiceProxies` version 4+.
- Fixed return type of "create" commands on controllers to return `JsonResponse<T>` as opposed to `{ "id": value }`.

### Version 4.1.0

- Update: Module renamed from `Intent.AspNetCore.Controllers.Interop.MediatR`.
- Update: Catered for scenario for our Auto CRUD implementation to auto-wireup a Query field.
- Controllers will now return a 404 Not Found response for non-collection null results.
