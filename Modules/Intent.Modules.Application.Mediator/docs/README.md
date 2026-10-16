# Intent.Application.Mediator

Generates a CQRS dispatch stack for an Intent-managed .NET application on the source-generated [Mediator](https://github.com/martinothamar/Mediator) library (`Mediator.Abstractions` and `Mediator.SourceGenerator`, both 3.0.2). It is an alternative to MediatR (which has moved to a commercial licence) and to Wolverine for applications that model Commands and Queries in the Services designer.

**This module works differently from the MediatR and Wolverine modules: the body of every handler is written by an AI agent, not generated.** You model commands, queries and what they do (create an entity, publish an event, invoke another command and so on), the module generates a handler stub, and an agent implements the stub from the model. The module generates the skills the agent follows and the designer rules that keep the agent aware of the model. See [AI-written handlers](#ai-written-handlers).

## What this module generates

| Output | Generated for | Notes |
|---|---|---|
| Command class | Each Command in the Services designer | Implements Mediator's `ICommand` or `ICommand<TResponse>`. Initialised through the constructor. Carries `[Authorize]` when the command has a Security stereotype (see [Authorization and Identity](#authorization-and-intentapplicationidentity)). |
| Query class | Each Query | Implements Mediator's `IQuery<TResponse>`. Same constructor and security treatment. |
| Command handler stub | Each Command | `ICommandHandler<TCommand, TResponse>` (or `ICommandHandler<TCommand>`), with a `ValueTask` returning `Handle` that throws `NotImplementedException`. A command with no response returns `ValueTask<Unit>`. |
| Query handler stub | Each Query | `IQueryHandler<TQuery, TResponse>` with a `ValueTask<TResponse>` `Handle` stub. |
| `MediatorConfiguration` | Once | `AddMediatorConfiguration`, registered into the Infrastructure service configuration. Calls `AddMediator` with transient lifetime, the Application assembly, the notification publisher and the pipeline (below). |
| `SequentialNotificationPublisher` | Once | Runs notification handlers one at a time, stops at the first failure and rethrows that handler's own exception (Mediator's built-in publisher keeps going and wraps failures in an `AggregateException`). |
| Pipeline behaviours | Once each, when applicable | Logging, UnhandledException, Performance, Authorization, MessageBusPublish and UnitOfWork. See [The pipeline](#the-pipeline). |
| `MessageBusFlushOwner` | When a message bus interface exists | Scoped. Lets the outermost unit of work own flushing the message bus, so a nested command cannot send messages before the outer work commits. |
| Skills | See [Skills](#skills) | `mediator-command-handler`, `mediator-query-handler` and, when the application has integration event handlers, `mediator-integration-event-handler`. |

The command and query classes and the handler signatures are fully generated and are never edited by hand. Everything inside `Handle` is yours (or the agent's).

NuGet packages added to the application: `Mediator.Abstractions` in the Application layer, and `Mediator.SourceGenerator` (private assets, analyzer-style) in the Infrastructure layer where `AddMediator` is called.

### Handler stub

```csharp
[IntentManaged(Mode.Merge, Signature = Mode.Fully)]
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    [IntentManaged(Mode.Merge)]
    public CreateOrderCommandHandler()
    {
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public async ValueTask<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // IntentInitialGen
        // TODO: Implement Handle (CreateOrderCommandHandler) functionality
        throw new NotImplementedException("Your implementation here...");
    }
}
```

The class and `Handle` signature stay in sync with the model on every Software Factory run. Once the body has been written and `Handle` is marked `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`, regeneration leaves the body alone.

## The pipeline

`MediatorConfiguration` sets `options.PipelineBehaviors` to a single literal array. The Mediator source generator reads that array at compile time, so the order is fixed and nothing else may insert, reorder or compute items. The behaviours, outermost first:

| # | Behaviour | Included when |
|---|---|---|
| 1 | `LoggingBehaviour` (a message pre-processor) | The **Enable logging behaviour** setting is on |
| 2 | `UnhandledExceptionBehaviour` | The **Enable unhandled exception behaviour** setting is on |
| 3 | `PerformanceBehaviour` | The **Enable performance behaviour** setting is on |
| 4 | `AuthorizationBehaviour` | `Intent.Application.Identity` is installed **and** at least one command or query has a Security stereotype |
| 5 | `MessageBusPublishBehaviour` | A message bus interface exists (see [Eventing](#eventing)). Flushes the bus after the handler returns, if it is the outermost owner. |
| 6 | `ValidationBehaviour` | `Intent.Application.Mediator.FluentValidation` is installed (it fills the `Application.Common.ValidationBehaviour` role) |
| 7 | `UnitOfWorkBehaviour` | The application persists through a unit of work. Constrained to Mediator's `IBaseCommand`, so queries are not wrapped. |

Each item is included only if that behaviour's own template will actually be generated, so the pipeline never references a class that does not exist.

Other modules that need to change the pipeline do so by **removing** an item, never by adding one. Each item carries metadata naming its behaviour, and the flush item is additionally tagged `eventbus-flush`. This is how the eventing modules take the flush out when a transactional outbox does the sending instead.

A pipeline can legitimately be empty (all opt-ins off, no Identity, no FluentValidation, no bus, no persistence). The module is written on the understanding that the Mediator source generator accepts an empty `PipelineBehaviors` array.

## Settings

All three are on the **Mediator Settings** group in the application settings, and all three are off by default.

| Setting | Default | Effect |
|---|---|---|
| Enable logging behaviour | Off | Logs every command and query, with the current user, before it is handled. Logs without a user when `Intent.Application.Identity` is not installed. |
| Enable performance behaviour | Off | Logs a warning when a command or query takes longer than 500 ms to handle. Logs without a user when Identity is not installed. |
| Enable unhandled exception behaviour | Off | Logs unhandled exceptions from commands and queries (validation exceptions are rethrown unlogged when FluentValidation is installed). |

When any of the three is on, the module adds a `CqrsSettings:LogRequestPayload` entry to `appsettings.json` (value `true`). The behaviours read it to decide whether to include the request payload in their log messages.

Settings that belong to the companion modules (for example `Intent.Application.Mediator.DomainEvents`' "Create implicit domain event handlers", off by default) are documented with those modules.

## AI-written handlers

The Software Factory generates stubs; it never turns modelled interactions into code. This module does not use `Intent.Application.DomainInteractions` or `Intent.Application.CQRS.CRUD`, and you do not model field-to-attribute mappings.

### What you model

In the Services designer, model the command or query and the interactions it performs as associations: Create Entity, Update Entity, Delete Entity, Query Entity, Invoke (another command, query or service operation), Publish Integration Event and Send Integration Command. **The associations, in their designer order, are the specification for the handler body.** Where the designer insists on a mapping on an association, model only the minimum it needs; the module and the agent do not read mapping content. The agent works out which command field populates which entity attribute or message property by name and meaning.

### What the Software Factory does

1. Generates the command or query and its handler stub (above).
2. On later runs, keeps the signature in sync and leaves the body alone.

No AI task is raised. An agent finds the work through the designer rules and the skills below: a handler still needs writing while its body holds the `// IntentInitialGen` marker and throws `NotImplementedException`.

### Skills

The module generates these skills into the application's AI context.

| Skill | Purpose |
|---|---|
| `mediator-command-handler` | How to implement a command handler: read the associations from the designer in order, what each interaction type means, `return Unit.Value;` for commands with no response, no flushing the message bus from a handler, when `SaveChangesAsync` is and is not needed, never change the signature, and mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`. |
| `mediator-query-handler` | The same for queries: read-only handling, no saving. |
| `mediator-integration-event-handler` | Generated only when the application has integration event handlers (from `Intent.Eventing.Contracts`). See [Integration event handlers](#integration-event-handlers). |

Each handler skill ends with an "Output expectations" section so that guidance contributed by other modules (for example EF Core Repositories, AutoMapper and Mapperly) is injected there.

### Designer rules and model drift

Adding or changing an association on an already-implemented command does not change any generated file, so nothing in the Software Factory output flags it. To cover this, the module adds an AI rule to the **Command**, **Query** and **Integration Event Handler** elements in the Services designer. It tells any agent editing that element that the handler is AI-written, and that when the element is created, or an association is added, removed, reordered or changed, the handler body must be written or revisited in the same piece of work using the matching skill. The handler skills carry the same rule from the other side: re-read the associations whenever you implement or revisit a handler.

No AI task is raised for a new or outdated handler. The limits of this approach:

- A developer editing the model by hand, without an agent, sees no prompt. **Rule for humans: whenever you change the associations on a command or query, update its handler.**
- An agent that edits the model without reading the element's rules will not know.

### Integration event handlers

Integration event handlers are generated by `Intent.Eventing.Contracts`, not by this module, and that module is unchanged. It writes a `HandleAsync` body only for the actions it understands, and then marks it fully generated. Three kinds of handler therefore need an agent to finish them:

- **Invoke only.** The body is a stub, because nothing is generated for an Invoke.
- **Mixed.** The body holds a generated Publish or Send, and an Invoke that the generator silently skipped.
- **Unmapped.** A Publish or Send with no field mappings, generated as a `new` message with an empty object initialiser. It compiles but sends an empty message.

The designer rule on **Integration Event Handler** tells an agent editing one to check it, and the `mediator-integration-event-handler` skill describes how to find handlers that need finishing (adding an Invoke changes no generated output, so the file alone does not show it). The skill tells the agent to take the whole body over with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`, send an Invoke target through `Mediator.ISender`, and populate every outgoing message from the incoming one.

After that takeover, Publish and Send changes on that handler no longer regenerate. When you change its associations, the designer rule tells the agent to revisit it.

### What the agent is told not to do

Handlers inject only Domain and Application layer dependencies, never infrastructure types; data access goes through repositories; Publish and Send only buffer messages on the bus (the pipeline flushes them), so the handler never flushes; and the handler does not call `SaveChangesAsync` just to persist (the unit of work does).

## Authorization and `Intent.Application.Identity`

This module has no hard dependency on `Intent.Application.Identity`.

- **Authorization.** A command or query with a Security stereotype gets `[Authorize]` and the `AuthorizationBehaviour` runs for it. If a command or query is secured in an application that does not have Identity, the Software Factory stops with an error on that element telling you to install `Intent.Application.Identity` or remove the stereotype. Authorization is never silently left off.
- **Logging and Performance** degrade gracefully: they log without the current user when Identity is not installed.

## How it fits with other modules

### Satellite modules

These modules plug into the core and are installed automatically when the module they extend is present (via this module's interoperability entries):

| When this is installed | This module is installed | What it adds |
|---|---|---|
| [Intent.AspNetCore.Controllers](https://docs.intentarchitect.com/articles/modules-dotnet/intent-aspnetcore-controllers/intent-aspnetcore-controllers.html) | `Intent.AspNetCore.Controllers.Dispatch.Mediator` | Controller actions that call `await _sender.Send(...)` on `Mediator.ISender`. |
| [Intent.FastEndpoints](https://docs.intentarchitect.com/articles/modules-dotnet/intent-fastendpoints/intent-fastendpoints.html) | `Intent.FastEndpoints.Dispatch.Mediator` | Endpoints that dispatch through `Mediator.ISender`. |
| [Intent.Application.FluentValidation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-fluentvalidation/intent-application-fluentvalidation.html) | `Intent.Application.Mediator.FluentValidation` | Validators for commands and queries, and the validation behaviour the core places in the pipeline. |
| [Intent.DomainEvents](https://docs.intentarchitect.com/articles/modules-dotnet/intent-domainevents/intent-domainevents.html) | `Intent.Application.Mediator.DomainEvents` | Domain-event notifications and handlers. Handlers are AI-written too. |

### Eventing

The message bus and event bus interfaces come from [Intent.Eventing.Contracts](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-contracts/intent-eventing-contracts.html). When one of them exists, the core generates `MessageBusPublishBehaviour` and `MessageBusFlushOwner`, so messages published from a handler are buffered and flushed after the unit of work, once, by the outermost dispatch.

The eventing transports each carry a small Mediator seam in their own module. Under a transactional outbox they remove the flush item from the pipeline (the outbox does the sending), and the MassTransit consumer and the Wolverine integration-event middleware claim `MessageBusFlushOwner` so that a command invoked from an integration handler does not flush before the handler's transaction commits. The seam is in [Intent.Eventing.MassTransit](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-masstransit/intent-eventing-masstransit.html), [Intent.Eventing.NServiceBus](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-nservicebus/intent-eventing-nservicebus.html) and [Intent.Eventing.Wolverine](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-wolverine/intent-eventing-wolverine.html).

Eventing.Wolverine also removes the flush item under its durable outbox, and when no Wolverine message bus is registered (a subscribe-only application), so commands do not fail to resolve a bus that is not there.

### Persistence and the unit of work

`UnitOfWorkBehaviour` is generated from `Intent.Common.UnitOfWork`, and only when the application persists through a unit of work. With [Intent.EntityFrameworkCore](https://docs.intentarchitect.com/articles/modules-dotnet/intent-entityframeworkcore/intent-entityframeworkcore.html), the EF Core module adds its existing-transaction guard to the Mediator unit-of-work behaviour, so work nested inside an existing database transaction (for example under a MassTransit EF outbox) does not open a second transaction scope.

Guidance for the agent about repositories and mapping comes from [Intent.EntityFrameworkCore.Repositories](https://docs.intentarchitect.com/articles/modules-dotnet/intent-entityframeworkcore-repositories/intent-entityframeworkcore-repositories.html), [Intent.Application.AutoMapper](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-automapper/intent-application-automapper.html) and [Intent.Application.Dtos.Mapperly](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-dtos-mapperly/intent-application-dtos-mapperly.html).

### Other roles this module fills

`Application.Command`, `Application.Query`, `Application.Command.Handler` and `Application.Query.Handler`, which other modules (dispatchers, UnitTesting) use as type sources. It deliberately does **not** fill `Application.Contract.Command` / `Application.Contract.Query` or the marker-interface roles. Modules that depend on those fail at generation time instead of emitting MediatR code.

## Moving an existing MediatR application to Mediator

MediatR's CRUD module marks every handler body it fills as fully generated. If you simply uninstall MediatR and install this module, the Mediator handler templates replace those bodies with stubs. Protect the handlers first.

1. **Before uninstalling MediatR, protect every handler:**
   - **Command and query handlers:** mark the `Handle` body `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`. The method still matches the Mediator one (same name, same parameter), so the Software Factory rewrites the signature to return `ValueTask` and keeps the body.
   - **Domain-event handlers:** mark the whole method `[IntentManaged(Mode.Ignore)]`. Their parameter type changes (`DomainEventNotification<X>` becomes `XDomainEventNotification`), so a body-only Ignore no longer matches and the method would be replaced by a stub. With the whole method ignored, the old method survives next to the new stub; move the body across and delete the old method.
   - **Fields and constructors of every handler:** mark them `[IntentManaged(Mode.Ignore)]` too. MediatR's CRUD module injected dependencies (`_orderRepository`, `_messageBus` and so on); the Mediator stub's constructor takes nothing, and on a merge-managed class the Software Factory removes members it previously generated, so the bodies would survive but no longer compile.
2. **Swap the modules and run the Software Factory.** It rewrites the signatures itself, because they are fully generated.
3. **Fix the bodies:** void command handlers must end with `return Unit.Value;`, and any leftover `using MediatR` lines must go.

There is no automatic migration. The path above is not yet confirmed by running it against a migrated application.
