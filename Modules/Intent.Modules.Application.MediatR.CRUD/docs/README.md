# Intent.Application.MediatR.CRUD

This module provides automatic implementation of CRUD (Create, Read, Update, Delete) operations for Commands and Queries modeled in the Services Designer when using the CQRS pattern with MediatR.

## What is CRUD?

CRUD stands for Create, Read, Update, and Delete - the four basic operations for persistent storage. This module automates the implementation of these operations by generating handler code based on mapping actions you configure in the Services Designer, eliminating the need to write repetitive data access code.

## What This Module Generates

When you model Commands and Queries with appropriate mapping actions, this module generates complete handler implementations that:

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

This module works in conjunction with `Intent.Application.DomainInteractions`. CRUD implementations are triggered by **mapping actions** you configure on Commands and Queries:

1. **Model your Command/Query** in the Services Designer
2. **Add mapping actions** (Create Entity, Update Entity, Query Entity, Delete Entity)
3. **Configure mappings** between DTO fields and entity properties
4. **Run Software Factory** - handlers are fully implemented

### Key Concept: Mapping Actions Drive Implementation

Simply having a Command or Query is not enough. The module generates implementations based on **explicit mapping actions** that connect your DTOs to domain entities. "Map from Domain" only creates the DTO structure - you need mapping actions for CRUD behavior.

## Creating CRUD Operations

### Quick Start with "Create CRUD CQRS Operations"

The fastest way to create CRUD operations is using Intent Architect's built-in wizard:

1. In the Services Designer, right-click and select **Create CRUD CQRS Operations**
2. Select the entity you want CRUD operations for
3. Intent creates all four operations (Create, Update, Delete, Query by ID) with mapping actions pre-configured

This creates a complete set of CRUD Commands and Queries ready to use.

### Example: Create Operation

