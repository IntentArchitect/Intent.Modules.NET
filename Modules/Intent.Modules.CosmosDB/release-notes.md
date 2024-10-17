### Version 1.2.5

- Fixed: NuGut packages added for lower version of .Net Framwoek..

### Version 1.2.4

- Improvement: Added setting to allow enum values to be serialized to a Document as string.
- Fixed: Value Objects can also now be set in Attribute return types.

### Version 1.2.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.2.2

- Improvement: Updated NuGet packages to latest stables.

### Version 1.2.1

- Improvement: Added support for explicit `ETag` modeling for Client/Server concurrency scenarios.

### Version 1.2.0

- New Feature: Added support for separate database multi tenancy.
- Improvement: Added more Linq support to the repository, also added some `protected` methods for better being able to extend the repository using `SQL`.

### Version 1.1.1

- Fixed: If key types other than `string` were being used for an entity and `Use Optimistic Concurrency` was enabled then uncompilable code would be generated.

### Version 1.1.0

- New Feature: Added support for Optimistic Concurrency using `ETag` for `CosmosDB` module.
- Improvement: Added `FindAsync` method to repository.

### Version 1.0.3

- Fixed: Fixed an issue around nullable collections not being realized correctly.

### Version 1.0.2

- Fixed: Fixed an issue where modeling `Value Object`s was not working.

### Version 1.0.0

- New Feature: CosmosDB module.
