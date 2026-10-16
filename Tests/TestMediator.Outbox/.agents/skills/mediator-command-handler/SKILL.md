---
name: mediator-command-handler
description: Implements the whole body of a Mediator command handler from the interactions modelled on the command. MUST load before creating, editing or revisiting any command handler e.g. *CommandHandler.cs, and after any Software Factory run that generates a new command handler stub.
template-id: Intent.Application.Mediator.CommandHandlerSkill
contentHash: 0533B1612EA660053AC6349522A725F5DB298FF9082CF36748A718E83C189E32
---
# Mediator Command Handler

Write the business logic of a command handler. In this solution the handler body is always written by you, never generated: the generator emits only a stub that throws `NotImplementedException`. The associations modelled on the command in the Services designer are the specification. Favor domain intent, existing code patterns, and clean layer boundaries over convenience shortcuts.

## Finding the handlers to write

Nothing raises a task or a reminder for a new or outdated handler. Find the work yourself:

- After every Software Factory run, search the Application project for `*CommandHandler.cs` files whose `Handle` body still holds the `// IntentInitialGen` marker and `throw new NotImplementedException`. Each one is a command handler nobody has written yet. Write every one you find.
- When you add, remove, reorder or change an association on a command in the designer, its handler is out of date even though no generated file changed. Update it in the same piece of work.

## Core rules

- Treat the existing handler file as the starting point. Write the `Handle` method body rather than rewriting unrelated parts of the file.
- The handler implements `ICommandHandler<TCommand, TResponse>` from the `Mediator` library (or `ICommandHandler<TCommand>` for a command with no response). `Handle` returns `ValueTask<TResponse>`.
- A command with no response returns `ValueTask<Unit>`. The body MUST end with `return Unit.Value;` on every path that completes normally.
- NEVER change the name, return type or parameters of `Handle`. They are generated. If the stub is not `async` and your body awaits, add only the `async` modifier.
- ALWAYS, when you write a `Handle` body, put `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on the `Handle` method so regeneration keeps your body.
- Inject only dependencies from the Domain or Application layers.
- Never introduce direct dependencies on infrastructure packages or infrastructure types in the handler, including Entity Framework, Dapper, concrete DbContexts, SQL clients, or vendor-specific data access abstractions.
- If data access is required, favor using an existing repository abstraction from the Domain or Application layers, and extend it if required.
- ALWAYS if the response requires any aggregation (Count/Sum/Average/Min/Max/First/Last/GroupBy), the handler MUST NOT compute it in memory. The handler MUST call a Domain/Application abstraction that returns an aggregated result/summary/shaped dataset. If no suitable method exists, extend the appropriate repository/read abstraction (contract first, then implementation) and then call it. No temporary in-handler aggregation is allowed.
- Follow the modeled domain language already present in the codebase: aggregates, entities, value objects, policies, invariants, statuses, and business terms should drive the implementation.
- Search the codebase for similar handlers, repository methods, domain services, validation flows, and result patterns before introducing a new approach.
- Add private helper methods inside the handler when they improve clarity or encapsulate repeated branching logic.
- Keep orchestration in the handler and place durable business rules in domain entities, value objects, specifications, or domain/application services when those patterns already exist nearby.

## Read the model first

The associations drawn from the command to other elements are the specification. Read them before you write anything.

1. Load the `exploring-the-model` skill.
2. Call `run_designer_query` with `getElementDetails` on the command in the Services designer.
3. List every association on the command in the order the designer shows them. That order is the order the work happens in.
4. Implement each association in that order. Later steps may use the entity loaded or created by an earlier one.

Field mappings are NOT modelled. The designer may show a minimal mapping on an association because it insists on one, but it carries no intent. Decide which command field populates which entity attribute, constructor argument or message property by name and meaning, using the command's fields and the target's attributes and constructors. When a name or meaning is genuinely ambiguous, search the codebase for a similar handler before asking.

### What each interaction means

- Create Entity: create a new instance of the target entity and add it through its repository. Use the entity's constructor or factory, and pass the command fields that match the entity's attributes by name and meaning. Do not leave required attributes unset.
- Update Entity: load the target entity from its repository using the identity carried by the command (usually an `Id` field), fail with the not-found pattern the solution already uses when it is missing, then apply the command's fields. Prefer domain methods on the entity over ad hoc property assignment when they exist.
- Delete Entity: load the target entity by the identity on the command, fail with the not-found pattern when it is missing, then remove it through the repository.
- Query Entity: load one or many instances of the target entity through its repository, filtering by the command fields that match its attributes. The result is available to the interactions that follow it.
- Call Service Operation (Invoke): call the modelled operation on the target service. Inject the service's interface from the Application layer and pass arguments derived from the command and from earlier results. When the Invoke targets a command or query, dispatch it with `Mediator.ISender`, e.g. `await _sender.Send(new OtherCommand(...), cancellationToken)`.
- Publish Integration Event: publish the target integration event through the message bus. Populate every property of the message from the command and from entities handled earlier in the handler.
- Send Integration Command: send the target integration command through the message bus. Populate every property the same way.

## Keep the handler in step with the model

Whenever you implement OR revisit this handler for any reason, re-read the command's associations again with `getElementDetails`. If the model changed since the body was written (an association added, removed, reordered or retargeted), update the body in the same piece of work.

A changed association changes no generated file, so nothing else will remind you. The Software Factory output is identical before and after, and the stub is already gone. Reading the model is your only check.

## Eventing

- The message bus (find its actual interface by searching the application, usually `IMessageBus`) buffers `Publish` and `Send` calls. The Mediator pipeline flushes them after the unit of work completes.
- NEVER call a flush or dispatch method on the bus from a handler. Only call `Publish` or `Send`.
- Do not add transactional or outbox code to a handler. The pipeline owns it.

## Saving

- A unit-of-work behaviour saves changes after the handler returns. Do not call `SaveChangesAsync` just to persist.
- Call `SaveChangesAsync` only when the command returns a payload that includes a database-generated key (for example an `Id` assigned on save) and the handler must return it. Save once, before reading the key.

## Workflow

1. Inspect the existing handler, command, response type, validator, repository interfaces, and related domain types.
2. Read the model as described above and list the interactions in order.
3. Search for code usages of:
  - similar command handlers
  - repository interfaces and existing repository methods
  - domain operations on the target aggregate or entity
  - result and error patterns used in the solution
4. Write the `Handle` body, one interaction at a time, in model order.
5. If the handler needs missing DAL capabilities, extend the relevant repository abstraction in an allowed layer instead of pulling infrastructure into the handler.
6. Add focused private helper methods when the flow becomes easier to read.
7. Thread the cancellation token through every async call.
8. Set `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on `Handle`, and make sure a void command ends with `return Unit.Value;`.
9. Verify the final code preserves layer boundaries and does not introduce infrastructure leakage.

