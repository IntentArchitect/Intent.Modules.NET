---
name: mediatr-query-handler
description: Implements business logic in MediatR query handler Handle methods. MUST load before creating or editing any query handler e.g. *QueryHandler.cs.
template-id: Intent.Application.MediatR.QueryHandlerSkillTemplate
contentHash: 4BC2F436F1001743438AC69E89C9D1CD83CB2C90F8FF857A95396F02B8DB2880
---
# MediatR Query Handler

Fill in the `Handle` method of an existing query handler. Its signature is generated; only the body is yours.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Read through repository interfaces. Filter, sort and page in the query the repository runs, never in memory after loading everything.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`ListByCriteriaAsync`, `GetSummaryAsync`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Follow the nearest similar query handler for paging, sorting and the response shape.
- A query changes no state.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make filtering or mapping easier to read.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, but lazy loading is off, so explicitly load any other related data in a repository method.

## Mapping (AutoMapper)

- Return entity-shaped `*Dto` types through AutoMapper (`Map` or `ProjectTo`), not `new XxxDto { … }`. Before you use one, find its `CreateMap<TEntity, TDto>()` in a `Profile`. If there is none, add the profile, nested mappings included, as `Mappings/<Aggregate>/<Entity>DtoProfile.cs` in the Application project. Profiles are found by assembly scanning, so leave the AutoMapper registration alone.
- Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment.
