---
name: application-service-implementation
description: Implements business logic in traditional (non-CQRS) application services. MUST load before creating or editing a traditional application service class in the Application layer e.g. *Service.cs under Implementation.
template-id: Intent.Application.ServiceImplementations.ServiceImplementationSkillTemplate
contentHash: 5AA49725845411C34C345AF5C77BBF02BF1F5BFECF442627BADFC9335E162C51
---
# Traditional Service Implementation

Fill in the operation bodies of an existing application service. Their signatures are generated; only the bodies are yours.

## Rules

- Mark each operation you write with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`.
- Change only the operations the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the service, its interface and DTOs, the repository interfaces and the domain types it touches, and work out each operation's flow from its name, parameters, return type and nearby features.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a service.
- Let the domain model drive the code: use its aggregates, value objects, statuses and business terms rather than names of your own.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Put durable business rules on the entity or a domain service when nearby code does that.
- Before changing state, check what nearby services check: that the entity exists, and that the caller may act on it (ownership, eligibility, status).
- Look for similar services, repository methods and domain operations before writing, and follow the nearest similar service for result types, error handling, mapping and events. Don't invent a new result or exception style where one exists.
- In read operations, filter, sort and page in the query the repository runs, never in memory after loading everything, and change no state. Apply the same business filters and visibility rules as nearby services (for example ownership, tenant, status or soft-delete), and load only what the response needs.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ListByCriteriaAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the service.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the service are fine when they make the flow easier to read.
- Before finishing, review your code against every rule in this skill, the sections below included.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.
- Do not call `SaveChangesAsync`: the unit of work commits after the operation returns. Call it, once and before returning, only when the operation returns a value the database generates, such as a new Id, a row version or a computed column. If unsure, leave it out, and remove any call you find that isn't needed.
- Do not call the repository's `Update`. EF tracks the entities it loaded, so changing their properties is enough. Call `Add` or `Remove` only to insert or delete, and remove any `Update` call you find on an entity loaded from a repository.

## Mapping (Mapperly)

- Return entity-shaped `*Dto` types through a Mapperly mapper, not `new XxxDto { … }`. Before you use one, find the `[Mapper]` partial class with its mapping method, for example `CustomerToCustomerDto(Customer customer)`; a mapper for a different entity does not count. If there is none, add it, nested mappers (`[UseMapper]`) and list overloads included, as `Mappings/<Aggregate>/<Entity>DtoMapper.cs` in the Application project, and register a new mapper the way the existing ones are, usually `services.AddSingleton<CustomerDtoMapper>()`.
- Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment. A missing mapper is not a reason, not even for now: add it first.

**Example**
```csharp
// Mappings/Customers/CustomerDtoMapper.cs
[Mapper]
public partial class CustomerDtoMapper
{
    public partial CustomerDto CustomerToCustomerDto(Customer customer);
    public partial List<CustomerDto> CustomerToCustomerDtoList(IEnumerable<Customer> customers);
}

// In the handler or service, with CustomerDtoMapper injected
return _mapper.CustomerToCustomerDto(customer);
```
