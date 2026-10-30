---
name: mediator-query-handler
description: Implements the whole body of a Mediator query handler from the interactions modelled on the query. MUST load before creating, editing or revisiting any query handler e.g. *QueryHandler.cs, and after any Software Factory run that generates a new query handler stub.
template-id: Intent.Application.Mediator.QueryHandlerSkill
contentHash: C6699B8EB70E1D6B8DF63A2287A7F0F98A147EFAA8B57BABA84B1D8F49CF4F89
---
# Mediator Query Handler

Write the business logic of a query handler. In this solution the handler body is always written by you, never generated: the generator emits only a stub that throws `NotImplementedException`. The associations modelled on the query in the Services designer are the specification. Keep handlers aligned with the modeled domain and existing query patterns while protecting architectural boundaries.

## Finding the handlers to write

Nothing raises a task or a reminder for a new or outdated handler. Find the work yourself:

- After every Software Factory run, search the Application project for `*QueryHandler.cs` files whose `Handle` body still holds the `// IntentInitialGen` marker and `throw new NotImplementedException`. Each one is a query handler nobody has written yet. Write every one you find.
- When you add, remove, reorder or change an association on a query in the designer, its handler is out of date even though no generated file changed. Update it in the same piece of work.

## Core rules

- Treat the existing handler file as the starting point. Write the `Handle` method body rather than rewriting unrelated code.
- The handler implements `IQueryHandler<TQuery, TResponse>` from the `Mediator` library. `Handle` returns `ValueTask<TResponse>`.
- NEVER change the name, return type or parameters of `Handle`. They are generated. The stub is always generated `async`. Keep `async` even when the body awaits nothing: removing it is a signature change, which the next Software Factory run reverts and reports as destructive.
- ALWAYS, when you write a `Handle` body, put `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on the `Handle` method so regeneration keeps your body.
- Inject only dependencies from the Domain or Application layers.
- Never introduce direct dependencies on infrastructure packages or infrastructure types in the handler, including Entity Framework, Dapper, concrete DbContexts, SQL clients, or vendor-specific read/query APIs.
- If data access is required, favor using an existing repository abstraction from the Domain or Application layers, and extend it if required.
- ALWAYS if the response requires any aggregation (Count/Sum/Average/Min/Max/First/Last/GroupBy), the handler MUST NOT compute it in memory. The handler MUST call a Domain/Application abstraction that returns an aggregated result/summary/shaped dataset. If no suitable method exists, extend the appropriate repository/read abstraction (contract first, then implementation) and then call it. No temporary in-handler aggregation is allowed.
- Follow the modeled domain language already present in the codebase, including terms for statuses, projections, identities, filters, and business concepts.
- Search the codebase for similar query handlers, read models, repository methods, projections, pagination patterns, and mapping conventions before inventing a new approach.
- Add private helper methods inside the handler when they improve readability, mapping clarity, or reuse of branching/query composition logic.
- Keep the handler focused on orchestrating retrieval, filtering, and mapping. Push reusable rules or interpretation logic into existing domain/application abstractions when nearby code already uses them.

## Read the model first

The associations drawn from the query to other elements are the specification. Read them before you write anything.

1. Load the `exploring-the-model` skill.
2. Call `run_designer_query` with `getElementDetails` on the query in the Services designer.
3. List every association on the query in the order the designer shows them. That order is the order the work happens in.
4. Implement each association in that order. Later steps may use the result of earlier ones.

Field mappings are NOT modelled. The designer may show a minimal mapping on an association because it insists on one, or warn that an association has no mappings. Neither carries intent: leave the warning, and do not add mappings to silence it, because the generator ignores them. Decide which query field filters which entity attribute, and which entity attribute fills which response property, by name and meaning, using the query's fields, the entity's attributes and the response type's properties.

### What each interaction means

- Query Entity: load one or many instances of the target entity through its repository, filtering by the query fields that match its attributes. This is the main interaction of a query; the response is shaped from its result.
- Call Service Operation (Invoke): call the modelled operation on the target service. Inject the service's interface from the Application layer and pass arguments derived from the query and from earlier results. When the Invoke targets another query, dispatch it with `Mediator.ISender`, e.g. `await _sender.Send(new OtherQuery(...), cancellationToken)`.
- Create Entity, Update Entity, Delete Entity, Publish Integration Event, Send Integration Command: a query is not expected to change state or publish messages. If one is modelled, implement it as the model says, and tell the user it is unusual for a query.

## Keep the handler in step with the model

Whenever you implement OR revisit this handler for any reason, re-read the query's associations again with `getElementDetails`. If the model changed since the body was written (an association added, removed, reordered or retargeted), update the body in the same piece of work.

A changed association changes no generated file, so nothing else will remind you. The Software Factory output is identical before and after, and the stub is already gone. Reading the model is your only check.

## Saving

Queries are read-only. A unit-of-work behaviour only wraps commands. Do not call `SaveChangesAsync` from a query handler.

## Workflow

1. Inspect the existing handler, query, response type, repository or read-service abstractions, and related domain/read-model types.
2. Read the model as described above and list the interactions in order.
3. Search for code usages of:
  - similar query handlers
  - projection or DTO mapping patterns
  - pagination, sorting, filtering, and authorization rules
  - repository or read-service methods serving similar data
4. Write the `Handle` body using existing query patterns first.
5. If the handler needs missing DAL capabilities, extend the relevant repository or read abstraction in an allowed layer instead of introducing infrastructure access into the handler.
6. Add focused private helper methods when they make filtering, mapping, or result composition easier to understand.
7. Thread the cancellation token through every async call.
8. Set `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on `Handle`.
9. Verify the final code preserves layer boundaries and avoids infrastructure leakage.

