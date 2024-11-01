### Version 4.2.1

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.2.0

- Improvement: Module project updated to .NET 8.
- Fixed: Enums can now be supplied as route parameters and parsed correctly.

### Version 4.1.7

- Improvement: Updated icon to SVG format.

### Version 4.1.3

- Update: Improved Queue Trigger support, including receiving messages as `QueueMessage`.
- Update: Passes through cancellation tokens where appropriate.
- Update: Proper return statement handling for when code gets wrapped.

### Version 4.1.1

- Fix: Azure Route info for `Http Tigger` on `ServiceOperations`.

### Version 4.1.0

- Support default values parameters for Domain Entities, Domain Services, and Service Controllers.

### Version 4.0.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Support for exposing Service Operations as Azure Functions through the use of Stereotypes.

### Version 4.0.0

- Support for Intent.AzureFunctions 4.0.0.

## Version 3.3.8

- New: Returning a primitive type with the media type set to application/json will wrap that value in a type that will make consuming that service possible when deserializing from json text.
