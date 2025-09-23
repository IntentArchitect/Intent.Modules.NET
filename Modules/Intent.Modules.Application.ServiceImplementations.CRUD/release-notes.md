### Version 5.2.1

- Improvement: Added support for Mapperly.

### Version 5.2.0

- Improvement: Added support for PATCH implementations of updates. This is currently only available for HTTP exposed Operations with the HTTP `PATCH` Verb.

### Version 5.1.2

- Improvement: Updated referenced package version

### Version 5.1.1

- Improvement: Updated dependency on `Intent.Application.DomainInteractions` to `1.0.1`

### Version 5.1.0

- Improvement: Upgraded to support implentations of interactions using the new interaction strategy mechanism.

### Version 5.0.28

- Improvement: Replaced the `System.Linq.Dynamic.Core` using clause with `static System.Linq.Dynamic.Core.DynamicQueryableExtensions` to better disambiguate `PagedResult`.
  > [!NOTE]
  >
  > Intent Architect should automatically remove the using clause `System.Linq.Dynamic.Core`, provided you have previously run the **Software Factory**. If it is not automatically removed simply remove it.
- Improvement: Improved code generated when using a literal as a query filter
- Fixed: CRUD implementations where failing in some circumstances with reserved words.
 
### Version 5.0.27

- Improvement: When a create operation is mapped to Cosmos DB document with a partition key which is separate from its primary key, the auto generated primary key can now be returned.
- Fixed: Mapping code now correctly generated when modeling and mapping a ValueObject collection as an Entity Attribute.

### Version 5.0.26

- Improvement: Support for different `MapTo` Extension methods.

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

- Improvement: Added support for ProjectTo query implementations for DBContext implementation CRUD Scenarios.
- Improvement: Comparison expressions with `bool` will generate a simplifed expression (e.g. `x.IsActive` instead `x.IsActive == true`)

### Version 5.0.18

- Improvement: CRUD for Compositional entities now throws a clearer exceptions if you have modeled multiple owners(which is not valid).
- Improvement: Added support for ProjectTo query implementations.
 
### Version 5.0.17

- Improvement: `Primary Key`s with Data source set to `User supplied` or `Auto-generated` no longer explicItly call `UnitOfWork` save changes. `Default` still does and should be used if you have Database generated primary keys.
 
### Version 5.0.16

- Improvement: Basic CRUD support for specifications.

### Version 5.0.15

- Improvement: Improved error when creating illegal queries.

### Version 5.0.14

- Fixed: Issue where `CRUD Update Action` was not generating correct code if Domain Services were injected.
- Fixed: Query with nullable mapped property, which is paginated generates uncompilable code.

### Version 5.0.13

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.12

- Improvement: Support for nullable `OrderBy` on Paging.

### Version 5.0.11

- Improvement: Add support for `OrderBy` on pagination.

### Version 5.0.10

- Improvement: FindAsync handling more mapping scenarios.
- Improvement: Handler implementation that involves operation invocations returning non-void types will now return appropriate variable declaration types such as `var myVar = ...;` or `var (prop1, prop2, prop3) = ...;`.

### Version 5.0.9

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 5.0.8

- Improvement: Added basic support non UOW based repositories.

### Version 5.0.7

- Improvement: Add support for Data Contract mappings with the Advanced mapping system.
- Fixed: Generating code for advanced mappings where there is a nested target DTO will now properly instantiate that DTO before mapping values.
- Fixed: Service Call Operations can also now invoke `Operations` on a `Class`.
- Fixed: Service dependency substitutions can also occur on parameters of a Service Call Operation.

### Version 5.0.6

- Improvement: Adding mapped literals for `Query`s caused software factory errors this is now supported.
- Improvement: Service calls returning DTO's with primary keys on newly created entities now force a UOW save to ensure key is populated.

### Version 5.0.5

- Fixed: Issue around CRUD with nullable return types.

### Version 5.0.4

- Improvement: Automatic wiring up of `DomainService`s in CRUD Implementations.
- Improvement: `DomainInteractionsManager` - Consolidated CallServiceOperation enhancements include refined service dependency checks, extended support for ClassModel, DataContractModel, and TypeDefinitionModel through VariableType, and robust handling of asynchronous operations with proper awaiting and cancellation token integration, improving clarity and flexibility in service operation calls.

### Version 5.0.3

- Improvement: Added support for `Value Object` collections.
- Improvement: Now can inject code in Service Implementations that are not standard but custom. Just ensure that Templates are of `File Builder` type, `Class` has a model that contains the `ServiceModel` as "model" metadata of the targeted `Service` and the `methods` that require implementation have `OperationModel` as "model" metadata.

### Version 5.0.2

- Improvement: CreateOrUpdate methods now support domain interfaces.
- Fixed: Optional query filters not working with DbContext oriented data-access.

### Version 5.0.1

- Imrpovement: Support for optional query filters when querying lists (or paginated lists) of entities.

### Version 5.0.0

- Fixed: When DTOs had the same names as its domain entity, generation of mapping extension method calls would result in uncompilable code.

### Version 4.3.4

- Normalized variable names used in generated methods for alignment with other modules.

### Version 4.3.2

- Fixed: Type disambiguation rules were incorrectly being applied to variable names for some CRUD operation implementations.

### Version 4.3.0

- Update: CRUD support for mapping service contracts to `Value Objects`.

### Version 4.3.0

- Update: Introduces NotFoundException when an entity is not found.

### Version 4.2.4

- Supports `pageIndex` and adapts to `pageNo` for pagination implementation.
- Supports update implementation when no `id` parameter is provided, but when an ID is available on the provided DTO.	
- Fix : Add call to `UnitOfWork` `SaveChanges` for Update CRUD operations with results, to ensure any newly created `Entity`s have their Id's populated. 

### Version 4.2.3

- Support for DTO return types on Create, Update and Delete operations.

### Version 4.2.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### 4.1.1

- Fixed: Implementations for update operations would not properly handle associations where the existing or target value was null.
