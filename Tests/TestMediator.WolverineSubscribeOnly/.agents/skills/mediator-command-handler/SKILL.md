---
name: mediator-command-handler
description: Implements the whole body of a Mediator command handler from the interactions modelled on the command. MUST load before creating, editing or revisiting any command handler e.g. *CommandHandler.cs, and after any Software Factory run that generates a new command handler stub.
template-id: Intent.Application.Mediator.CommandHandlerSkill
contentHash: 7657933292DECD5647DE1D76D0DA94B302F33689B518C391FA96B19B5EB85E8C
---
# Mediator Command Handler

Write the `Handle` body of a command handler. The generator only emits a stub that throws `NotImplementedException`; the associations modelled on the command in the Services designer are the specification.

## Finding the handlers to write

- After every Software Factory run, search the Application project for `*CommandHandler.cs` files whose `Handle` still throws `NotImplementedException`, and write every one.
- When you add, remove, reorder or change an association on a command, update its handler in the same piece of work. No generated file changes, so nothing else will remind you.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body. Never change its name, return type or parameters, and keep it `async` even when it awaits nothing.
- The handler implements `ICommandHandler<TCommand, TResponse>` from the `Mediator` library and returns `ValueTask<TResponse>`. A command with no response returns `ValueTask<Unit>` and ends every normal path with `return Unit.Value;`.
- Change only what the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the handler, its command, response and validator, the repository interfaces and the domain types it touches.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Let the domain model drive the code: use its aggregates, value objects, statuses and business terms rather than names of your own.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Put durable business rules on the entity or a domain service when nearby code does that. Let entities raise domain events themselves; never dispatch one by hand.
- Before changing state, check what nearby handlers check: that the entity exists, and that the caller may act on it (ownership, eligibility, status).
- Look for similar handlers, repository methods and domain operations before writing, and follow the nearest similar handler for result types, error handling and the not-found pattern. Don't invent a new result or exception style where one exists.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Publish or send messages only through the message bus (usually `IMessageBus`). The pipeline flushes it after the unit of work commits, so never flush it yourself and add no transaction or outbox code.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.
- Before finishing, check that every association modelled on the command is implemented and a command with no response returns `Unit.Value`, and review your code against every rule in this skill, the sections below included.

## Read the model first

Load the `exploring-the-model` skill and call `run_designer_query` with `getElementDetails` on the command. Implement its associations in the order the designer shows them; later steps may use what earlier ones loaded or created. Re-read them whenever you revisit the handler.

Field mappings are not modelled. Ignore any minimal mapping or missing-mapping warning on an association, and add none: decide which command field fills which attribute, constructor argument or message property by name and meaning. When a name or meaning is ambiguous, check a similar handler before asking.

- Create Entity: construct the entity through its constructor or factory with the matching command fields, and add it through its repository.
- Update Entity: load it by the identity on the command, fail with the not-found pattern when it is missing, then apply the command's fields, through domain methods where they exist.
- Delete Entity: load it by the identity on the command, fail with the not-found pattern when it is missing, then remove it through the repository.
- Query Entity: load the matching instances through the repository for the steps that follow.
- Call Service Operation (Invoke): call the operation through its Application interface. When it targets a command or query, dispatch it with `Mediator.ISender`.
- Publish Integration Event or Send Integration Command: populate every property of the message from the command and earlier results, then publish or send it through the message bus.

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
