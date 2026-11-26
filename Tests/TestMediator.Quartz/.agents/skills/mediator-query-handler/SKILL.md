---
name: mediator-query-handler
description: Implements the whole body of a Mediator query handler from the interactions modelled on the query. MUST load before creating, editing or revisiting any query handler e.g. *QueryHandler.cs, and after any Software Factory run that generates a new query handler stub.
template-id: Intent.Application.Mediator.QueryHandlerSkill
contentHash: 39A9D66DBD1B8902B4D56298C5909351A63A07B72593A538872D1EB0079FCA2F
---
# Mediator Query Handler

Write the `Handle` body of a query handler. The generator only emits a stub that throws `NotImplementedException`; the associations modelled on the query in the Services designer are the specification.

## Finding the handlers to write

- After every Software Factory run, search the Application project for `*QueryHandler.cs` files whose `Handle` still throws `NotImplementedException`, and write every one.
- When you add, remove, reorder or change an association on a query, update its handler in the same piece of work. No generated file changes, so nothing else will remind you.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body. Never change its name, return type or parameters, and keep it `async` even when it awaits nothing.
- The handler implements `IQueryHandler<TQuery, TResponse>` from the `Mediator` library and returns `ValueTask<TResponse>`.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Read through repository interfaces. Filter, sort and page in the query the repository runs, never in memory after loading everything. Project to the response in that query when the repository supports it.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByCriteriaAsync`, `GetSummaryAsync`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Follow the nearest similar query handler for paging, sorting, the paged result type and the not-found pattern.
- A query changes no state.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make filtering or mapping easier to read.

## Read the model first

Load the `exploring-the-model` skill and call `run_designer_query` with `getElementDetails` on the query. Implement its associations in the order the designer shows them; later steps may use earlier results. Re-read them whenever you revisit the handler.

Field mappings are not modelled. Ignore any minimal mapping or missing-mapping warning on an association, and add none: decide which query field filters which attribute, and which attribute fills which response property, by name and meaning.

- Query Entity: load the matching instances through the repository. This is a query's main interaction; shape the response from its result.
- Call Service Operation (Invoke): call the operation through its Application interface. When it targets another query, dispatch it with `Mediator.ISender`.
- Create, Update or Delete Entity, Publish Integration Event, Send Integration Command: unusual on a query. Implement what is modelled and tell the user it is unusual.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.
