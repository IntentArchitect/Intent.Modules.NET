### Version 4.7.1

- Improvement: Comments are now generated for methods on repository interfaces.

### Version 4.7.0

- Improvement: Updated to respect new Stored Procedure stereotypes introduced in version `1.1.1` of the `Intent.Modelers.Domain.StoredProcedures` module.
- Fixed: When domain types (such as a Classes, DataContract and Enums) were used on repository operations, usings would not always be added.
- Fixed: Repository Operations now also recognize the `Asynchronous` stereotype.

### Version 4.6.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.6.2

- Improvement: Using the new `CSharpTypeTuple` to represent an Operation returning out parameters which can then be used to declare `var (param1, param2)` expressions.

### Version 4.6.1

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 4.6.0

- New Feature: Generate methods on Repositories for Bespoke and Entity based Repositories.
- Improvement: Made the `_dbContext` member protected so it can be accessed in inherited repositories.

### Version 4.5.7

- Fixed: Output parameters now can specify their respective SQL types and character size to prevent exceptions being thrown due to a missing size value.

### Version 4.5.6

- Fixed: `CustomRepositoryTemplate` did not take `DbContextInstance` into account when there are multiple DbContexts.

### Version 4.5.5

- Improvement: Class that is in a Domain package for a different DB will now apply the relevant DbContext to the Repository.

### Version 4.5.4

- Fixed: Stored Procedures will generate execution parameter names with `@` prefix symbols.
- Fixed: OUTPUT parameters have not always been correctly set.

### Version 4.5.3

- Improvement: Added roles to interface template for easier location in other modules.
- Improvement: Keyless entities will now have a manual insert SQL statement generated for them.
- Improvement: Updated Module project to .NET 8.

### Version 4.5.2

> ⚠️ **KNOWN ISSUE**
> 
>  The `Stored Procedure` element was moved to `Intent.Modules.Modelers.Domain.StoredProcedures` module and there is a bug with Intent Architect 4.2.2 and earlier that when you update to this version it will cause and error regarding a duplicate element.

- Improvement: `CustomRepositoryTemplate` and `CustomRepositoryInterfaceTemplate` Initialization Logic - Adjusted the execution timing of StoredProcedureHelpers.ApplyImplementationMethods outside the AddClass method scope. This change ensures more efficient startup behavior and better supports runtime metadata lookups (i.e. for async method configurations).
- Improvement: `StoredProcedureHelpers` - Refined the implementation method to better support stored procedures with complex return types. This includes a more sophisticated handling of async operations, output parameters, and return statements, ensuring that stored procedures are executed more efficiently and correctly.

### Version 4.5.1

- Improvement: Added support for stored procedures which return scalar values.

### Version 4.5.0

- Improvement: Moved `PagedList<T>` type into the `Intent.EntityFrameworkCore 4.6.0` module since pagination is an separate concert to the repository pattern.

### Version 4.3.0

- Improvement: Added several repository method overloads to bettere support more advanced queryOptions though LINQ.

### Version 4.2.10

- New Feature: Added a `Database Setting` namely `Add synchronous methods to repositories`, when this is on, the EF Repositories will contain synchronous versions of their operations.

### Version 4.2.7

- Fixed: In some cases, custom repositories weren't adding a using directive when `IReadOnlyCollection<T>` was used.

### Version 4.2.6

- Update: Support for Composite PrimaryKeys on repositories.
- Fixed: `User-Defined Table Type Settings` stereotype will no longer be automatically applied to `Data Contract` elements (it can still be applied manually).

### Version 4.2.5

- Stored procedures now support `OUTPUT` and `Table-Valued` parameters.

### Version 4.2.4

- FindByIdAsync indicates that return type could be null.

### Version 4.2.3

- Update: Removed various compiler warnings.
- Added new `IEfRepository` to contain methods with `Expression`s and `IQueryable`s which were removed from interfaces in `Intent.Entities.Repositories.Api` 4.0.5.
- Added `Update` method to repositories. With default EF Core configuration this is not needed, but is now available.

### Version 4.2.2

- Added support for executing Stored Procedures. To use a Stored Procedure:
	- Create a `Repository` in the Domain Designer (either in the package root or a folder).
	- You can optionally set the "type" of the repository to a `Class` which will extend the existing repository which is already generated for it, otherwise if no "type" is specified a new Repository is generated.
	- On a repository you can create `Stored Procedure`s.
	- At this time, the module supports a Stored Procedure returning: nothing, an existing `Class` or a `Data Contract` (`Domain Object`).
	- The Software Factory will generate methods on the Repositories for calling the Stored Procedures.
- Update: Removes some warnings from generated code.


### Version 4.2.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- Update: Repositories will only be generated for Classes created inside a "Domain Package".

### Version 4.0.2

- Update: Converted RepositoryBase into Builder Pattern version.

### Version 4.0.1

- Update: Internal template changes.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.14

- Update: Added `IQueryable` option for `FindAsync` operation.
