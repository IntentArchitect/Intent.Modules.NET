# Intent.Application.ServiceImplementations

This module provides a standard implementation pattern for traditional Application Services using a class-based approach with operations represented as methods.

## What are Application Services?

Application Services are classes that represent the application layer's coordination point between external requests (from controllers, APIs, etc.) and your domain logic. Unlike CQRS with MediatR where operations are modeled as discrete request/response objects, Application Services use a traditional object-oriented approach with methods on a service class.

## What This Module Generates

The `Intent.Application.ServiceImplementations` module generates service implementation classes for services modeled in the Services Designer:

- **Service Implementation Classes** - Concrete classes implementing service interfaces
- **Method Implementations** - Service methods with parameters and return types
- **Dependency Injection** - Constructor injection for repositories, domain services, and infrastructure
- **DTO Mapping** - Integration with mapping patterns (AutoMapper, Mapperly)
- **Transaction Coordination** - Orchestration of domain operations

## Application Service Paradigm

**Intent.Application.ServiceImplementations** implements the **Traditional Service Paradigm**, which:

- Combines read and write logic into a single service
- Uses a unified data model and service structure
- Groups operations naturally by business capability
- Simplifies development with a traditional object-oriented approach
- Common in systems with straightforward requirements or minimal scalability concerns

This paradigm is appropriate when:
- Your domain model and service structure are naturally unified
- Operations are grouped by business capability
- You prefer a class-based approach with methods
- Your team is familiar with traditional service-oriented patterns

## Alternative Paradigm: CQRS with MediatR

The **CQRS Paradigm** (Command Query Responsibility Segregation) is available through the `Intent.Application.MediatR` module and:

- Separates read and write responsibilities into distinct models optimized for their respective purposes
- Models each operation as a discrete request/response object (use case-centric)
- Enables specialized pipeline behaviors (validation, logging, transactions)
- Provides loose coupling through the mediator pattern
- Ideal for systems with complex requirements or high scalability demands

This paradigm is appropriate when:
- Your operations benefit from independent optimization and pipeline behaviors
- You want strict separation between commands and queries
- Individual operations have complex, distinct concerns
- You require loose coupling via the mediator pattern

## Service Modeling

Services are modeled in the Intent Architect Services Designer:

![Service Modeling](images/service-modeling.png)

Each service can have multiple operations, with each operation defining:
- **Parameters** - Input values with types
- **Return Types** - What the operation returns
- **DTO Mappings** - How domain entities map to DTOs

## Generated Code Example

### Service Interface (from Intent.Application.Contracts)

```csharp
public interface ICustomerService
{
    Task<Guid> CreateCustomer(string name, string surname, string email);
    Task<CustomerDto> GetCustomer(Guid id);
    Task UpdateCustomer(Guid id, string name, string surname, string email);
    Task DeleteCustomer(Guid id);
}
```

### Service Implementation

```csharp
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Guid> CreateCustomer(string name, string surname, string email)
    {
        var customer = new Customer
        {
            Name = name,
            Surname = surname,
            Email = email
        };

        _customerRepository.Add(customer);
        return customer.Id;
    }

    public async Task<CustomerDto> GetCustomer(Guid id)
    {
        var customer = await _customerRepository.FindByIdAsync(id);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with id {id} not found");
        }
        
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task UpdateCustomer(Guid id, string name, string surname, string email)
    {
        var customer = await _customerRepository.FindByIdAsync(id);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with id {id} not found");
        }

        customer.Name = name;
        customer.Surname = surname;
        customer.Email = email;
    }

    public async Task DeleteCustomer(Guid id)
    {
        var customer = await _customerRepository.FindByIdAsync(id);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with id {id} not found");
        }

        _customerRepository.Remove(customer);
    }
}
```

## Service Responsibilities

Application Service implementations typically:

1. **Coordinate domain operations** - Call domain entities, value objects, and domain services
2. **Manage transactions** - Ensure operations complete atomically
3. **Translate between layers** - Convert between DTOs and domain models
4. **Orchestrate workflows** - Coordinate multiple repository calls and domain logic
5. **Handle cross-cutting concerns** - Validation, authorization, logging (via decorators/interceptors)

## Integration with Other Modules

### Intent.Application.Contracts
Defines the service interfaces that this module implements.

### Intent.Application.AutoMapper / Intent.Application.Dtos.Mapperly
Provides mapping between domain entities and DTOs.

### Intent.EntityFrameworkCore / Intent.MongoDB
Repository implementations for data access.

### Intent.Application.DependencyInjection
Registers service implementations in the DI container.

### Intent.Application.FluentValidation
Can be used with decorators for input validation.

## Related Patterns

- **CQRS with MediatR** - Alternative pattern using `Intent.Application.MediatR` module
- **Domain Services** - Business logic services in domain layer via `Intent.DomainServices`
- **Repository Pattern** - Data access abstraction via various persistence modules
- **Domain Events** - Internal events within bounded context
