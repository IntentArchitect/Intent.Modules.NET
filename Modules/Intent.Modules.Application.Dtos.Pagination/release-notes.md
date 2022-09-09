### Version 3.3.0

- New: Adds types to decorate service operations to be used for paginating results.
- Note: For best results, use in conjunction with Auto CRUD services which will automatically wire up your query results to be paginated. You will need to add a `pageSize` and `pageNumber` integer parameter to your service operation for it to take effect.