## Command-specific guidance

Commands change state. Prefer patterns like these when supported by the surrounding codebase:

- load the aggregate or entity through a repository
- validate existence and ownership/eligibility rules
- invoke domain behavior rather than mutating state ad hoc
- persist through the repository abstraction
- return the established command response type used by the solution
- let entities raise domain events through their own behavior; do not dispatch domain events by hand

## Repository extension guidance

When a needed repository capability is missing:

- Extend the existing repository abstraction closest to the aggregate or read/write concern.
- Keep the new method shaped around domain intent, not storage mechanics.
- Prefer expressive methods such as `FindByIdAsync`, `ExistsBy...Async`, or `FindActive...Async` over storage-oriented names.
- Do not add infrastructure comments or instructions to the handler.
- Do not reference how the repository will be implemented in EF, Dapper, SQL, or similar.

## EF Related Data Loading guidance

- NEVER use `Include` or `ThenInclude` in the Application Layer, these are only available in the Infrastructure layer.
- Lazy loading with proxies is enabled. 
- Entities are configured using the `Owns` apis, so compsitional children will be automatically loaded with their parents.
- You can rely on navigation properties being automatically loaded when accessed.
- (CRITICAL) If your implementation will cause a lot of Lazy loading consider other alternatives, like moving the data loading into the repository layer.

## Unit of Work guidance

- SaveChanges rule (STRICT): Do not call UnitOfWork.SaveChangesAsync(...) / SaveChangesAsync(...) in a handler/service method unless the operation returns a payload that requires DB-generated values, such as a generated Id, surrogate key, RowVersion/concurrency token, DB-generated timestamp, or computed column.
- If the operation returns Unit, void, Task, or IRequest with no result: do not call SaveChangesAsync.
- If the operation returns an identifier or DTO that needs generated fields: call SaveChangesAsync before returning.
- If unsure, omit SaveChangesAsync and assume an outer unit-of-work/pipeline commit.
- When reviewing code, remove SaveChangesAsync unless there is a clear generated-value or immediate-commit requirement.

## Entity Framework repository guidance

- Repository update rule (STRICT): Do not call repository.Update(...) / repo.Update(...) when using EF repositories.
- EF tracks loaded entities automatically. Modify the entity properties directly and let the Unit of Work persist the tracked changes.
- Only call Add/Create/Delete operations when inserting or removing entities.
- When reviewing code, remove unnecessary Update calls for entities loaded from an EF repository.

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

- implements every association modelled on the command, in model order
- fills in the `Handle` body and marks it `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`
- adds private helper methods in the handler if useful
- extends allowed-layer abstractions when needed for the handler to be correct
- preserves the existing feature style and naming
- keeps unrelated refactors out of scope

## Review checklist

Before finishing, check that:

- the command's associations were re-read and each one is implemented
- a void command returns `Unit.Value`
- every injected dependency belongs to the Domain or Application layers
- the handler imports no infrastructure package namespaces
- the handler never flushes the message bus, and calls `SaveChangesAsync` only when a generated key is returned
- repository changes, if any, are expressed as abstractions only
- the implementation follows existing code usage patterns rather than inventing a novel style without reason
- business language matches the surrounding domain model
- the code path is cancellation-aware and async-safe
