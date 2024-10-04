### Version 6.0.19

- Improvement: Basic CRUD support for specifications.

### Version 6.0.18

- Improvement: Improved error when creating illegal queries.

### Version 6.0.17

- Fixed: Issue where `CRUD Update Action` was not generating correct code if Domain Services were injected.
- Fixed: Query with nullable mapped property, which is paginated generates uncompilable code.
- Fixed: Advanced mapped DTO fields will now conform to Pascal Case.

### Version 6.0.16

- Improvement: Updated module NuGet packages infrastructure.

### Version 6.0.15

- Improvement: Support for nullable `OrderBy` on Paging.

### Version 6.0.14

- Improvement: Support for `OrderBy` on Paging.
- Improvement: CQRS CRUD script now supports surrogate Primary keys marked as `User supplied`.

### Version 6.0.13

- Improvement: FindAsync handling more mapping scenarios.
- Improvement: Handler implementation that involves operation invocations returning non-void types will now return appropriate variable declaration types such as `var myVar = ...;` or `var (prop1, prop2, prop3) = ...;`.

### Version 6.0.12

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 6.0.11

- Improvement: Added basic support non UOW based repositories.

### Version 6.0.10

- Improvement: Add support for Data Contract mappings with the Advanced mapping system.
- Fixed: Generating code for advanced mappings where there is a nested target DTO will now properly instantiate that DTO before mapping values.
- Fixed: Service Call Operations can also now invoke `Operations` on a `Class`.
- Fixed: Service dependency substitutions can also occur on parameters of a Service Call Operation.
- Fixed: Command/Query handler implementations failed when the consolidated option was enabled.

### Version 6.0.9

- Fixed: Resolved an issue causing syntax errors when mapping services onto Value Objects using the basic mapping configuration.

### Version 6.0.8

- Improvement: Adding mapped literals for `Query`s caused software factory errors this is now supported.
- Improvement: Service calls returning DTO's with primary keys on newly created entities now force a UOW save to ensure key is populated.

### Version 6.0.7

- Fixed: Issue around CRUD with nullable return types.

### Version 6.0.6

- Improvement: Automatic wiring up of `DomainService`s in CRUD Implementations.
- Improvement: `CommandMappingImplementationStrategy` and `QueryMappingImplementationStrategy` was updated to also include the `ServiceOperationMappingTypeResolver` when performing generating implementation logic for mappings.
- Improvement: `DomainInteractionsManager` - Consolidated CallServiceOperation enhancements include refined service dependency checks, extended support for ClassModel, DataContractModel, and TypeDefinitionModel through VariableType, and robust handling of asynchronous operations with proper awaiting and cancellation token integration, improving clarity and flexibility in service operation calls.

### Version 6.0.5

- Improvement: Removed support for implicit keys, allowing keyless entities to now be modelled.

### Version 6.0.4

- Fixed: If a nested compositional entity does not have an owner Id, it will no longer run the GetAllImplementationStrategy.
- Fixed: GetAllImplementationStrategy will only execute when it can find a mapped Entity for compositional owner.

### Version 6.0.3

- Improvement: Added support for `ValueObject` collections.

### Version 6.0.2

- Fixed: Optional query filters not working with DbContext oriented data-access.
- Improvement: CreateOrUpdate methods now support domain interfaces.

### Version 6.0.1

- Improvement: Support for optional query filters when querying lists (or paginated lists) of entities.

### Version 6.0.0

- Improvement: Advanced Mapping system CRUD Support.
- Improvement: When multiple matching CRUD strategies are found the module will now apply the first strategy based on the registration order.

### Version 5.4.0

- Improvement: Better CRUD support for composite keys.

### Version 5.3.3

- Fixed: When a Query By Id is set to return a nullable object and has a repository that can't find the entity, it will return `default` as opposed to throw a `NotFoundException`.

### Version 5.3.2

- Improvement: Added CRUD Strategy for ODataQuery. 

### Version 5.3.1

- Improvement: Projecting for Commands now included aggregational foreign keys.
- Fixed: Create CRUD scripts would create fields for _DomainEvent_s on constructors.

### Version 5.3.0

- Fixed: Addressed an issue where the CRUD Conventions script would fail when generating for Composites.

### Version 5.2.7

- Fixed: When calling domain operations for Document DB aggregate entities, an uncompilable `_<entityName>Repository.Update(aggregateRoot)` statement would be generated.

### Version 5.2.6

