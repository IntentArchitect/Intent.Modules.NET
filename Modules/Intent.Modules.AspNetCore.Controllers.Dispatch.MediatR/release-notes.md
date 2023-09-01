### Version 5.3.0

- Improvement: Controller methods will no longer return `NotFound()` for `null` results. To have a `404 Not Found` HTTP response be returned, a `throw new NotFoundException("<message>")` can be performed as appropriate.
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
