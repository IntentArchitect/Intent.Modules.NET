### Version 1.3.7

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.3.6

- Improvement: Bumped `Microsoft.NET.Tests.Sdk` to 17.6.0

### Version 1.3.5

- Fixed: Correct handling of nullable values in generated tests.

### Version 1.3.4

- Fixed: Having Entity Interfaces enabled would cause code compilation errors.

### Version 1.3.3

- Improvement: Updated test for Query By Id cases where a nullable object is returned, it shouldn't check for a `NotFoundException` to be thrown but rather to assert that the result is `default`.
- Fixed: In one case where a GetAll Query is unmapped, the unit tests threw an Exception around an Assertion class that wasn't found.

### Version 1.3.2

- Fixed: All tests will now use the exact same logic to detect whether a given Command/Query has a CRUD Handler for it to have a Unit Test.

### Version 1.3.1

- Improvement: Fluent Validation tests updated to accomodate the `IServiceProvider` injection scenarios.
- Improvement: Fluent Validation that receives Repositories, will also be stubbed.
- Fixed: Entities with Interfaces are now supported.
- Fixed: GetNestedCompositionalOwner detects owner classes more accurately to not throw exceptions unnecessarily.

### Version 1.3.0

- Improvement: Updated based on improvements made in MediatR.CRUD module.
- Fixed: Software Factory `MethodNotFound` exception would occur when run with `Intent.Application.MediatR.CRUD` version `5.2.4` and newer.
 
### Version 1.2.4

- Fixed: Limiting field mapping types for Assertion Class content generation.
- Fixed: GetByIdQuery handlers will only be targeted if the mapped type matches the return DTO mapped type.

### Version 1.2.3

- Fixed: When Private Setters on the Domain designer is switched on, the CRUD Test implementations won't generate to match the lookup criteria for the Auto-CRUD Implementation.

### Version 1.2.2

- Fixed: Generated tests targeted Command / Query handlers that aren't auto-implemented by CRUD auto-implementation system.

### Version 1.2.1

- Fixed: Value Objects are now supported that are embedded or associated with Classes.

### Version 1.2.0

- Improvement: Refactored module completely for better internal maintainability.
- Improvement: Future-proofing Command / Query Handler constructor parameters that receive new parameters (from Intent Architect) will now automatically get stubbed/mocked in unit tests.
- Improvement: Now catering for composite keys in Command / Query handlers.
- Improvement: Now generating tests for Commands projecting to Constructors and Operations.
- Improvement: Assertion classes will now only generate if being used by Unit Tests.
- Improvement: Now generating tests for Queries that are setup for pagination.
- Fixed: Get query handler tests would cause a crash in some cases when trying to generate.
- Fixed: Inaccurately named unit tests.
- Fixed: Assertions classes had a wrong comparison input on `HaveSameCount()` calls.
- Fixed: Mistake in nested delete handler tests where testing missing nested entity didn't return aggregate root item.

### Version 1.1.1

- Fixed: Ensure certain fields to be within a certain text range for negative testing to work successfully.

### Version 1.1.0

- Improvement: Introduces NotFoundException when an entity is not found in the tests.

### Version 1.0.3

- Improvement: Fixed Unit Tests for negative test cases where Enums are in collections.

### Version 1.0.1

- Improvement: Fixed and improved test scenarios for Enums where Enums that doesn't have a default literal value of `0` will be tested and then also literal values that doesn't exist in an Enum. It will also handle Enums that doesn't have literal values at all. 

### Version 1.0.0

- New Feature: Module to generate Unit Tests for Command and Query handlers as well as Fluent Validation Validators.
