### Version 1.2.10-pre.0

- New Feature: Added `ObjectMappingMappingStrategy`, a fourth `IMappingStrategy` alongside the AutoMapper and Mapperly ones. It matches when `Intent.Application.Dtos.ObjectMapping` is installed and generates query handler Call Sites that call that module's generated extension methods directly — `order.MapToOrderDto()` for a single entity, `orders.MapToOrderDtoList()` for a collection, `order?.MapToOrderDto()` when the Query Entity Action end is nullable, and `orders.MapToPagedResult(x => x.MapToOrderDto())` for an offset-paged query. No `IMapper` is injected into any handler.
- New Feature: The strategy resolves the Mapping Extension Class by template role, which both registers the `using` for it and guards against emitting a Call Site to a class that was never generated — a DTO with no domain mapping produces no call rather than a reference to a missing type.
- Note: The Object Mapping strategy reports `HasProjectTo() == false`. Its mapping methods are ordinary C# over materialised objects, not expression trees, so an application whose `Default Query Implementation` is set to `ProjectTo` gets the existing `ElementException` naming the offending element, rather than a query that cannot be translated at runtime. Cursor-paged Call Sites are not verified for this strategy.

### Version 1.2.9

- Fixed: Software Factory crash (`NullReferenceException`) when generating Update or Delete entity interactions if `Intent.Common.UnitOfWork` is not installed.

### Version 1.2.8

- Improvement: Better error handling in SF around Update Entity Actions with Update Mappings but missing Query Mapping.

### Version 1.2.7
- Fixed: When a user-supplied primary key is used, use its name in the `return` statement instead of always assuming it is "id" 

### Version 1.2.6
- Improvement: Updated code generation for query handlers when Mapperly is used to avoid premature materializing collections before mapping. 
- Fixed: When two or more DTOs in the same folder are of the form `<Entity>Dto.cs` and `<Entity><SomeString>Dto.cs` the correct DTO is now injected into the constructor of the corresponding handlers

### Version 1.2.5

- Fixed: Mapperly paged queries not always generating the correct mappings.

### Version 1.2.4

- Improvement: Better support to identify areas where AI should be implementing logic.
- Improvement: Added support for AI Context in various implementation strategies to enable AI-assisted generation of service call logic.

### Version 1.2.3

- Fixed: Added guards to give clearer error messages when Primary Keys could not be found on an Entity and when an Entity template could not be found.
- Improvement: Module dependencies updated.

### Version 1.2.2

- Improvement: Introduced components (and relocated basic entity patch logic) for upcoming JSON Patch module.
- Improvement: Improved logic for determing the request property to use for key lookups.

### Version 1.2.1
- Fixed: Cancellation token always being added to service call, even when service did not have a cancellation token parameter.

### Version 1.2.0

- Improvement: Module dependencies updated.
- Improvement: Interaction Strategies enhanced to have better detection regarding actions with mappings to prevent ugly errors being raised during Software Factory runs.

### Version 1.1.12

- Improvement: Mapperly mapping classes are now injected as opposed to being instantiated directly.

### Version 1.1.11

- Improvement: Exception handling with FriendlyExceptions being thrown.
- Improvement: When Accessing Composite Entities directly without accessing the parent will result in a friendlier error message.

### Version 1.1.10

- Improvement: Create and Update actions involving Aggregate Root IDs will now leverage appropriate query logic to fetch those aggregate roots and assign them to the entity in question.

### Version 1.1.9

- Improvement: Documentation and ProjectUrl link added.

### Version 1.1.8

- Internal Improvement: Preparing for future support for result pattern in mapping system.

### Version 1.1.7

- Improvement: Checking for installed mappers are more flexible.
- Fixed: CRUD not explicitly calls SaveChanges for EF when modeling Auto-generated primary keys.
- Improvement: Added support for singular DBSet names

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