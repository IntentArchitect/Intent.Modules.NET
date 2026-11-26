---
name: application-service-implementation
description: Implements business logic in traditional (non-CQRS) application services. MUST load before creating or editing a traditional application service class in the Application layer e.g. *Service.cs under Implementation.
template-id: Intent.Application.ServiceImplementations.ServiceImplementationSkillTemplate
contentHash: EF3FE63C83313E5C713E71DC6432F0506112D23E78728B1DAD2203B6927EB48A
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

## Mapping (AutoMapper)

- Return entity-shaped `*Dto` types through AutoMapper, not `new XxxDto { … }`: use the generated `MapToXxxDto(_mapper)` and `MapToXxxDtoList(_mapper)` extensions where they exist, otherwise `Map` or `ProjectTo`. Before you use a mapping, find its `CreateMap<TEntity, TDto>()`: a generated DTO declares it in its own `Mapping(Profile)` (`IMapFrom<TEntity>`), anything else in a `Profile` class. A `ProjectTo` call that compiles, or a mapping for another type, does not prove it exists. If there is none, add a profile, nested mappings included, as `Mappings/<Aggregate>/<Entity>DtoProfile.cs` in the Application project, and never map a DTO a second time. Profiles are found by assembly scanning, so leave the AutoMapper registration alone.
- Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment. A missing mapping is not a reason: add it.

**Example**
```csharp
// Generated DTO: the mapping is on the DTO, and MapToCustomerDto is generated with it
public class CustomerDto : IMapFrom<Customer>
{
    public void Mapping(Profile profile) => profile.CreateMap<Customer, CustomerDto>();
}

return customer.MapToCustomerDto(_mapper);

// No mapping yet: Mappings/Customers/CustomerSummaryDtoProfile.cs
public class CustomerSummaryDtoProfile : Profile
{
    public CustomerSummaryDtoProfile() => CreateMap<Customer, CustomerSummaryDto>();
}
```
