---
name: mediator-command-handler
description: Implements the whole body of a Mediator command handler from the interactions modelled on the command. MUST load before creating, editing or revisiting any command handler e.g. *CommandHandler.cs, and after any Software Factory run that generates a new command handler stub.
template-id: Intent.Application.Mediator.CommandHandlerSkill
contentHash: 0321D56B0619392786657A8D1592C0F84F685434953B504F63563169DD0113B9
---
# Mediator Command Handler

Write the `Handle` body of a command handler. The generator only emits a stub that throws `NotImplementedException`; the associations modelled on the command in the Services designer are the specification.

## Finding the handlers to write

- After every Software Factory run, search the Application project for `*CommandHandler.cs` files whose `Handle` still throws `NotImplementedException`, and write every one.
- When you add, remove, reorder or change an association on a command, update its handler in the same piece of work. No generated file changes, so nothing else will remind you.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body. Never change its name, return type or parameters, and keep it `async` even when it awaits nothing.
- The handler implements `ICommandHandler<TCommand, TResponse>` from the `Mediator` library and returns `ValueTask<TResponse>`. A command with no response returns `ValueTask<Unit>` and ends every normal path with `return Unit.Value;`.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Let entities raise domain events themselves; never dispatch one by hand.
- Follow the nearest similar handler for result types, error handling and the not-found pattern.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Publish or send messages only through the message bus (usually `IMessageBus`). The pipeline flushes it after the unit of work commits, so never flush it yourself and add no transaction or outbox code.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.

## Read the model first

Load the `exploring-the-model` skill and call `run_designer_query` with `getElementDetails` on the command. Implement its associations in the order the designer shows them; later steps may use what earlier ones loaded or created. Re-read them whenever you revisit the handler.

Field mappings are not modelled. Ignore any minimal mapping or missing-mapping warning on an association, and add none: decide which command field fills which attribute, constructor argument or message property by name and meaning.

- Create Entity: construct the entity through its constructor or factory with the matching command fields, and add it through its repository.
- Update Entity: load it by the identity on the command, fail with the not-found pattern when it is missing, then apply the command's fields, through domain methods where they exist.
- Delete Entity: load it by the identity on the command, fail with the not-found pattern when it is missing, then remove it through the repository.
- Query Entity: load the matching instances through the repository for the steps that follow.
- Call Service Operation (Invoke): call the operation through its Application interface. When it targets a command or query, dispatch it with `Mediator.ISender`.
- Publish Integration Event or Send Integration Command: populate every property of the message from the command and earlier results, then publish or send it through the message bus.

## Validator

Every command has a generated validator (`*CommandValidator.cs`), and its rules are also yours to write. The generator emits only the rules modelled explicitly on the command's fields, plus `NotNull`, enum and nested DTO checks. It does not reliably derive rules from the entities the command touches, so write them when you write the handler.

- Load the `fluent-validation-custom-validation` skill before editing the validator, and follow its rules.
- Write the rules in `ConfigureValidationRules`. Derive them from the entity the command creates or updates, found through the Create Entity and Update Entity associations, and match each command field to an entity attribute by name and meaning, as for the handler.
- Take each rule from evidence on that entity: required attributes, maximum lengths, numeric ranges, formats such as email or regular expressions, and unique indexes. The evidence is the entity itself, its Domain Constraints stereotypes, and its persistence configuration (for example `IsRequired`, `HasMaxLength` or a unique `HasIndex`). Do not invent rules the entity does not carry.
- Check uniqueness with `MustAsync` through a repository abstraction, e.g. an `ExistsBy...Async` method, extending the repository if the method is missing. For an Update Entity command, exclude the entity being updated from the check.
- Keep the rules that are already generated, and do not add a second rule for a field that one of them already covers.
- ALWAYS, when you add rules, put `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on `ConfigureValidationRules` so regeneration keeps them. From then on the generator no longer updates that method, so when you revisit the command, also re-read the validation stereotypes on its fields and bring the rules in line.
- A command that only deletes or calls other operations usually needs no rules beyond the generated ones. Leave its validator as generated.
- Before finishing, check that every rule in the validator is traceable to the target entity or a field stereotype.

## Persistence (EF Core)

- `Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method.
- Do not call `SaveChangesAsync`: the pipeline's unit of work commits after the handler. Call it only when the handler returns a value the database generates, such as a new Id.
- Do not call the repository's `Update`. EF tracks the entities it loaded, so changing their properties is enough. Call `Add` or `Remove` only to insert or delete.

## Mapping (AutoMapper)

- Return entity-shaped `*Dto` types through AutoMapper (`Map` or `ProjectTo`), not `new XxxDto { … }`. Before you use one, find its `CreateMap<TEntity, TDto>()` in a `Profile`. If there is none, add the profile, nested mappings included, as `Mappings/<Aggregate>/<Entity>DtoProfile.cs` in the Application project. Profiles are found by assembly scanning, so leave the AutoMapper registration alone.
- Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment.
