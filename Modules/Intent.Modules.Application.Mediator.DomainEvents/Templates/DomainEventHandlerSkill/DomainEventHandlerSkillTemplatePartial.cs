using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Application.Mediator.Templates.CommandHandlerSkill;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventHandlerSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DomainEventHandlerSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Mediator.DomainEvents.DomainEventHandlerSkill";

        public const string SkillName = "mediator-domain-event-handler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventHandlerSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: SkillName)
                .FromMarkdown($$"""
---
name: {{SkillName}}
description: Implements the whole body of a Mediator domain event handler from the interactions modelled under each handled domain event. MUST load before creating, editing or revisiting any domain event handler e.g. *DomainEventHandler.cs or *Handler.cs implementing INotificationHandler, and after any Software Factory run that generates a new domain event handler stub.
template-id: {{TemplateId}}
---

# Mediator Domain Event Handler

Write the business logic of a domain event handler. In this solution the handler body is always written by you, never generated: the generator emits only a stub that throws `NotImplementedException`. The interactions modelled under each handled domain event in the Services designer are the specification. Favor domain intent, existing code patterns, and clean layer boundaries over convenience shortcuts.

## Finding the handlers to write

Nothing raises a task or a reminder for a new or outdated handler. Find the work yourself:

- After every Software Factory run, search the Application project for domain event handlers (classes implementing `INotificationHandler`) whose `Handle` body still holds the `// IntentInitialGen` marker and `throw new NotImplementedException`. Each one is a handler nobody has written yet. Write every one you find.
- When you add, remove, reorder or change an association under a handled domain event on a Domain Event Handler in the designer, that handler is out of date even though no generated file changed. Update it in the same piece of work.
- A default handler (generated per domain event with no modelled handler, when the "Create implicit domain event handlers" setting is on) has nothing modelled. Infer its behaviour from the event's name and data and from nearby code. When no concrete side effect can be inferred, ask the user instead of inventing one.

## Core rules

- Treat the existing handler file as the starting point. Write the `Handle` method bodies rather than rewriting unrelated parts of the file.
- The handler implements `INotificationHandler<TNotification>` from the `Mediator` library, once per handled domain event. `TNotification` is the generated `<DomainEvent>Notification` class (for example `OrderPlacedDomainEventNotification`). Each `Handle` returns `ValueTask`.
- The domain event is `notification.DomainEvent`. Read its data from there, for example `var domainEvent = notification.DomainEvent;`.
- NEVER change the name, return type or parameters of `Handle`, and never change which notification type it accepts. They are generated. The stub is always generated `async`. Keep `async` even when the body awaits nothing: removing it is a signature change, which the next Software Factory run reverts and reports as destructive.
- NEVER edit the `DomainEventNotification` base class, the `<DomainEvent>Notification` classes or `DomainEventNotificationFactory`. They are generated from the domain events.
- ALWAYS, when you write a `Handle` body, put `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on the `Handle` method so regeneration keeps your body.
- Several handlers can react to the same event. Do not assume yours is the only one, and do not assume an order between handlers.
- Inject only dependencies from the Domain or Application layers.
- Never introduce direct dependencies on infrastructure packages or infrastructure types in the handler, including Entity Framework, Dapper, concrete DbContexts, SQL clients, or vendor-specific data access abstractions.
- If data access is required, favor using an existing repository abstraction from the Domain or Application layers, and extend it if required.
- ALWAYS if the work requires any aggregation (Count/Sum/Average/Min/Max/First/Last/GroupBy), the handler MUST NOT compute it in memory. The handler MUST call a Domain/Application abstraction that returns an aggregated result/summary/shaped dataset. If no suitable method exists, extend the appropriate repository/read abstraction (contract first, then implementation) and then call it. No temporary in-handler aggregation is allowed.
- Follow the modeled domain language already present in the codebase: aggregates, entities, value objects, policies, invariants, statuses, and business terms should drive the implementation.
- Search the codebase for similar handlers, repository methods, domain services, and result patterns before introducing a new approach.
- Add private helper methods inside the handler when they improve clarity or encapsulate repeated branching logic.
- Keep orchestration in the handler and place durable business rules in domain entities, value objects, specifications, or domain/application services when those patterns already exist nearby.

## How domain events reach the handler

1. An entity raises a past-tense domain event through its own behavior.
2. When the unit of work saves, the infrastructure publishes each pending event through `DomainEventService`.
3. `DomainEventNotificationFactory` wraps the event in its `<DomainEvent>Notification`, and Mediator publishes that notification to every `INotificationHandler` for it, one handler at a time.
4. If a handler throws, the remaining handlers do not run and the exception reaches the code that saved. A domain event with no handler is simply ignored.

The handler reacts to something that already happened. Do not move the original command's decision into the handler.

## Read the model first

The interactions drawn from the handled domain event to other elements are the specification. Read them before you write anything.

1. Load the `exploring-the-model` skill.
2. Call `run_designer_query` with `getElementDetails` on the domain event handler in the Services designer.
3. For each domain event the handler handles, list every association under it in the order the designer shows them. That order is the order the work happens in inside that event's `Handle` method.
4. Implement each association in that order. Later steps may use the entity loaded or created by an earlier one.

A handler may handle several domain events. Each has its own `Handle` method and its own list of interactions. A domain event with a comment and no interactions still tells you what the handler is for: implement the intent of the comment.

Field mappings are NOT modelled. The designer may show a minimal mapping on an association because it insists on one, or warn that an association has no mappings. Neither carries intent: leave the warning, and do not add mappings to silence it, because the generator ignores them. Decide which domain event data populates which entity attribute, constructor argument, command field or message property by name and meaning, using the domain event's properties and the target's attributes and constructors. When a name or meaning is genuinely ambiguous, search the codebase for a similar handler before asking.

### What each interaction means

- Create Entity: create a new instance of the target entity and add it through its repository. Use the entity's constructor or factory, and pass the domain event data that matches the entity's attributes by name and meaning. Do not leave required attributes unset.
- Update Entity: load the target entity from its repository using the identity carried by the domain event, fail with the not-found pattern the solution already uses when it is missing (or return quietly if stale events are ignored elsewhere in the solution), then apply the changes. Prefer domain methods on the entity over ad hoc property assignment when they exist.
- Delete Entity: load the target entity by the identity on the domain event, handle a missing entity the same way, then remove it through the repository.
- Query Entity: load one or many instances of the target entity through its repository, filtering by the domain event data that matches its attributes. The result is available to the interactions that follow it.
- Call Service Operation (Invoke): call the modelled operation on the target service. Inject the service's interface from the Application layer and pass arguments derived from the domain event and from earlier results. When the Invoke targets a command or query, dispatch it with `Mediator.ISender`, e.g. `await _sender.Send(new OtherCommand(...), cancellationToken)`. The `{{CommandHandlerSkillTemplate.SkillName}}` skill describes how the command's own handler is written.
- Publish Integration Event: publish the target integration event through the message bus. Populate every property of the message from the domain event and from entities handled earlier in the handler.
- Send Integration Command: send the target integration command through the message bus. Populate every property the same way.

## Keep the handler in step with the model

Whenever you implement OR revisit this handler for any reason, re-read the domain event handler's associations again with `getElementDetails`. If the model changed since the body was written (an association added, removed, reordered or retargeted, or a domain event added to or removed from the handler), update the body in the same piece of work.

A changed association changes no generated file, so nothing else will remind you. The Software Factory output is identical before and after, and the stub is already gone. Reading the model is your only check.

## Eventing

- The message bus (find its actual interface by searching the application, usually `IMessageBus`) buffers `Publish` and `Send` calls. The Mediator pipeline flushes them after the unit of work completes.
- NEVER call a flush or dispatch method on the bus from a handler. Only call `Publish` or `Send`.
- Do not add transactional or outbox code to a handler. The pipeline owns it.

## Saving

- The handler runs inside the unit of work that raised the event, while that work is saving. Do not call `SaveChangesAsync` from a handler.
- Do not publish domain events by hand. Let the entities raise them through their own behavior.

## Workflow

1. Inspect the existing handler, the domain events it handles, repository interfaces, and related domain types.
2. Read the model as described above and list the interactions for each handled event, in order.
3. Search for code usages of:
   - similar domain event handlers
   - repository interfaces and existing repository methods
   - domain operations on the target aggregate or entity
   - result and error patterns used in the solution
4. Write each `Handle` body, one interaction at a time, in model order.
5. If the handler needs missing DAL capabilities, extend the relevant repository abstraction in an allowed layer instead of pulling infrastructure into the handler.
6. Add focused private helper methods when the flow becomes easier to read.
7. Thread the cancellation token through every async call.
8. Set `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on each `Handle` you wrote.
9. Verify the final code preserves layer boundaries and does not introduce infrastructure leakage.

## Idempotency and error handling

- Check for retry assumptions before adding durable side effects. A handler can run again if the work that raised the event is retried.
- If creating records or messages, look for existing processed-event checks, natural keys, unique constraints, or methods like `AddIfNotExistsAsync`.
- Do not implement durable de-duplication in memory.
- Follow nearby handlers for stale events or missing data: silently return, throw the established domain/application exception, or log and return, whichever is the convention. Do not invent a new result type: `Handle` returns `ValueTask`.

## Repository extension guidance

When a needed repository capability is missing:

- Extend the existing repository abstraction closest to the aggregate or read/write concern.
- Keep the new method shaped around domain intent, not storage mechanics.
- Prefer expressive methods such as `FindByIdAsync`, `ExistsBy...Async`, or `FindActive...Async` over storage-oriented names.
- Do not add infrastructure comments or instructions to the handler.
- Do not reference how the repository will be implemented in EF, Dapper, SQL, or similar.

## Output expectations

Produce a concrete code update that:

- implements every interaction modelled under each handled domain event, in model order
- fills in each `Handle` body and marks it `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`
- adds private helper methods in the handler if useful
- extends allowed-layer abstractions when needed for the handler to be correct
- preserves the existing feature style and naming
- keeps unrelated refactors out of scope

## Review checklist

Before finishing, check that:

- the handler's associations were re-read and each one is implemented, for every handled domain event
- every `Handle` still has its generated signature and returns `ValueTask`
- every injected dependency belongs to the Domain or Application layers
- the handler imports no infrastructure package namespaces
- the handler never flushes the message bus and never calls `SaveChangesAsync`
- repository changes, if any, are expressed as abstractions only
- the implementation follows existing code usage patterns rather than inventing a novel style without reason
- business language matches the surrounding domain model
- the code path is cancellation-aware and async-safe
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}