### Version 5.2.2

- Improvement: Updated referenced package version

### Version 5.2.1

- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).
- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 5.2.0

- Improvement: Upgraded to support implentations of interactions using the new interaction strategy mechanism.

### Version 5.0.28

- Improvement: Replaced the `System.Linq.Dynamic.Core` using clause with `static System.Linq.Dynamic.Core.DynamicQueryableExtensions` to better disambiguate `PagedResult`.
  > [!NOTE]
  >
  > Intent Architect should automatically remove the using clause `System.Linq.Dynamic.Core`, provided you have previously run the **Software Factory**. If it is not automatically removed simply remove it.
- Improvement: Improved code generated when using a literal as a query filter
- Fixed: CRUD implementations where failing in some circumstances with reserved words.

### Version 5.0.27

- Improvement: Support for different `MapTo` Extension methods.
- Fixed: Mapping code now correctly generated when modeling and mapping a ValueObject collection as an Entity Attribute.

### Version 5.0.26

- Fixed: Not found exception message incorrect for composite entities.

### Version 5.0.25

- Fixed: Invoking Service Operations from other Service Operations with the same DTO type caused an exception.

### Version 5.0.24

- Improvement: Added validations for Paging with missing parameters.
- Improvement: Updated `System.Linq.Dynamic.Core` to version without vulnerability.

### Version 5.0.23

- Improvement: Added support for `Domain` and `Services` naming conventions for `Entities`, `Attributes` and `Operations`.
- Fixed: `Call Service Operation` mapping didn't allow mapping a 1 -> * association on an Entity that is supplied to the operation.

### Version 5.0.22

- Fixed: Added support for `ToList` mapping.

### Version 5.0.21

- Improvement: Provided a better error message to guide users when they attempt to call an operation using Call Service Operation on an Entity without Creating / Querying it first.

### Version 5.0.20

- Fixed: For certain persistence technologies generated filter methods incorrectly used the concrete type of the entity as opposed to the document interface type resulting in un-compilable code.

  > [!NOTE]
  >
  > The corresponding module updates for the persistence technology will also need to be installed in order for the fix to be fully applied:
  >
  > - **Intent.CosmosDB**: 1.2.8

### Version 5.0.19

- Improvement: Improved code quality by making `DomainEventService.GetNotificationCorrespondingToDomainEvent` static.
- Improvement: Added support for ProjectTo query implementations for DBContext implementation CRUD Scenarios.
- Improvement: Comparison expressions with `bool` will generate a simplifed expression (e.g. `x.IsActive` instead `x.IsActive == true`)

### Version 5.0.18

- Improvement: Converted T4 template to CSharpFileBuilder paradigm.
- Improvement: CRUD for Compositional entities now throws a clearer exceptions if you have modeled multiple owners(which is not valid).

### Version 5.0.17

- Improvement: `Primary Key`s with Data source set to `User supplied` or `Auto-generated` no longer explicItly call `UnitOfWork` save changes. `Default` still does and should be used if you have Database generated primary keys.
- Improvement: Updated `DomainEventServiceTemplate` template to `CSharpFileBuilderTemplate`.

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