## Query-specific guidance

Queries read and shape data. Prefer patterns like these when supported by the surrounding codebase:

- retrieve through an existing repository or read abstraction
- project to the response contract in the data access call when the repository supports it, instead of loading whole entities and mapping them in memory
- read without change tracking when the repository exposes it
- apply business-relevant filters and visibility rules consistently with nearby handlers
- reuse established paging, sorting, and projection conventions; return the existing paged result type for a paged query
- when a single item is requested by identity and it does not exist, fail with the not-found pattern the solution already uses, or return null only when the response type is nullable and nearby handlers do so
- map to the existing response contract with minimal surprise
- never mutate domain state in the query flow

## Repository and read abstraction guidance

When a needed read capability is missing:

- Extend the existing repository or read abstraction closest to the business concept.
- Keep method signatures expressive in domain/application terms rather than storage terms.
- Prefer names such as `FindByIdAsync`, `FindByCriteriaAsync`, `SearchActive...Async`, or `GetSummaryAsync` over schema-oriented names.
- Do not explain or encode infrastructure implementation details in the handler.
- Do not reference EF includes, Dapper SQL, joins, or storage-specific tuning from the handler.

## AutoMapper guidance

- Any read/query method, including MediatR query handlers and application services, that returns Application-layer DTOs (`*Dto`) derived from Domain entities **MUST** use AutoMapper.
    - Do not manually construct DTOs (`new XxxDto { ... }`) on read/query paths.
- **AutoMapper gate (absolute):** If you use any `ProjectTo*`, `Find*ProjectTo*`, `FindAllProjectTo*`, or `*ProjectToAsync*` method anywhere in the call chain, you **MUST**:
    - **verify mapping exists** by locating `CreateMap<TDomain, TDto>()` in a `Profile` **and cite file path + excerpt**, **OR**
    - if verification fails, **immediately create** the required AutoMapper `Profile`(s) (including **all required nested mappings**).
    - **No assumptions allowed** (a generic projection method or other feature usage is not verification).
- **Registration assumption (do not block on DI):**
    - Assume AutoMapper is registered via assembly scanning, e.g.:services.AddAutoMapper(Assembly.GetExecutingAssembly());
    - Therefore, **do not delay profile creation** because DI registration details are not currently visible.
    - Do not modify DI registration as part of this guidance unless the user explicitly asks.
- Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation and AutoMapper is not reasonable.
    - This must include an inline code comment explaining why AutoMapper is not reasonable.
    - “Mapping doesn’t exist yet” is not a valid exception.
- If you can't find any existing mappings, create them in the same project as the services under:
    - `./Mappings/<FeatureOrAggregate>/<Entity>DtoProfile.cs`
    - Example: `MyApp.Application/Mappings/Invoices/InvoiceDtoProfile.cs`            

**Example:**
```csharp

public class CustomerDtoProfile : Profile
{
    public CustomerDtoProfile()
    {
        CreateMap<Customer, CustomerDto>();
    }
}

public static class CustomerDtoMappingExtensions
{
    public static CustomerDto MapToCustomerDto(this Customer projectFrom, IMapper mapper) =>
        mapper.Map<CustomerDto>(projectFrom);

    public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper) =>
        projectFrom.Select(x => x.MapToCustomerDto(mapper)).ToList();
}
```

## Output expectations

Produce a concrete code update that:

- implements every association modelled on the query, in model order
- fills in the `Handle` body and marks it `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`
- adds private helper methods in the handler if useful
- extends allowed-layer abstractions when needed for the handler to be correct
- preserves the existing feature style, mapping conventions, and response shape
- keeps unrelated refactors out of scope

## Review checklist

Before finishing, check that:

- the query's associations were re-read and each one is implemented
- every injected dependency belongs to the Domain or Application layers
- the handler imports no infrastructure package namespaces
- repository or read-service changes, if any, are expressed as abstractions only
- the implementation follows existing usage patterns for projections, filtering, and pagination
- business language matches the surrounding model and response contract
- the code path is cancellation-aware and async-safe
