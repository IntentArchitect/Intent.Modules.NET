### Version 5.0.0

- Improvement: Module now generates Specifications for Aggregate Roots.
- Improvement: Better interactions with CRUD modules to use Specifications out the box
- Improvement: Clean up of Repository contracts

### Version 4.1.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.1.1

- Improvement: Updated NuGet packages to latest stables.

### Version 4.1.0

- Improvement: Updated module project to .NET 8.
- Fixed: Compilation error due to missing `ToPagedListAsync` method in `RepositoryBase`.

### Version 4.0.4

- Improvement: Fixed the optional filters for queries that access the data with the DbContext; Refactored the way the query filters are constructed.

### Version 4.0.3

- Improvement: Resolve compiler warnings.

### Version 4.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Improvement: Updated supported client version to [3.2.0, 5.0.0).

### Version 4.0.0

- New Feature: Introducing Ardalis Specification as an alternative Repository pattern that updates the existing Entity Framework Repository pattern and introduces a [Specification pattern](http://specification.ardalis.com/usage/create-specifications.html).
