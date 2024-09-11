### Version 4.0.13

- TODO

### Version 4.0.12

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.0.11

- Improvement: Support for nullable `OrderBy` on Paging.

### Version 4.0.10

- Improvement: Support for `OrderBy` on pagination.

### Version 4.0.9

- Improvement: Support for the renaming of the `IPagedResult` to `IPagedList` from the `Intent.Entities.Repositories.Api` module

### Version 4.0.7

- Improvement: `PagedResult<TData>` Type-Definition moved to version `3.4.9` of the `Intent.Modelers.Services` module.

### Version 4.0.6

- Improvement: Added `C#` stereotype to `PagedResult` and set its `Is Collection` property to `true`.

### Version 4.0.5

- Stop generating code for `PagedResult` which causes non-nullable data compilation warnings.
- Added : `Add Pagination` designer extension for CQRS `Query`s and service `Operation`s.

### Version 4.0.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.3

- Updated supported client version to [3.4.0-pre.0, 5.0.0).

### Version 4.0.1

- Fixed: Added check to prevent Null Reference Exception.

### Version 3.3.0

- New: Adds types to decorate service operations to be used for paginating results.
- Note: For best results, use in conjunction with Auto CRUD services which will automatically wire up your query results to be paginated. You will need to add a `pageSize` and `pageNumber` integer parameter to your service operation for it to take effect.
