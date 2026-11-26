---
name: mediator-domain-event-handler
description: Implements the whole body of a Mediator domain event handler from the interactions modelled under each handled domain event. MUST load before creating, editing or revisiting any domain event handler e.g. *DomainEventHandler.cs or *Handler.cs implementing INotificationHandler, and after any Software Factory run that generates a new domain event handler stub.
template-id: Intent.Application.Mediator.DomainEvents.DomainEventHandlerSkill
contentHash: 85C7D5BF24891DACD281F3D240297FC35A2F40875199D0B558FCFD73ECC5C476
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
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Load aggregates through their repository interface and change them through their domain behaviour. Never publish a domain event by hand.
- A handler can run again when the work that raised the event is retried. Before adding a durable side effect, follow the nearest handler's de-duplication (natural keys, processed-event checks, `AddIfNotExistsAsync`), never an in-memory one, and its handling of stale events or missing data.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Publish or send messages only through the message bus (usually `IMessageBus`). The pipeline flushes it after the unit of work commits, so never flush it yourself and add no transaction or outbox code.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.

## Read the model first

Load the `exploring-the-model` skill and call `run_designer_query` with `getElementDetails` on the domain event handler. For each handled event, implement the associations under it in the order the designer shows them, in that event's `Handle`; later steps may use what earlier ones loaded or created. An event with a comment and no interactions: implement the comment's intent. Re-read the associations whenever you revisit the handler.

Field mappings are not modelled. Ignore any minimal mapping or missing-mapping warning on an association, and add none: decide which event data fills which attribute, constructor argument, command field or message property by name and meaning.

- Create Entity: construct the entity through its constructor or factory with the matching event data, and add it through its repository.
- Update Entity or Delete Entity: load it by the identity on the event. When it is missing, follow the solution's not-found pattern, or return quietly where stale events are ignored elsewhere. Then apply the change through domain methods, or remove it through the repository.
- Query Entity: load the matching instances through the repository for the steps that follow.
- Call Service Operation (Invoke): call the operation through its Application interface. When it targets a command or query, dispatch it with `Mediator.ISender`; the `mediator-command-handler` skill covers that command's own handler.
- Publish Integration Event or Send Integration Command: populate every property of the message from the event and earlier results, then publish or send it through the message bus.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.
- Do not call `SaveChangesAsync`: the handler runs inside the unit of work that raised the event.
- Do not call the repository's `Update`. EF tracks the entities it loaded, so changing their properties is enough. Call `Add` or `Remove` only to insert or delete.

## Mapping (AutoMapper)

- Return entity-shaped `*Dto` types through AutoMapper (`Map` or `ProjectTo`), not `new XxxDto { … }`. Before you use one, find its `CreateMap<TEntity, TDto>()` in a `Profile`. If there is none, add the profile, nested mappings included, as `Mappings/<Aggregate>/<Entity>DtoProfile.cs` in the Application project. Profiles are found by assembly scanning, so leave the AutoMapper registration alone.
- Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment.
