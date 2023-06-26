### Version 5.2.1

- Fixed: Ensure that the `IExceptionFilter` doesn't clash with another library's equivalent interface (such as MassTransit).

### Version 5.2.0

- FileBuilder metadata updated.
- Updated contract to include Api Version information.
- Introduced ExceptionFilter to deal with Exceptions that need to translate into an HTTP response.

### Version 5.1.1

- Have controllers fully manage using statements by default.

### Version 5.1.0

- Service Operations can now have Default values.
- Fixed: When only a minimalist set of modules was installed, `Application.Contract.Dto` was missing as a type source for controllers.

### Version 5.0.5

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 5.0.4

- Fixed: Security being applied to operations that have the Unsecured stereotype applied.

### Version 5.0.3

- Refactored to use `IHttpEndpoint` abstraction available from `Intent.Modelers.Types.ServiceProxies` version 4+.

### Version 5.0.1

- Moved interoperability dependencies for Dispatch.MediatR and Dispatch.Services to this module.

### Version 5.0.0

- Switched to abstract model type IControllerModel for ControllerTemplate, allowing support for Controllers being created from different sources.
- Fixed: Operations which returned nullable DTOs would generate uncompilable `typeof(SomeDto?)` for controller operation attributes.

### Version 4.0.2

- Fixed: Errors thrown when applying `application/json` to the Return Type Mediatype the Http Settings of an operation.
- Adds `[Produces(MediaTypeNames.Application.Json)]` when explicitly specified in the Http Settings.

### Version 4.0.1

- Fixed: Applying unsecured to a service would have no effect, it would need to be applied to individual operations.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.14

- Add `GetOperationAttributes` to `ControllerDecorator`.

### Version 3.3.13

- Add description to module.


### Version 3.3.12

- Update: Updated Intent.Metadata.WebApi that will no longer automatically apply HttpSettings stereotypes but will auto add them using event scripts.

### Version 3.3.11

- New: Http Settings' Return Type Mediatype setting will determine if the primitive return type should be wrapped in a JsonResponse object or not.
- Fixed: Controllers will now add usings for enums.

### Version 3.3.10

- Update: Decorators can add attributes to controllers.
- Fixed: Controller actions that made use of Http Headers didn't specify header names.