- Improvement: Upgraded CRUD Scripts to be forward compatible with Intent Architect v4.1.

### Version 5.2.5

- Fixed: GetNestedCompositionalOwner detects owner classes more accurately to not throw exceptions unnecessarily.

### Version 5.2.4

- Improvement: Will now automatically add necessary fields for primary keys for command and queries mapped to owned composite entities and implementations will properly recognize them.
- Fields and corresponding DTOs are now automatically created for `1 .. 1` associations.  

### Version 5.2.3

- Improvement: Updated extension method to deal with composite keys for use in the Unit Test module.

### Version 5.2.2

- Improvement: Commands will always be made for `Class` `Operation`s regardless of whether or not private setters is enabled.
- Fixed: Possible `Async` suffix of `Class` `Operation`s is not included in the name of generated commands.
- Fixed: `Operation` `Parameter`s of type `Domain Service` are no longer automatically mapped.

### Version 5.2.1

- Fixed: Implementations were not being generated for commands mapped to constructors.
- Fixed: Mappings to `Is Collection` parameters of type `Data Contract` or `Value Object` would generate uncompilable code.

### Version 5.2.0

- Improvement: Asynchronous domain operation will now be `await`ed and have `CancellationToken`s passed in as a parameter.
- Improvement: Variable names for new / existing entities in generated implementations for constructor and operator invocations now normalized so as to work properly with integration event publishing.

### Version 5.1.4

- Improvement: `Create CQRS CRUD Operations` can now be done on folders, not just the root node of the designer.
- Improvement: Better CRUD support for Entities with Composite Primary Keys.
- Improvement: Pagination implementation support `PageIndex` which is similar to `PageNo` but 0 based.
- Improvement: Commands / Queries will ignore mapping certain Class Attributes that are internally marked as `set-by-infrastructure` so as to not exposing them to the public unintentionally.

### Version 5.1.3

- Fixed: Nested composite entity Command/Query handlers had inconsistent naming conventions that caused other code (like injecting publishers) to break at compile time.
- Fixed: Compositional deletion implementations weren't explicitly calling repository `Update` methods when required.
- Improvement: CRUD support for mapping service contracts to `Value Objects` and `Data Contracts`.

### Version 5.1.2

- Fixed: Added NotFoundException for nested entity creation scenarios if the Aggregate entity Id could not be found.

### Version 5.1.0

- Improvement: Introduces NotFoundException when an entity is not found.

### Version 5.0.5

- Improvement: Support for DTO return types on Constructor mapped commands.
- Fixed : Add call to `UnitOfWork` `SaveChanges` for Update CRUD operations with results, to ensure any newly created `Entity`s have their Id's populated. 

### Version 5.0.4

- Improvement: Removed various compiler warnings.
- Improvement: Support explicitly calling repository `Update` methods when required.
- Fixed: Update implementations for properties of nullable complex types would throw an exception when attempting to assign a null value.

### Version 5.0.3

- Improvement: Support for DTO return types on Create, Update and Delete requests.

### Version 5.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 5.0.1

- Improvement: Decoupled this module from the Intent.Metadata.RDBMS module.
- Fixed: Duplicate Create and Update methods being generated for properties where shared DTO and Entity types are involved.

### Version 5.0.0

- Improvement: Create CQRS CRUD Operations script now will map to constructors for create if the entity has a non-empty one, and will create a command per operation if `Ensure Private Setters` setting is enabled.
- Fixed: Would try to use domain entity setters event when `Ensure Private Setters` setting was enabled.

### Version 4.1.3

- Fixed: `Create CRUD Service` no longer adds the DeleteCommand as part of operation signature.

### Version 4.1.2

- Fixed: Implementations for update operations would not properly handle associations where the existing or target value was null.
- Fixed: Create CRUD script will refer (in nested compositional cases) to the nested compositional owner's Id Type when implicit key types are defined.
- Fixed: Projecting to the Domain Entity will ensure that collections of primitive types will also work.

### Version 4.1.1

- Fixed: Added `null` checks on places previously not covered.

### Version 4.1.0

- New Feature: Now Commands, Queries and DTOs will include fields of mapped Entities' inheritance hierarchies.
- Improvement: Cleaned up and refactored the `Create CRUD Service` script.
- Improvement: Service and resource names use the singular naming convention now.
- Fixed: `Create CRUD Service` script now does better job at fetching Primary Key fields from Domain Entities.
- Fixed: Ids on Commands / Queries for explicit keys should now be mapped.
- Fixed: Generated CRUD handling code now properly detects Primary Key Ids.
- Fixed: Requests with incoming `null` values on Collection Properties will no longer throw NullReferenceExceptions.
- New: Using the new Association property on Foreign Key stereotype to identify the correct Attribute on an Entity.
[release-notes.md](release-notes.md)

