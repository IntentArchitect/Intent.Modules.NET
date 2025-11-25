# Intent.Application.ServiceImplementations.Conventions.CRUD

This module provides automatic implementation of CRUD (Create, Read, Update, Delete) operations for traditional service operations modeled in the Services Designer using a class-based service pattern.

## What is CRUD?

CRUD stands for Create, Read, Update, and Delete - the four basic operations for persistent storage. This module automates the implementation of these operations by generating service method code based on mapping actions you configure in the Services Designer, eliminating the need to write repetitive data access code.

## What This Module Generates

When you model service operations with appropriate mapping actions, this module generates complete method implementations that:

- **Create Operations**: Insert new entities into the database
- **Update Operations**: Modify existing entities (including PATCH support for partial updates)
- **Delete Operations**: Remove entities from the database
- **Query Operations**: 
  - Find entities by ID
  - List/query entities with filtering support
  - Query with pagination (when `Intent.Application.Dtos.Pagination` is installed)

The generated code includes:
- Repository interactions (fetching, saving, deleting)
- DTO-to-entity mapping
- Entity-to-DTO mapping
- Query filtering and pagination
- Composite entity handling (automatically fetches parent entities when needed)

## How It Works

This module works in conjunction with `Intent.Application.DomainInteractions`. CRUD implementations are triggered by **mapping actions** you configure on service operations:

1. **Model your Service** in the Services Designer
2. **Add operations** (CreateCustomer, UpdateProduct, DeleteOrder, GetById, etc.)
3. **Add mapping actions** (Create Entity, Update Entity, Query Entity, Delete Entity)
4. **Configure mappings** between DTO fields and entity properties
5. **Run Software Factory** - service methods are fully implemented

### Key Concept: Mapping Actions Drive Implementation

Simply having a service operation is not enough. The module generates implementations based on **explicit mapping actions** that connect your DTOs to domain entities. "Map from Domain" only creates the DTO structure - you need mapping actions for CRUD behavior.

## Creating CRUD Operations

### Quick Start with "Create CRUD Operations"

The fastest way to create CRUD operations is using Intent Architect's built-in wizard:

1. In the Services Designer, right-click and select **Create CRUD Operations**
2. Select **Services** (not CQRS)
3. Select the entity you want CRUD operations for
4. Intent creates all four operations (Create, Update, Delete, Get by ID) with mapping actions pre-configured

This creates a service class with CRUD methods ready to use.

### Manual Operation Creation

You can also create operations manually:

1. Create a Service in the Services Designer
2. Add an operation (e.g., `CreateCustomer`)
3. Add parameters (name, email, etc.)
4. Right-click the operation and select **Create Entity**
5. Select the target entity
6. Configure the mapping in the dialog

## Supported CRUD Operations

### Create Operations

**Mapping Action:** Create Entity

Generates code that:
- Creates a new entity instance
- Maps operation parameters to entity properties
- Adds entity to repository
- Returns the entity's ID (or other return type)

Example generated code:

```csharp
public async Task<Guid> CreateCustomer(string name, string email, CancellationToken cancellationToken = default)
{
    var customer = new Customer
    {
        Name = name,
        Email = email,
    };

    _customerRepository.Add(customer);
    await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    return customer.Id;
}
```

For composite entities, automatically fetches and validates parent entities.

### Update Operations

**Mapping Action:** Update Entity

Generates code that:
- Fetches the existing entity by ID
- Updates entity properties from parameters
- Calls repository Update method

Example generated code:

```csharp
public async Task UpdateCustomer(Guid id, string name, string email, CancellationToken cancellationToken = default)
{
    var customer = await _customerRepository.FindByIdAsync(id, cancellationToken);
    if (customer is null)
    {
        throw new NotFoundException($"Could not find Customer '{id}'");
    }

    customer.Name = name;
    customer.Email = email;

    _customerRepository.Update(customer);
}
```

### Delete Operations

**Mapping Action:** Delete Entity

Generates code that:
- Fetches the entity by ID
- Removes it from the repository
- Throws `NotFoundException` if entity doesn't exist

Example generated code:

```csharp
public async Task DeleteCustomer(Guid id, CancellationToken cancellationToken = default)
{
    var customer = await _customerRepository.FindByIdAsync(id, cancellationToken);
    if (customer is null)
    {
        throw new NotFoundException($"Could not find Customer '{id}'");
    }

    _customerRepository.Remove(customer);
}
```

### Query Operations (Find by ID)

**Mapping Action:** Query Entity (single result)

Generates code that:
- Fetches entity by ID from repository
- Maps entity to DTO
- Throws `NotFoundException` if not found

Example generated code:

```csharp
public async Task<CustomerDto> GetCustomer(Guid id, CancellationToken cancellationToken = default)
{
    var customer = await _customerRepository.FindByIdAsync(id, cancellationToken);
    if (customer is null)
    {
        throw new NotFoundException($"Could not find Customer '{id}'");
    }

    return customer.MapToCustomerDto(_mapper);
}
```

### Query Operations (List/Filter)

**Mapping Action:** Query Entity (collection result)

Generates code that:
- Queries entities from repository
- Applies filtering based on operation parameters
- Maps entities to DTOs
- Supports pagination when configured

Example generated code:

