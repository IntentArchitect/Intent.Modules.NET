---
name: mediatr-query-handler
description: Implements business logic in MediatR query handler Handle methods. MUST load before creating or editing any query handler e.g. *QueryHandler.cs.
template-id: Intent.Application.MediatR.QueryHandlerSkillTemplate
contentHash: 11ED53965201A264A7B7C9179E21FB599C705D17243A54F54AE99FB74E570C2D
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
