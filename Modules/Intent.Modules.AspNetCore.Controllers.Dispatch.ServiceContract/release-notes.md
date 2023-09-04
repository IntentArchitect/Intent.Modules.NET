### Version 5.2.0

- Fixed: Controller methods which return nullable types will no longer return `NotFound()` for `null` results. To force that a `404 Not Found` HTTP response be returned, a `throw new NotFoundException("<message>")` can be performed as appropriate.
- Fixed: Controller methods with a _Return Type Mediatype_ of `application/json` would under certain circumstances not wrap scalar types in a `JsonResponse<T>`.

### Version 5.1.2

- Only publish events in controller methods that cause mutations.

### Version 5.1.1

- CancellationTokens are now passed in as a parameters for asynchronous operations.

### Version 5.1.0

- Support default values parameters for Domain Entities, Domain Services, and Service Controllers.

### Version 5.0.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 5.0.2

- Refactored to use `IHttpEndpoint` abstraction available from `Intent.Modelers.Types.ServiceProxies` version 4+.

### Version 4.1.0

- Update: Module combined from `Intent.AspNetCore.Controllers.Interop.Contracts` and `Intent.AspNetCore.Controllers.Interop.EntityFrameworkCore`.
- Update: Controllers will now return a 404 Not Found response for non-collection null results.