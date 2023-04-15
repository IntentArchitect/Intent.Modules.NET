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