From the [Building an Application tutorial](https://docs.intentarchitect.com/articles/tutorials/building-an-application/building-an-application.html):

1. Create a Command in the Services Designer
2. Right-click the Command and select **Create Entity**
3. Select the target entity (e.g., `BasketItem`)
4. In the mapping dialog, double-click the entity to map all fields
5. The generated handler will have full implementation:

```csharp
public async Task<Guid> Handle(AddToBasketCommand request, CancellationToken cancellationToken)
{
    var basket = await _basketRepository.FindByIdAsync(request.BasketId, cancellationToken);
    if (basket is null)
    {
        throw new NotFoundException($"Could not find Basket '{request.BasketId}'");
    }

    var basketItem = new BasketItem
    {
        Quantity = request.Quantity,
        UnitPrice = request.UnitPrice,
        ProductId = request.ProductId,
    };
    basket.BasketItems.Add(basketItem);

    _basketRepository.Update(basket);

    return basketItem.Id;
}
```

Note how the module:
- Automatically fetched the parent `Basket` (composite relationship)
- Mapped Command properties to the `BasketItem` entity
- Added the item to the parent's collection
- Returned the new entity's ID

## Supported CRUD Operations

### Create Operations

**Mapping Action:** Create Entity

Generates code that:
- Creates a new entity instance
- Maps DTO properties to entity properties
- Adds entity to repository
- Returns the entity's ID (or other return type)

For composite entities, automatically fetches and validates parent entities.

### Update Operations

**Mapping Action:** Update Entity

Generates code that:
- Fetches the existing entity by ID
- Updates entity properties from DTO
- Calls repository Update method

### Delete Operations

**Mapping Action:** Delete Entity

Generates code that:
- Fetches the entity by ID
- Removes it from the repository
- Throws `NotFoundException` if entity doesn't exist

### Query Operations (Find by ID)

**Mapping Action:** Query Entity (single result)

Generates code that:
- Fetches entity by ID from repository
- Maps entity to DTO
- Throws `NotFoundException` if not found

### Query Operations (List/Filter)

**Mapping Action:** Query Entity (collection result)

Generates code that:
- Queries entities from repository
- Applies filtering based on query parameters
- Maps entities to DTOs
- Supports pagination when configured

## PATCH Support for Partial Updates

When you need to update only specific fields (partial update), use HTTP PATCH semantics:

1. Model an Update Command
2. Make relevant domain entity attributes **nullable**
3. Make corresponding Command DTO fields **nullable**
4. Expose the Command as HTTP endpoint with **Verb = PATCH** (in Http Settings stereotype)

The generated code will only update fields that are present (non-null) in the request:

```csharp
public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
{
    var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
    if (product is null)
    {
        throw new NotFoundException($"Could not find Product '{request.Id}'");
    }

    if (request.Name != null)
    {
        product.Name = request.Name;
    }
    if (request.Description != null)
    {
        product.Description = request.Description;
    }
    if (request.Price != null)
    {
        product.Price = request.Price.Value;
    }

    _productRepository.Update(product);
}
```

**Requirements for PATCH:**
- Domain attributes must be nullable
- Command/DTO fields must be nullable
- HTTP Verb must be set to "PATCH"

## Query Filtering

You can model query filters using the **Query Action** in the Services Designer:

1. Add properties to your Query (e.g., `CustomerId`, `Status`)
2. Apply **Query Action** mapping
3. Configure how request properties map to query filters

The module generates LINQ expressions for filtering:

```csharp
var orders = await _orderRepository
    .FindAllAsync(x => x.CustomerId == request.CustomerId, cancellationToken);
```

Supports:
- Equality comparisons
- Boolean properties (simplified expressions: `x.IsActive` instead of `x.IsActive == true`)
- Literal values in filters
- Complex nested queries

## Pagination Support

When `Intent.Application.Dtos.Pagination` component is installed:

1. Add paging parameters to your Query (PageNo, PageSize)
2. The module generates paginated query code:

```csharp
var orders = await _orderRepository
    .FindAllAsync(
        x => x.CustomerId == request.CustomerId,
        queryOptions => queryOptions
            .OrderBy(x => x.OrderDate)
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize),
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

By default, handlers are generated with `[IntentManaged(Mode.Fully)]` on the method body when mappings are configured. The Software Factory manages the entire implementation.

If mappings are not fully configured or you want to customize:
- The method body uses `[IntentManaged(Mode.Merge)]`
- A commented `throw new NotImplementedException()` is generated with `[IntentInitialGen]`
- You can add custom logic alongside generated code

For complete custom control, change the attribute to `[IntentManaged(Mode.Ignore)]` and the Software Factory will not modify your implementation.

## When to Use MediatR CRUD vs Service Implementations CRUD

### Use MediatR CRUD (this module) when:
- You're using the CQRS pattern with MediatR
- Operations are modeled as discrete Commands and Queries
- You want request/response objects as first-class types
- You benefit from MediatR pipeline behaviors (validation, logging, etc.)

### Use Service Implementations CRUD when:
- You prefer traditional service-oriented design
- Operations are modeled as methods on a service class
- Your team is more familiar with class-based services
- You're working with existing service-oriented patterns

Both modules provide the same CRUD implementation capabilities through mapping actions.

## Integration with Other Modules

This module works with:

- **Intent.Application.MediatR** - Provides the CQRS Command/Query infrastructure
- **Intent.Application.DomainInteractions** - Provides the mapping action system that drives CRUD implementations
- **Intent.Application.Dtos** - DTOs for request/response contracts
- **Intent.Application.Dtos.Pagination** - Pagination support for query operations
- **Intent.Entities** - Domain entities being operated on
- **Intent.EntityFrameworkCore** / **Intent.CosmosDB** / etc. - Repository implementations

For more on the CQRS pattern with MediatR, see [Intent.Application.MediatR documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-mediatr/intent-application-mediatr.html).

## Related Modules

- [Intent.Application.MediatR](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-mediatr/intent-application-mediatr.html) - CQRS with MediatR
- [Intent.Application.ServiceImplementations.Conventions.CRUD](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-serviceimplementations-crud/intent-application-serviceimplementations-crud.html) - CRUD for traditional services
- [Intent.Application.DomainInteractions](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-domaininteractions/intent-application-domaininteractions.html) - Mapping actions system
- [Intent.Application.Dtos.Pagination](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-dtos-pagination/intent-application-dtos-pagination.html) - Pagination patterns

## External Resources

- [CQRS Pattern](https://learn.microsoft.com/azure/architecture/patterns/cqrs)
- [MediatR Library](https://github.com/jbogard/MediatR)
- [Building an Application Tutorial](https://docs.intentarchitect.com/articles/tutorials/building-an-application/building-an-application.html)
