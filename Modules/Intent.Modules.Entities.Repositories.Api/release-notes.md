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