```csharp
public async Task<List<OrderDto>> GetCustomerOrders(Guid customerId, CancellationToken cancellationToken = default)
{
    var orders = await _orderRepository.FindAllAsync(x => x.CustomerId == customerId, cancellationToken);
    return orders.MapToOrderDtoList(_mapper);
}
```

## PATCH Support for Partial Updates

When you need to update only specific fields (partial update), use HTTP PATCH semantics:

1. Model an Update operation
2. Make relevant domain entity attributes **nullable**
3. Make corresponding operation parameters **nullable**
4. Expose the operation as HTTP endpoint with **Verb = PATCH** (in Http Settings stereotype)

The generated code will only update fields that are present (non-null) in the request:

```csharp
public async Task UpdateProduct(Guid id, string? name, string? description, decimal? price, CancellationToken cancellationToken = default)
{
    var product = await _productRepository.FindByIdAsync(id, cancellationToken);
    if (product is null)
    {
        throw new NotFoundException($"Could not find Product '{id}'");
    }

    if (name != null)
    {
        product.Name = name;
    }
    if (description != null)
    {
        product.Description = description;
    }
    if (price != null)
    {
        product.Price = price.Value;
    }

    _productRepository.Update(product);
}
```

**Requirements for PATCH:**
- Domain attributes must be nullable
- Operation parameters must be nullable
- HTTP Verb must be set to "PATCH"

## Query Filtering

You can model query filters using the **Query Action** in the Services Designer:

1. Add parameters to your operation (e.g., `customerId`, `status`)
2. Apply **Query Action** mapping
3. Configure how parameters map to query filters

The module generates LINQ expressions for filtering:

```csharp
var orders = await _orderRepository
    .FindAllAsync(x => x.CustomerId == customerId && x.Status == status, cancellationToken);
```

Supports:
- Equality comparisons
- Boolean properties (simplified expressions: `x.IsActive` instead of `x.IsActive == true`)
- Literal values in filters
- Complex nested queries

## Pagination Support

When `Intent.Application.Dtos.Pagination` component is installed:

1. Add paging parameters to your operation (PageNo, PageSize)
2. The module generates paginated query code:

```csharp
var orders = await _orderRepository
    .FindAllAsync(
        x => x.CustomerId == customerId,
        queryOptions => queryOptions
            .OrderBy(x => x.OrderDate)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize),
        cancellationToken);
```

For more on pagination, see the [Intent.Application.Dtos.Pagination documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-dtos-pagination/intent-application-dtos-pagination.html).

## Composite Entity Handling

When working with composite entities (entities that cannot exist without a parent), the module automatically:

1. Fetches the parent entity first
2. Validates parent exists
3. Adds/updates the child entity through the parent's collection

This ensures referential integrity and follows aggregate root patterns.

## Customizing Generated Code

By default, service methods are generated with `[IntentManaged(Mode.Fully)]` on the method body when mappings are configured. The Software Factory manages the entire implementation.

If mappings are not fully configured or you want to customize:
- The method body uses `[IntentManaged(Mode.Merge)]`
- A commented `throw new NotImplementedException()` is generated with `[IntentInitialGen]`
- You can add custom logic alongside generated code

For complete custom control, change the attribute to `[IntentManaged(Mode.Ignore)]` and the Software Factory will not modify your implementation.

## When to Use Service Implementations CRUD vs MediatR CRUD

### Use Service Implementations CRUD (this module) when:
- You prefer traditional object-oriented service design
- Operations are naturally grouped by business capability
- Your team is familiar with service-oriented patterns
- You want methods on a class rather than discrete request objects
- You're working with existing service-oriented patterns

### Use MediatR CRUD when:
- You're using the CQRS pattern with MediatR
- Operations benefit from pipeline behaviors (validation, logging, transactions)
- You prefer loose coupling via the mediator pattern
- Individual operations have complex, distinct concerns
- You want request/response as first-class objects

Both modules provide the same CRUD implementation capabilities through mapping actions.

## Integration with Other Modules

This module works with:

- **Intent.Application.ServiceImplementations** - Provides the service class infrastructure
- **Intent.Application.DomainInteractions** - Provides the mapping action system that drives CRUD implementations
- **Intent.Application.Dtos** - DTOs for response contracts
- **Intent.Application.Dtos.Pagination** - Pagination support for query operations
- **Intent.Entities** - Domain entities being operated on
- **Intent.EntityFrameworkCore** / **Intent.CosmosDB** / etc. - Repository implementations

For more on traditional Application Services, see [Intent.Application.ServiceImplementations documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-serviceimplementations/intent-application-serviceimplementations.html).

## Related Modules

- [Intent.Application.ServiceImplementations](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-serviceimplementations/intent-application-serviceimplementations.html) - Traditional Application Services
- [Intent.Application.MediatR.CRUD](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-mediatr-crud/intent-application-mediatr-crud.html) - CRUD for CQRS with MediatR
- [Intent.Application.DomainInteractions](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-domaininteractions/intent-application-domaininteractions.html) - Mapping actions system
- [Intent.Application.Dtos.Pagination](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-dtos-pagination/intent-application-dtos-pagination.html) - Pagination patterns

## External Resources

- [Building an Application Tutorial](https://docs.intentarchitect.com/articles/tutorials/building-an-application/building-an-application.html)
- [Service-Oriented Architecture](https://learn.microsoft.com/dotnet/architecture/microservices/architect-microservice-container-applications/service-oriented-architecture)
