---
name: mediator-domain-event-handler
description: Implements the whole body of a Mediator domain event handler from the interactions modelled under each handled domain event. MUST load before creating, editing or revisiting any domain event handler e.g. *DomainEventHandler.cs or *Handler.cs implementing INotificationHandler, and after any Software Factory run that generates a new domain event handler stub.
template-id: Intent.Application.Mediator.DomainEvents.DomainEventHandlerSkill
contentHash: 4E1559E4D5842B7E7142D727F14955B3BA7CE326E4C972BB3F102F88D1102B29
---
# Mediator Domain Event Handler

Write the `Handle` bodies of a domain event handler. The generator only emits stubs that throw `NotImplementedException`; the interactions modelled under each handled domain event in the Services designer are the specification.

## Finding the handlers to write

- After every Software Factory run, search the Application project for `INotificationHandler` classes whose `Handle` still throws `NotImplementedException`, and write every one.
- When you change the associations under a handled domain event, or the events a handler handles, update the handler in the same piece of work. No generated file changes, so nothing else will remind you.
- A default handler, generated for a domain event with no modelled handler when "Create implicit domain event handlers" is on, has nothing modelled. Infer its behaviour from the event and nearby code, and ask the user when no concrete side effect can be inferred.

## Rules

- Mark each `Handle` you write with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`. Never change its name, return type, parameters or notification type, and keep it `async` even when it awaits nothing.
- The handler implements `INotificationHandler<TNotification>` from the `Mediator` library once per handled event, where `TNotification` is the generated `<DomainEvent>Notification`. Each `Handle` returns `ValueTask`; read the event from `notification.DomainEvent`.
- Never edit `DomainEventNotification`, the `<DomainEvent>Notification` classes or `DomainEventNotificationFactory`. They are generated.
- The event has already happened: react to it, and do not move the original command's decision into the handler. Several handlers can handle one event, in no guaranteed order, and one that throws stops the rest and fails the save.
- Change only what the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the handler, the domain events it handles, the repository interfaces and the domain types it touches.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Let the domain model drive the code: use its aggregates, value objects, statuses and business terms rather than names of your own.
- Load aggregates through their repository interface and change them through their domain behaviour. Put durable business rules on the entity or a domain service when nearby code does that. Never publish a domain event by hand.
- Look for similar handlers, repository methods and domain operations before writing, and follow the nearest one. Don't invent a new approach where one exists.
- A handler can run again when the work that raised the event is retried. Before adding a durable side effect, follow the nearest handler's de-duplication (natural keys, processed-event checks, `AddIfNotExistsAsync`), never an in-memory one, and its handling of stale events or missing data.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Publish or send messages only through the message bus (usually `IMessageBus`). The pipeline flushes it after the unit of work commits, so never flush it yourself and add no transaction or outbox code.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.
- Before finishing, check that every association under each handled event is implemented, and review your code against every rule in this skill, the sections below included.

## Read the model first

Load the `exploring-the-model` skill and call `run_designer_query` with `getElementDetails` on the domain event handler. For each handled event, implement the associations under it in the order the designer shows them, in that event's `Handle`; later steps may use what earlier ones loaded or created. An event with a comment and no interactions: implement the comment's intent. Re-read the associations whenever you revisit the handler.

Field mappings are not modelled. Ignore any minimal mapping or missing-mapping warning on an association, and add none: decide which event data fills which attribute, constructor argument, command field or message property by name and meaning. When a name or meaning is ambiguous, check a similar handler before asking.

- Create Entity: construct the entity through its constructor or factory with the matching event data, and add it through its repository.
- Update Entity or Delete Entity: load it by the identity on the event. When it is missing, follow the solution's not-found pattern, or return quietly where stale events are ignored elsewhere. Then apply the change through domain methods, or remove it through the repository.
- Query Entity: load the matching instances through the repository for the steps that follow.
- Call Service Operation (Invoke): call the operation through its Application interface. When it targets a command or query, dispatch it with `Mediator.ISender`; the `mediator-command-handler` skill covers that command's own handler.
- Publish Integration Event or Send Integration Command: populate every property of the message from the event and earlier results, then publish or send it through the message bus.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.
- Do not call `SaveChangesAsync`: the handler runs inside the save that raised the event, and that save writes your changes too. Remove any such call you find in a handler you review.
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
