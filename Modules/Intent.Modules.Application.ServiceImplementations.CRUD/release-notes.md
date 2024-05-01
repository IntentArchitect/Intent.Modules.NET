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
