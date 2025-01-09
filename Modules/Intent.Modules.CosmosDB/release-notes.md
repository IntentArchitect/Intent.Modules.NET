### Version 1.2.8

- Improvement: It is now possible to specify the throughput type and amount (when applicable) for a container. Refer to the [documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-cosmosdb/intent-cosmosdb.html#throughput) for more details.
- Fixed: CRUD modules would generate filter methods using the concrete type of the entity as opposed to the document interface type resulting in un-compilable code.

  > [!NOTE]
  >
  > You will need to ensure that corresponding CRUD modules have also been updated to at least the following minimum versions:
  >
  > - **Intent.Application.MediatR.CRUD**: 6.0.23
  > - **Intent.Application.ServiceImplementations.Conventions.CRUD**: 5.0.20
  > - **Intent.MediatR.DomainEvents**: 5.0.20

### Version 1.2.7

- Improvement: Updated NuGet package versions.

### Version 1.2.6

- Improvement: Small updated to module internal code

### Version 1.2.5

- Fixed: NuGut packages added for lower version of .Net Framework.

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
