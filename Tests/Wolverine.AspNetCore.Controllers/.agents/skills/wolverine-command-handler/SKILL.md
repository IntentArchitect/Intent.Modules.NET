---
name: wolverine-command-handler
description: Implements business logic in Wolverine command handler Handle methods. MUST load before creating or editing any command handler e.g. *CommandHandler.cs.
template-id: Intent.Application.Wolverine.CommandHandlerSkillTemplate
contentHash: BB4E58C8C599DE5A4BA9FA2C8E009DF9E99C2CD887AA36B3BFF3D33C38811957
---
# Wolverine Command Handler

Fill in the `Handle` method of an existing command handler. Its signature is generated; only the body is yours.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- Wolverine handlers are plain classes found by naming convention. Never make one implement a handler interface such as `IRequestHandler`.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Put durable business rules on the entity or a domain service when nearby code does that.
- Follow the nearest similar handler for result types, error handling and events. Publish an event only where nearby code already does.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`GetForUpdateAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.
- Do not call `SaveChangesAsync`: the pipeline's unit of work commits after the handler. Call it only when the handler returns a value the database generates, such as a new Id.
- Do not call the repository's `Update`. EF tracks the entities it loaded, so changing their properties is enough. Call `Add` or `Remove` only to insert or delete.

## Mapping (AutoMapper)

- Return entity-shaped `*Dto` types through AutoMapper (`Map` or `ProjectTo`), not `new XxxDto { … }`. Before you use one, find its `CreateMap<TEntity, TDto>()` in a `Profile`. If there is none, add the profile, nested mappings included, as `Mappings/<Aggregate>/<Entity>DtoProfile.cs` in the Application project. Profiles are found by assembly scanning, so leave the AutoMapper registration alone.
- Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment.