### Version 4.0.3

- Improvement: Dependency version updates.

### Version 4.0.2

- Fixed: Create CRUD Service for nested compositional entities will only pick compositional relationships.

### Version 4.0.1

- New Feature: Adds CRUD capabilities to Eventing modules.
- New Feature: Added support for creating and managing CRUD operations for composite Domain Entities that are nested one level deep from an Aggregate Root Domain Entity.
  ![](data:image/png;base64,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)
  ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFAAAAAcCAYAAAD2izi6AAAAAXNSR0IArs4c6QAAA6VJREFUaEPt2FlPE1EUB/D/FNopM3TTKkuxJUaDBFrAIiKJWKvRSBSNJpqob+pHMG5Ro8btcxAShURjUlA0MXEJKrQKISqgUpeCxQahhbbTbcbMuKWWzeKLnbkv07Q9tzm/OXdO7yXKzZUcnavB98H9uEqX2QUI4ePglB/E2nUbuLwCoySWhoB35AOImtp6Lr/QlEa4FCIArlm7niswFEsaaQh8Hn6fDLhomRFfP31Mmmp7Qz0c7Q+T3pPL5TBXWvC825XGz2ZOSApgRUMjIh/dKFtpwKs3HhTl6WHbXIdbLa0YH5/CpHo5KgppdNy5i8NHT6D/1RvUlizCqMeDjpv3YNu9B63Xr4mmFaUArtmyFWxgAvm5MrzmAU3FoHMIsOFxEFQBekamUGXU4bajDfYdu9Db+QD+ySD2NtjQ4riP6hornnY+y5wSmyOTFEDRZP6PEpUAFwgpAf4LwCprLbdkqWGBU4kz3PdlGMSKlas4UkmLU2CBWUeY4G9AgiAw6h0Gx0n74dlceae8fIPglASYlZWFkeHkP9EymQwsyy7wPmVeeKHBiEQiMTNgLk1DQSpAURT4cwe+JuPxGHy+Mei0WlA0BZ/Ph2g0JkyU6aOkZAUGB9/9Wp1zApaWliA4FUQ4HEZ2thykkkQ8FsNn7yj4bVxVpQVO1wvw5RyNRjPdLyW/OQFFJ/KXCU8LyM/BPwchLFppzCzA/XpsJTWRSIRBwD8hyc1DQK3RgiSVyU2EYcIoM1vnES595WWfC0plzp+AIZSZqwWdnQ11iIASXntGvNCRcaj0Jvi9b9E3yKBy1za4bjSDZTnkG8vhHnCKSvVlnxNKJTU9IEmroNOqsG9TjdC2bz3oQo3VCnLxEniG+tHXOwq1cTHwrh++BItEIoYwI65OPCugqEopzWRnBPRPjKc5pbjCNFrd9Ev4J+DVK5dx7PgJQaVuXS06nzzF2TOnce78BZAkiUgkgmrrajhdz3HwwH50O50YGBhMUbx08QJOnjoNisoRnpcMw6DCYkFgMgC3+/1/qz4n4JEjhzA29hX2jTZh58HvNvR6Pbq6uuFwtKHcXAaL2YympmYULSvCRpsNfBdnOQ6PHj1GY+MOaNRqKBQK9PT0oq29HXa7HaRCDo1Wi5aWVgwNuTMFsJQjlRQYJoS/WcI/Dxr4fXMwFIJKpRJAAoEAaJpGKBSa8WRHJiOEivxfR1IFmkzFHJ8Mf1gQi0snL/O5qfJsGbKy5eAL4RvIPtGZUGH3OwAAAABJRU5ErkJggg==)

### Version 4.0.0

- Improvement: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.12

- Improvement: Refactored the CRUD script so that it is easier to read and to follow.
- Fixed: Delete command was not mapped to the domain for explicit keys.

### Version 3.3.11

- New Feature: CRUD Creation scripts now automatically return the surrogate key type in the create operation if it is not a composite key. This will return the Entity's ID on creation. Explicit and Implicit keys are respected.
- New Feature: Those surrogate key types will be automatically wrapped in a `application/json` wrapper object to aid clients consuming that service operation with parsing valid JSON.
