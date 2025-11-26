# Intent.Application.ServiceCallHandlers

Implement service operations using individual handler classes instead of large service classes.

## What This Module Does

This module enables the Service Call Handler pattern, where each service operation is handled by a dedicated handler class implementing a common interface. Instead of large service classes with many methods, you create focused, single-responsibility handler classes.

When enabled, Intent generates:
- One handler class per service operation
- Handler base class with dependency injection setup
- Dependency registration in DI container
- Consistent handler invocation pattern

## Generated Artifacts

### Service Call Handler Classes
For each service operation, generates:
- `IServiceCallHandler<TRequest, TResponse>` - Handler interface
- `[OperationName]Handler : IServiceCallHandler<TRequest, TResponse>` - Implementation
- Handler accepts request object, returns response object
- Handles operation logic, validation, and error scenarios

### Request/Response Objects
- **Request** - Input parameters for the operation
- **Response** - Output/return value from the operation
- Automatically generated from service operation definition

### Handler Dispatcher
- Resolves appropriate handler from dependency injection
- Invokes handler with request
- Captures and returns response
- Enables consistent error handling across all operations

## Key Design Patterns

### Single Responsibility Principle
Each handler has one reason to change: its specific operation
- Easier to test individual operations
- Clearer code organization and navigation
- Simpler cognitive load per class

### Handler Interface Contract
All handlers implement common interface:
```csharp
public interface IServiceCallHandler<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request);
}
```

### Dependency Injection
Handler dependencies injected in constructor:
- Repositories for data access
- Domain services for business logic
- Cross-cutting concerns (logging, validation)
- Current user context

### Operation Composition
Build complex operations from simpler handlers:
- Reusable handler patterns across operations
- Composition over inheritance
- Mix and match handler behaviors

## Integration with Other Modules

### Required Dependencies
- **Intent.Application.ServiceImplementations** - Service interface and container generation

## When To Use

**Use Service Call Handlers when:**
- Your service classes have 10+ operations
- Operations are largely independent
- You want maximum testability per operation
- Team prefers focused, single-responsibility classes
- You need clear operation-specific cross-cutting concerns

**Use traditional Service classes instead when:**
- Service has only 2-5 operations
- Operations share significant common logic
- You prefer fewer, larger classes
- Team is unfamiliar with handler patterns

**Use MediatR Handlers instead when:**
- You want full CQRS pattern separation
- You need sophisticated pipeline behaviors
- Operations fit cleanly into Command/Query categories
- Cross-cutting concerns are better expressed as pipeline behaviors

## Handler Pattern Example

```csharp
// Auto-generated Request DTO
public class CreateProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Auto-generated Response DTO
public class CreateProductResponse
{
    public int ProductId { get; set; }
    public string Name { get; set; }
}

// Auto-generated Handler Interface
public interface IServiceCallHandler<CreateProductRequest, CreateProductResponse>
{
    Task<CreateProductResponse> HandleAsync(CreateProductRequest request);
}

// User-implemented Handler
public class CreateProductHandler : IServiceCallHandler<CreateProductRequest, CreateProductResponse>
{
    private readonly IProductRepository _repository;
    private readonly ICurrentUser _currentUser;

    public CreateProductHandler(IProductRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<CreateProductResponse> HandleAsync(CreateProductRequest request)
    {
        // Validate authorization
        if (!_currentUser.IsInRole("Admin"))
            throw new UnauthorizedException();

        // Create entity
        var product = new Product { Name = request.Name, Price = request.Price };
        
        // Persist
        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        // Return response
        return new CreateProductResponse
        {
            ProductId = product.Id,
            Name = product.Name
        };
    }
}
```

## Related Modules

- **Intent.Application.ServiceImplementations** - Service interface generation and registration

