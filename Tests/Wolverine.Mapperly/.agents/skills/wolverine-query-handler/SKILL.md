---
name: wolverine-query-handler
description: Implements business logic in Wolverine query handler Handle methods. MUST load before creating or editing any query handler e.g. *QueryHandler.cs.
template-id: Intent.Application.Wolverine.QueryHandlerSkillTemplate
contentHash: 5700758D0471AB3CA3F75EC436FE82ED8D2F7F9E09E25248BCE95202609192D3
---
# Wolverine Query Handler

Fill in the `Handle` method of an existing query handler. Its signature is generated; only the body is yours.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- Wolverine handlers are plain classes found by naming convention. Never make one implement a handler interface such as `IRequestHandler`.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Read through repository interfaces. Filter, sort and page in the query the repository runs, never in memory after loading everything.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`ListByCriteriaAsync`, `GetSummaryAsync`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Follow the nearest similar query handler for paging, sorting and the response shape.
- A query changes no state.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make filtering or mapping easier to read.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.

## Mapping (Mapperly)

- Return entity-shaped `*Dto` types through a Mapperly mapper, not `new XxxDto { … }`. Before you use one, find the `[Mapper]` partial class with its mapping method, for example `CustomerToCustomerDto(Customer customer)`. If there is none, add it, nested mappers (`[UseMapper]`) and list overloads included, as `Mappings/<Aggregate>/<Entity>DtoMapper.cs` in the Application project, and register a new mapper the way the existing ones are, usually `services.AddSingleton<CustomerDtoMapper>()`.
- Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment.
