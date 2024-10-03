### Version 5.0.16

- Improvement: Basic CRUD support for specifications.

### Version 5.0.15

- Fixed: Issue where `CRUD Update Action` was not generating correct code if Domain Services were injected.
- Fixed: Query with nullable mapped property, which is paginated generates uncompilable code.

### Version 5.0.14

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.13

- Improvement: Support for nullable `OrderBy` on Paging.

### Version 5.0.12

- Improvement: Support for `OrderBy` on Paging.

### Version 5.0.11

- Improvement: FindAsync handling more mapping scenarios.
- Improvement: Handler implementation that involves operation invocations returning non-void types will now return appropriate variable declaration types such as `var myVar = ...;` or `var (prop1, prop2, prop3) = ...;`.

### Version 5.0.10

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 5.0.9

- Improvement: Added basic support non UOW based repositories.

### Version 5.0.8

- Improvement: Add support for Data Contract mappings with the Advanced mapping system.
- Fixed: Service Call Operations can also now invoke `Operations` on a `Class`.
- Fixed: Service dependency substitutions can also occur on parameters of a Service Call Operation.

### Version 5.0.7

- Improvement: Adding mapped literals for `Query`s caused software factory errors this is now supported.
- Improvement: Service calls returning DTO's with primary keys on newly created entities now force a UOW save to ensure key is populated.

### Version 5.0.6

- Fixed: Issue around CRUD with nullable return types.

### Version 5.0.5

- Improvement: `DomainInteractionsManager` - Consolidated CallServiceOperation enhancements include refined service dependency checks, extended support for ClassModel, DataContractModel, and TypeDefinitionModel through VariableType, and robust handling of asynchronous operations with proper awaiting and cancellation token integration, improving clarity and flexibility in service operation calls.

### Version 5.0.4

- Improvement: Domain Event Handlers will now by default have `[IntentManaged(Mode.Fully, Body = Mode.Fully)]` in all circumstances, meaning that for cases where the file was originally generated before a mapping was created, the mapping will now get generated on subsequent Software Factory generations after the mapping was added.

### Version 5.0.3

- Improvement: Added support for `Value Object` collections.
- Fixed: Domain Event handlers sometimes ended up with a `throw new NotImplementedException...` when there was an implementation.

### Version 5.0.2

- Fixed: Optional query filters not working with DbContext oriented data-access.

### Version 5.0.1

- Improvement: Support for optional query filters when querying lists (or paginated lists) of entities.
 
### Version 5.0.0

- Support for explicit modeling of Domain Event Handlers and the Advanced Mapping capabilities available in Intent Architect 4.1.0

### Version 4.2.1

- `Publish` method on the domain event service now accepts a `CancellationToken`.

### Version 4.2.0

- Converted Default Domain Event Handler to use the C# Builder implementation paradigm. These classes can now be easily extended.

### Version 4.1.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.6

- Fixed: `async` keyword added to methods where only `Task` was returned.

### Version 3.3.4

- Fixed: Event handlers not respecting folder paths from the designer.
