### Version 1.1.1

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.1.0

- Improvement: Module project updated to .NET 8.

### Version 1.0.10

- Improvement: Updated icon to SVG format.

### Version 1.0.8

- Update: Improved Queue Trigger support, including receiving messages as `QueueMessage`.
- Update: Proper return statement handling for when code gets wrapped.

### Version 1.0.6

- Fix: Query parameters were not being dispatched correctly to CommnadHandlers for `Query`'s.

### Version 1.0.6

- Fix: Added additional MediatR module dependencies so that installing this module works out the box.

### Version 1.0.5

- Added the necessary File Builder metadata in order to indicate which line of code has to do with service dispatching. This is important for other modules that want to hook-into that space.

### Version 1.0.3

- Fix: Azure Route info for `Http Trigger` on `ServiceOperations`.
- Update: Changed dispatcher to use constructors, inline with `Command`s and `Queries`s not having parameterless constructors.

### Version 1.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Support for exposing Commands and Queries as Azure Functions through the use of Stereotypes.

### Version 4.0.0

- Support for dispatching Command and Queries from Azure Functions by mapping the Azure Function onto the Command or Query to be dispatched.
