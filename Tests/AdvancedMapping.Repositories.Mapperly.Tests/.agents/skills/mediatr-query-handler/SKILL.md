---
name: mediatr-query-handler
description: Implements business logic in MediatR query handler Handle methods. MUST load before creating or editing any query handler e.g. *QueryHandler.cs.
template-id: Intent.Application.MediatR.QueryHandlerSkillTemplate
contentHash: B38772B4D6FE11AAEECF116B73ACD60C865AB4A49134CA26E6B28B5BFD7E8CC8
---
# MediatR Query Handler

Fill in the `Handle` method of an existing query handler. Its signature is generated; only the body is yours.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- Change only what the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the handler, its query and response, the repository interfaces and the domain types it reads, and work out the intended result from their names and from nearby features.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Let the domain model drive the code: use its statuses, identities and business terms rather than names of your own.
- Read through repository interfaces. Filter, sort and page in the query the repository runs, never in memory after loading everything.
- Apply the same business filters and visibility rules as nearby handlers (for example ownership, tenant, status or soft-delete), and load only what the response needs.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`ListByCriteriaAsync`, `GetSummaryAsync`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Look for similar query handlers, repository methods and projections before writing, and follow the nearest one for paging, sorting, mapping and the response shape. Don't invent a new approach where one exists.
- A query changes no state.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make filtering or mapping easier to read.
- Before finishing, review your code against every rule in this skill, the sections below included.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.

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
