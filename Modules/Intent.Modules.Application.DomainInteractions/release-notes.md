### Version 1.1.7

- Improvement: Checking for installed mappers are more flexible.
- Fixed: CRUD not explicitly calls SaveChanges for EF when modeling Auto-generated primary keys.

### Version 1.1.6

- Improvement: Improved error message when `ProjectTo` is selected as the query strategy with an unsupported provider (Mapperly).
- Fixed: Unnecessary dependency on `Intent.Common.UnitOfWork` module causing errors when the module isn't installed.
- Fixed: Service "update" interactions didn't work on "one to zero-or-one" owned entities.

### Version 1.1.5

- Improvement: Support for Mapperly module

### Version 1.1.4

- Improvement: Refactored so that can be used as NuGet package in other modules.
- Improvement: Detects existence and name of `CancellationToken` parameter before adding to Domain Interactions.
- Fixed: Exception thrown on invoking operations that accept only one parameter when that parameter is a Domain Service.
- Fixed: Incorrect variable name being used in some scenarios when generating the mapping code.

### Version 1.1.3

- Improvement: Support for Static Constructor mapping.
- Fixed: Logic to use AutoMapper would be generated even when the required AutoMapper module was not installed.

### Version 1.1.2

- Fixed: Add handling for Parameter specialization type in CallServiceInteractionStrategy.

### Version 1.1.1

- Fixed: Value Object update mappings are incorrectly being applied to nested Value Object collections.
- Fixed: Accessing owning entities should now be possible if the owning entity's PK naming convention includes the entity name.

### Version 1.1.0

- Improvement: Created `EntityPatchMappingTypeResolver` to support PATCH type update operations with null checks before assignment.

### Version 1.0.2

- Improvement: Added support for cursor based pagination

### Version 1.0.1

- Improvement: `CommandQueryMappingResolver` no longer assumes that Commands and Queries always have parameterized constructors and can now also detect if object initialization is needed, for example for "DTO" versions of requests for Service Proxy invocations.
- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).
- Improvement: Added dependency to `Intent.Modelers.Services.DomainInteractions 2.3.0` which enables Domain suggestions for creating CRUD services directly from Class elements.

### Version 1.0.0

- Supports domain interactions via the new interaction strategy mechanism.