# Intent.EntityFrameworkCore.Repositories

The Repository Pattern is a design pattern used in software development to encapsulate data access logic, providing a clean separation between the domain/business logic and data persistence. It acts as an intermediary between the application and the data source (such as a database), abstracting query operations and ensuring that the underlying data access logic is centralized and reusable. This pattern improves testability, maintainability, and flexibility by allowing the data access strategy to change without affecting business logic. In .NET applications, repositories often work alongside the Unit of Work pattern and ORM frameworks like Entity Framework to manage transactions efficiently.

## Data Fetching Strategies

Eager and lazy loading are strategies within the broader topic of Object-Relational Mapping (ORM) and Data Fetching Strategies.

- Eager Loading: Loads related entities immediately with the main entity using .Include(). This is useful when you need related data upfront to avoid multiple database queries.
- Lazy Loading: Loads related entities only when they are accessed for the first time. This can reduce initial query cost but may lead to multiple queries (N+1 problem).

There are several ways to achieve these strategies.

### Enabling Lazy Loading Proxies

There is an application setting `Database Settings - Lazy loading with proxies`

This setting allows you to configure whether you would like to use Entity Frameworks, Lazy loading with proxies feature.
This setting is on by default, but can be turned off if you don't want this behaviour.

For more info on lazy loading with proxies check out [the official documentation](https://learn.microsoft.com/ef/core/querying/related-data/lazy#lazy-loading-with-proxies).

### Modeling Owned Relationships

Any relationships which are modeled as owned (i.e. Black Diamonds) are automatically configured by EF to eager load. In this example loading an `Order` entity will always eager load the `OrderItems` collection, because its `Owned`.

![Owned Relationshipd](images/owned-relationship.png)

### Extending Entity Repositories to use `Includes`


Given the above relationships, lets assume we wanted to eager load the `Orders` when loading a `Customer`.

Here are several examples of how we could extend the entity pattern to implement eager loading.

Below are 3 examples of implementing eager loading

1. Setting the repository to eager load `Orders` for all repository "Find" operations
2. Add a custom operations which eager load the `Orders` for this operation
3. Override a base "Find" operation to eager load `Orders`

``` csharp

public class CustomerRepository : RepositoryBase<Customer, Customer, ApplicationDbContext>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    // Setting the repository to eager load `Orders` for all repository "Find" operations
    protected override IQueryable<Customer> CreateQuery()
    {
        var result = base.CreateQuery();
        return result.Include(c => c.Orders);
    }

    // Add a custom operations which eager load the `Orders` for this operation
    public Task<List<Customer>> FindAllWithOrdersAsync(CancellationToken cancellationToken = default)
    {
        return QueryInternal(filterExpression: null).Include(c => c.Orders).ToListAsync<Customer>(cancellationToken);
    }

    // Override a base "Find" operation to eager load `Orders`
    public override Task<List<Customer>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        return QueryInternal(filterExpression: null).Include(c => c.Orders).ToListAsync<Customer>(cancellationToken);
    }

```

The `Include` extension method is part of the `Microsoft.EntityFramework` NuGet package. If you are implementing an architecture like `Clean Architecture` this should only be used in your technology layer, as it is a technology concern. Some teams add this `Microsoft.EntityFramework` NuGet package to their application layer so that they can use the `Include` on a per repository call basis, for example.

```csharp

public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
{
    // passing in an Include expression
    var customers = await _customerRepository.FindAllAsync( o => o.Include(c => c.Orders), cancellationToken);
    return customers.MapToCustomerDtoList(_mapper);
}
```

Some make this technical trade-off (compromise) for the convenience of being able to customer the eager loading per repository call.  