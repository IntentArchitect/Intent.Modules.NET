### Version 5.1.9

- Fixed: Resolved issue where the cursor based paged result interface was not always being output when it should

### Version 5.1.8

- Fixed: When a repository method name was suffixed with `Async` a Software Factory error would occur due to generation of multiple `cancellationToken` parameters.
- Improvement: Added support for cursor based pagination.

### Version 5.1.7

- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 5.1.6

- Improvement: Added `FindById` overload with `queryOptions` on repositories.
- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 5.1.5

- Improvement: Moved custom repository creation to this module leveraged by any database provider.

### Version 5.1.4

- Fixed: Removed extraneous 2nd type parameter from `Intent.Entities.Repositories.Api.EntityRepositoryInterface` which would be generated for entities which were in a Domain Package without any particular persistence technology.

### Version 5.1.3

- Fixed: template config issue for `EntityRepositoryInterface`.
- Improvement: Fixed small reference error in XML documentation

### Version 5.1.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.1.1

- Improvement: Model representation added for advanced mapping purposes.

### Version 5.1.0

- Improvement: Renamed the `IPagedResult<T>` interface to `IPagedList<T>` to remain consistent with the concrete type `PagedList<T>` and to prevent confusion with the `PagedResult<T>` type which is for outbound DTOs.

### Version 4.1.3

- Improvement: Upgrades IUnitOfWork Interface template to CSharpFileBuilder paradigm.

### Version 4.1.3

- Improvement: Added support for entities with generic type parameters.

### Version 4.1.2

- Improvement: Added Document DB Provider support, allowing this module to be used in conjunction with other Document DB technologies within the same application.

### Version 4.1.0

- Improvement: Added NotFoundException for queries when they can't find Entities by a search criteria.

### Version 4.0.6

- Improvement: FindByIdAsync indicates that return type could be null.

### Version 4.0.5

- Improvement: Removed various compiler warnings.
- Improvement: Removed methods containing expressions and/or IQueryable to reduce coupling and allow to be used for persistence technoglogies which don't support these.

### Version 4.0.4

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.2

- Improvement: Decoupled this module from the Intent.Metadata.RDBMS module.

### Version 4.0.1

- Improvement: Repository Interface also updated to use new Builder Pattern paradigm.

### Version 4.0.0

- Improvement: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.8

- Improvement: Added `IQueryable` option for `FindAsync` operation.
