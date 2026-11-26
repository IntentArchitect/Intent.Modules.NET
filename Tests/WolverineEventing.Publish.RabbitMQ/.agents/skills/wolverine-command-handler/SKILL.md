---
name: wolverine-command-handler
description: Implements business logic in Wolverine command handler Handle methods. MUST load before creating or editing any command handler e.g. *CommandHandler.cs.
template-id: Intent.Application.Wolverine.CommandHandlerSkillTemplate
contentHash: B4E13D0B74488A129C1A745C9054C139AFF9F708D3A20529C276AA3FADC9B57D
---
# Wolverine Command Handler

Fill in the `Handle` method of an existing command handler. Its signature is generated; only the body is yours.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- Wolverine handlers are plain classes found by naming convention. Never make one implement a handler interface such as `IRequestHandler`.
- Change only what the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the handler, its command, response and validator, the repository interfaces and the domain types it touches, and work out the intended flow from their names and from nearby features.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Let the domain model drive the code: use its aggregates, value objects, statuses and business terms rather than names of your own.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Put durable business rules on the entity or a domain service when nearby code does that.
- Before changing state, check what nearby handlers check: that the entity exists, and that the caller may act on it (ownership, eligibility, status).
- Look for similar handlers, repository methods and domain operations before writing, and follow the nearest similar handler for result types, error handling and events. Don't invent a new result or exception style where one exists. Publish an event only where nearby code already does.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`GetForUpdateAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.
- Before finishing, review your code against every rule in this skill, the sections below included.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.
- Do not call `SaveChangesAsync`: the pipeline's unit of work commits after the handler. Call it, once and before returning, only when the handler returns a value the database generates, such as a new Id, a row version or a computed column. If unsure, leave it out, and remove any call you find that isn't needed.
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
