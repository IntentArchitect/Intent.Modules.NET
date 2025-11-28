# Intent.Application.Dtos.Pagination

## What is Pagination?
Pagination constrains large result sets into manageable slices, improving performance and user experience by loading data incrementally. This module generates DTO result wrappers and mapping helpers for both offset-based and cursor-based pagination patterns.

Intent Architect supports two pagination strategies:
- **Offset-based pagination** (`PagedResult<T>`) - Traditional page number and page size approach with total counts
- **Cursor-based pagination** (`CursorPagedResult<T>`) - Token-based approach for efficient pagination of dynamic datasets

## What This Module Generates

### PagedResult<T> (Offset-based)
A generic DTO wrapper with the following properties:
- `TotalCount` - Total number of items across all pages
- `PageCount` - Total number of pages available
- `PageSize` - Number of items per page
- `PageNumber` - Current page number (1-indexed)
- `Data` - Collection of items for the current page

### CursorPagedResult<T> (Cursor-based)
A generic DTO wrapper with the following properties:
- `Data` - Collection of items for the current page
- `CursorToken` - Opaque token to retrieve the next page
- `PageSize` - Number of items requested per page
- `HasMoreResults` - Boolean indicating if more pages are available (derived from CursorToken presence)

### Mapping Extensions
Helper extension methods to map repository/query results into paginated DTOs, reducing boilerplate in service handlers.

### Type Source Installer
Registers pagination types with the application layer for use in Services Designer modeling.

## Examples

### Example: Offset-based Pagination (PagedResult)
```csharp
// Query Handler with PagedResult
public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _repository;

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        // Repository returns IPagedList<Customer> from EF Core
        var pagedCustomers = await _repository.FindAllAsync(
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);
        
        // MapToPagedResult is an Intent-generated extension method
        return pagedCustomers.MapToPagedResult(customer => customer.MapToCustomerDto());
    }
}

// API Response
{
  "totalCount": 1523,
  "pageCount": 16,
  "pageSize": 100,
  "pageNumber": 1,
  "data": [
    { "id": 1, "name": "Acme Corp" },
    { "id": 2, "name": "Beta Industries" }
    // ... 98 more items
  ]
}
```

### Example: Cursor-based Pagination (CursorPagedResult)
```csharp
public async Task<CursorPagedResult<CustomerDto>> GetCustomers(string? after, int size)
{
    var query = _dbContext.Customers.OrderBy(c => c.Id);
    return await query.ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
        .ToCursorPagedResultAsync(after, size); // intent-generated extension
}
```

## When to Use PagedResult vs CursorPagedResult

Often this is limited to the persistence technology you have chosen. However here are some things to consider:

### Use PagedResult (Offset-based) when:
- Users need to jump to specific page numbers (e.g., "Go to page 5")
- You need to display total count and page count in the UI
- The dataset is relatively stable (infrequent inserts/deletes)
- Implementing traditional table pagination with page number selectors
- Dataset size is moderate (thousands, not millions)

### Use CursorPagedResult (Cursor-based) when:
- Dealing with real-time or frequently changing data (e.g., social feeds, logs)
- Implementing infinite scroll patterns in the UI
- Optimizing performance for very large datasets (millions of rows)
- Preventing duplicate or skipped items during pagination
- Users only need "next page" functionality (no arbitrary page jumping)
- Working with distributed systems where consistent counts are expensive

## Integration with Other Modules
- Works with `Intent.Application.Dtos` base DTO infrastructure.
- Commonly combined with repositories (Entity Framework, MongoDb, CosmosDb, Redis) through LINQ queries.
- Complements `Intent.Application.ServiceImplementations` or CQRS-based query handlers (`Intent.MediatR`).

## Intent-Specific Patterns
- Extensions encapsulate converting a query into the correct wrapper reducing repetition.
- DTO wrapping enforces consistent API responses and allows future augmentation (e.g., including server timing).

## Pagination Defaults

Default values for all paginated queries or operations can be set on the `Application Settings` screen.

- **Page Size Default** - The default 'page size' to be applied to all paged services.
- **Order By Default** - The default 'order by' to be applied to all paged services.

If the service has been exposed as an HTTP Endpoint, then the defaults will also be applied to the controller and reflect on the OpenAPI documentation.

## Related Modules
- **Intent.Application.Dtos** - Base DTO infrastructure
- **Intent.Entities.Repositories.Api** - Repository patterns with pagination support
- **Intent.EntityFrameworkCore** - Entity Framework implementation with pagination
- **Intent.MongoDb** - MongoDB repositories with pagination
- **Intent.CosmosDB** - Cosmos DB repositories with pagination
- **Intent.Application.MediatR** - CQRS query handlers that return paginated results

## External Resources
- [Pagination in EF Core (Microsoft)](https://learn.microsoft.com/ef/core/querying/pagination)
- [API Pagination Best Practices (Microsoft)](https://learn.microsoft.com/azure/architecture/best-practices/api-design#pagination)
- [Cursor Pagination Explained (GraphQL)](https://graphql.org/learn/pagination/)
- [Offset vs Cursor Pagination](https://jsonapi.org/profiles/ethanresnick/cursor-pagination/)
