### Version 6.1.1

- Fixed: `TypeSchemaFilterExtension` class moved from `Intent.AspNetCore.Controllers` to `Intent.AspNetCore.Swashbuckle` module where it belongs as other hosting infrastructure using Swashbuckle can also benefit from it.

### Version 6.1.0

- Fixed: Removed Legacy behaviour where `Services` without `Http Service Settings` stereotypes would result in interpreting the Operations with `Http Settings` to have a base route of `api/[controller]`.

### Version 6.0.12

- Improvement: Added support for customizing `Success Response Code`.
- Fixed: Snag found in the code responsible for generating the AddControllers line in the Startup routine especially when `AddJsonOptions` were introduced.

### Version 6.0.11

- Improvement: Updated module NuGet packages infrastructure.

### Version 6.0.10

- Improvement: The code responsible for generating the AddControllers line in the Startup routine has been enhanced to better identify that statement at runtime.

### Version 6.0.9

- Improvement: Controllers and their operations will now generate `[ApiExplorerSettings(IgnoreApi = <value>]` attributes based on the _OpenAPI Settings_ Stereotype's _Ignore_ property's value.
- Fixed: Issue where Policies where not added to controllers.

### Version 6.0.8

- Fixed: Due to the nature of how GetTypeName resolves namespaces there are cases where ambiguous references still exist and causes compilation errors, this fix forces to re-evaluate a lot of types in this module.

### Version 6.0.7

- Improvement: Application Client Dto type using directives also to be resolved now in Service implementations.

### Version 6.0.6

- Fixed: `ExceptionFilter` will now return `UnauthorizedResult` when `UnauthorizedAccessException` is caught.

### Version 6.0.5

- Improvement: Swagger Schema Filter support, specifically for `DateOnly`.

### Version 6.0.4

- Improvement: Swagger Schema Filter support, specifically for `TimeSpan`.

### Version 6.0.3

- Improvement: File transfer ( upload / download ) support.
- Improvement: Added Service Designer validation - `GET Operations do not support collections on complex objects`.

### Version 6.0.2

- Improvement: Dependent modules changed, version bumped.

### Version 6.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).
- Improvement: Added setting for configuring controller JSON serialization options. (with thanks to [@shainegordon](https://github.com/shainegordon) for their [PR](https://github.com/IntentArchitect/Intent.Modules.NET/pull/11) for this).
- Improvement: Added Support for `+` in roles, to describe `and` relationships between roles e.g. `Admin,Manager` (or) vs `Admin+Manager` (and)

- Fixed: Roles and policies not being applied to individual endpoints when the `API Settings` have the `Default API Security` set to `Secured by default`
 

### Version 5.4.4

- Improvement: Added an `IControllerTemplate` interface for custom dispatch support.

### Version 5.4.3

- Improvement: Will now respect query string parameter names as introduced in [`Intent.Metadata.WebApi` version `4.3.2`](https://github.com/IntentArchitect/Intent.Modules/blob/development/Modules/Intent.Modules.Metadata.WebApi/release-notes.md#version-432).

### Version 5.4.2

- Improvement: Now respects _OpenAPI Settings_ stereotype's _OperationId_ property for customizing the `operationId` for an endpoint, requires at least version `4.3.1` of the `Intent.Metadata.WebApi` module.

### Version 5.4.1

- Improvement: Internal model update for better identifying Controller Templates during Software Factory Execution.

### Version 5.4.0

- Improvement: The heuristic for whether or not to annotate that particular controller methods can return a 404 response has been improved.
- Improvement: One can now specify a Default Route Prefix for API Services.
- Fixed: Controller methods which would return `JsonResponse<T>` incorrectly had a return type of `ActionResult<T>` instead of `ActionResult<JsonResponse<T>>`.

### Version 5.3.2

- Improvement: `ExceptionFilter` will also look out for `UnauthorizedAccessException` exceptions for being thrown and returned as `FORBIDDEN` HTTP result types. 

### Version 5.3.0

- Improvement: Ensure that Swagger will know about HTTP 500 errors returning `ProblemDetails`.

### Version 5.2.1

- Fixed: Ensure that the `IExceptionFilter` doesn't clash with another library's equivalent interface (such as MassTransit).

### Version 5.2.0

- Improvement: FileBuilder metadata updated.
- Improvement: Updated contract to include Api Version information.
- Improvement: Introduced ExceptionFilter to deal with Exceptions that need to translate into an HTTP response.

### Version 5.1.1

- Improvement: Have controllers fully manage using statements by default.

### Version 5.1.0

- Improvement: Service Operations can now have Default values.
- Fixed: When only a minimalist set of modules was installed, `Application.Contract.Dto` was missing as a type source for controllers.

### Version 5.0.5

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 5.0.4

- Fixed: Security being applied to operations that have the Unsecured stereotype applied.

### Version 5.0.3

- Improvement: Refactored to use `IHttpEndpoint` abstraction available from `Intent.Modelers.Types.ServiceProxies` version 4+.

### Version 5.0.1

- Improvement: Moved interoperability dependencies for Dispatch.MediatR and Dispatch.Services to this module.

### Version 5.0.0

- Improvement: Switched to abstract model type IControllerModel for ControllerTemplate, allowing support for Controllers being created from different sources.
- Fixed: Operations which returned nullable DTOs would generate uncompilable `typeof(SomeDto?)` for controller operation attributes.

### Version 4.0.2

- Fixed: Errors thrown when applying `application/json` to the Return Type Mediatype the Http Settings of an operation.
- Improvement: Adds `[Produces(MediaTypeNames.Application.Json)]` when explicitly specified in the Http Settings.

### Version 4.0.1

- Fixed: Applying unsecured to a service would have no effect, it would need to be applied to individual operations.

### Version 4.0.0

- New Feature: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.14

- Improvement: Add `GetOperationAttributes` to `ControllerDecorator`.

### Version 3.3.13

- Improvement: Add description to module.


### Version 3.3.12

- Improvement: Updated Intent.Metadata.WebApi that will no longer automatically apply HttpSettings stereotypes but will auto add them using event scripts.

### Version 3.3.11

- New Feature: Http Settings' Return Type Mediatype setting will determine if the primitive return type should be wrapped in a JsonResponse object or not.
- Fixed: Controllers will now add usings for enums.

### Version 3.3.10

- Improvement: Decorators can add attributes to controllers.
- Fixed: Controller actions that made use of Http Headers didn't specify header names.
