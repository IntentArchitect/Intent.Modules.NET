### Version 5.0.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 5.0.3

- Refactored to use `IHttpEndpoint` abstraction available from `Intent.Modelers.Types.ServiceProxies` version 4+.
- Fixed return type of "create" commands on controllers to return `JsonResponse<T>` as opposed to `{ "id": value }`.

### Version 4.1.0

- Update: Module renamed from `Intent.AspNetCore.Controllers.Interop.MediatR`.
- Update: Catered for scenario for our Auto CRUD implementation to auto-wireup a Query field.
- Controllers will now return a 404 Not Found response for non-collection null results.
