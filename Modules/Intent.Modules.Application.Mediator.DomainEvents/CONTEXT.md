# Context: Intent.Application.Mediator.DomainEvents

## Modelling status

Fully modelled in the Module Builder designer (2026-10-16). Every template has its own File-Per-Model or Single-File
element, so a Software Factory run on this application matches the hand-written C#.

Decisions taken while modelling that the code does not carry:

- **Roles and locations.** `DomainEventHandler` is role `Application.DomainEventHandler.Explicit`, `DefaultDomainEventHandler`
  `Application.DomainEventHandler.Default` (the siblings' values; UnitTesting's `DomainEventHandlerTest` reads them), both
  at `EventHandlers`. The two notification templates had no role anywhere, so they got `Application.Common.TypedDomainEventNotification`
  and `Application.Common.DomainEventNotificationFactory`, both at `Common/Models` beside the base class.
- **`DomainEventNotificationFactory` is a Single File with the Domain Event as its model type.** The designer generates a
  parameterless `GetDomainEventNotificationFactoryName` for it, which is what the code uses.
- **The G22 rule is modelled, not hand-written.** A `designers` folder holds the Designer Settings element "Mediator Domain
  Events Services Extensions" (extends the Services designer) with one Element Extension typed `Domain Event Handler`
  carrying the rule in its AI Settings, the same shape as the core module's. Software Factory generates
  `modelers/*.designer.settings` and the `<metadata>` install entry from it. A hand-added `<metadata>` entry is **not**
  preserved: the first attempt was stripped on the next run, so do not author the file by hand.
- **To work on this module in Intent Architect, install `Intent.Modelers.Domain.Events` and `Intent.DomainEvents` into
  this application with `enableFactoryExtensions`, `installTemplateOutputs`, `installApplicationSettings` and
  `installDesignerMetadata` all off.** With the defaults, `Intent.DomainEvents` generates its own `DomainEvent.cs`,
  `IDomainEventService.cs`, `IHasDomainEvent.cs` and a `.sln` into this module's parent folder, and the modeler's plugin
  fails to load against an older Services modeler. The designer only needs them present for element-type references.
- **Verified end to end in `Tests/TestMediator.Publisher`**: the `Order` constructor raises `OrderPlacedDomainEvent`, its handler
  sends `CreateAuditEntryCommand`, and tests prove the nested commit does not flush (only the outermost dispatch does, once,
  last), and that a handler failing after the nested command rolls both rows back.

## Purpose

The domain-event satellite of `Intent.Application.Mediator`. It dispatches an application's domain events through the
source-generated Mediator library and generates the handlers that react to them. Like the core, it generates stubs
and a skill, and an AI agent writes every handler body from the modelled interactions; no code is generated
from interactions.

Design source: `intent/.plans/2026-10-13-mediator-dispatch-modules.mdx` (G# decisions from planning, D# from the spike
in `Spike/MediatorSpike/DECISIONS.md`).

## Architectural Decisions

- **Per-event sealed wrapper, abstract generic base, factory switch (D7).** `DomainEventNotification<TDomainEvent>` is
  abstract; each domain event gets `{Name}Notification`; `DomainEventNotificationFactory.Create` maps event to wrapper
  with a `switch`. Rejected, from the spike: a generic notification published directly, which is the MediatR stack's
  shape (spike probe P6: the Mediator source generator emits code that does not compile for the open generic); domain
  events implementing Mediator's `INotification` (works, probe P7, but makes the Domain layer depend on Mediator and
  changes handler bodies); a hand-built dispatcher (a mini-mediator to maintain). The generated factory carries a comment
  saying the same.
- **The factory is ordered, and an unmapped event throws.** Derived events come before their base events, by inheritance
  depth, then by name with an ordinal comparer so the output is stable between runs. Depth is capped at 50 so a cyclic
  model cannot loop. The `default` branch throws `InvalidOperationException` rather than dropping the event, so a missing
  case is noticed. Rejected: a silent no-op default.
- **Default handlers sit behind a setting that is off by default (G18).** `Intent.MediatR.DomainEvents` has the same
  setting but defaults to on. Here a default handler is a stub that throws, so an implicit handler for an event nobody
  asked to handle would throw on every publish until an agent implemented it. With the setting off, an event without a
  modelled handler simply has no handler, and publishing it is a no-op (spike probe P11). The set of "handled" events is
  computed in `DefaultDomainEventHandlerTemplateRegistration.GetModels` from every `DomainEventHandlerModel`'s handled
  events.
- **Every handler is an AI stub (G8).** Explicit and default handlers generate a `Handle` returning `ValueTask`, with an
  `IntentInitialGen` TODO and `throw new NotImplementedException`. The class is `Merge, Signature = Fully`, the
  constructor `Merge`, and `Handle` is `Fully` with `Body = Merge`; the AI sets `Body = Ignore` on each `Handle` it
  writes. These handlers never call `RegisterAsProcessingHandlerForModel`, and the module registers no interaction
  strategy, so nothing translates interactions to code. Rejected: porting `Intent.Application.DomainInteractions`, as
  `Intent.Application.Wolverine.DomainEvents` and `Intent.MediatR.DomainEvents` do.
- **No AI tasks; the skill and a designer rule carry the guidance.** AI task providers are deprecated (developer
  decision, 2026-10-14), so the planned "Implement Domain Event Handler" task (G16) and its `TemplateAITask` copy were
  removed. The skill's "Finding the handlers to write" section tells an agent to search for `// IntentInitialGen` plus
  `NotImplementedException` stubs after a run, and what to do with a default handler that has nothing modelled. The
  G22 `AI Settings` rule on Domain Event Handler (see "Modelling status") is the model-side carrier. Accepted cost: nothing proactively flags an unwritten handler.
- **The skill is generated by a File Template with role `AI.Context.Skills.Handler`**, not the plain `AI.Context.Skills`
  that the Wolverine and MediatR domain-event skills use. It is the same role as the core's command and query handler
  skills, and the skill has an "Output expectations" section. The reason is the plan's note that the handler skills get
  EF Core, Repositories and mapper guidance injected into that section (G7 table). Whether the injection actually reaches
  this skill is not confirmed.
- **The skill refers to the core's command-handler skill by constant.** `DomainEventHandlerSkillTemplate` interpolates
  `CommandHandlerSkillTemplate.SkillName` from the core project, for the sentence telling the agent how to write the
  command a handler Invokes. That is the only use of the core project in this module.
  - Judged: keep the project reference. A `public const string` is compiled in as a literal, so nothing from the core
    assembly is loaded at runtime, and the core `.dll` is not packaged here (`PrivateAssets=All`, and the `.imodspec`
    ships only this module's own `.dll`). The cost is build coupling only. The benefit is that renaming the core's skill
    breaks this build instead of leaving an instruction that points at a skill that no longer exists, which an agent
    would not report. It is also the repo's established satellite convention, and the module already declares the core as
    an `.imodspec` dependency, so the coupling is already real. Rejected: a duplicated string literal, which drifts
    silently.
  - Written against the core at `1.0.0-pre.0`. `SkillName` must stay a `const`; a `static readonly` would not be inlined
    and would need the core assembly at runtime.
- **`DomainEventService` is registered through `ContainerRegistrationRequest`,** for the interface under role
  `Application.Common.DomainEventServiceInterface`, concern `Infrastructure`, per-service-call (scoped) lifetime. It
  logs, then `await _mediator.Publish(factory.Create(domainEvent), cancellationToken)` using Mediator's `IPublisher`.
  Scoped, not singleton, so it shares the unit-of-work scope with its handlers.

## Invariants & Constraints

- **The `TemplateId` constants must keep matching the designer-derived ids.** Types are resolved by id:
  `GetTypeName(TypedDomainEventNotificationTemplate.TemplateId, domainEvent)` in the factory and handlers,
  `GetTypeName(DomainEventNotificationFactoryTemplate.TemplateId)` in the service. A mismatch does not raise an error: the
  template simply stops participating. Rename or move a designer element only together with its constant's consumers.
- **Role strings used by the code are provided by `Intent.DomainEvents`,** not by this module: `Domain.Events`,
  `Domain.Common.EventBase` (the constraint on `TDomainEvent`) and `Application.Common.DomainEventServiceInterface`.
  Without that module installed the types do not resolve.
- **NuGet dependencies are declared in template constructors**, never in `OnBuild`. `NugetPackages` locks
  `Mediator.Abstractions` at 3.0.2, matching the core.
- **Handlers must not flush the bus or call `SaveChangesAsync`, and must not publish domain events by hand.** This is
  guidance carried by the skill and the designer rule, not enforced by generated code.
- **Do not add a dependency on `Intent.Application.DomainInteractions`** (plan decision for this module).

## Module Interactions

- **Intent.Application.Mediator (core)** — owns the Mediator configuration, pipeline and the sequential notification
  publisher (D22). Its `.imodspec` interoperability installs this module at `1.0.0-pre.1` when `Intent.DomainEvents` is
  detected. This module declares the core as a dependency at `1.0.0-pre.1` and references its project for one constant
  (see above).
- **Intent.DomainEvents** — written against role names, not against a version contract. The declared floor is 5.1.0; the
  copy in this repository is 5.1.2. Its interoperability detects MediatR and Wolverine but not this module's core.
- **Intent.Modelers.Domain.Events 4.4.7** — `DomainEventModel`, `DomainEventHandlerModel`, `HandledDomainEvents()`,
  `GetDomainEventHandlerModels()`, `GetDomainEventModels()`. Written against 4.4.7.
- **Intent.Modelers.Services 4.0.16** — the `Services(application)` metadata accessor used by the handler registrations.
- **Intent.MediatR.DomainEvents and Intent.Application.Wolverine.DomainEvents** — alternatives, not companions. Each
  generates its own `DomainEventService` for the same `IDomainEventService`, so they are not meant to be installed with
  this module. Migration from MediatR is covered in the plan's migration callout and in the README.
- **UnitTesting** — the plan says its `DomainEventHandlerTest` reads role `Application.DomainEventHandler.Explicit`
  (G20: unchanged unless a test app proves otherwise). Nothing here has exercised it.

## Open questions

- **Intent.DomainEvents and AutoMapper.** `Intent.Application.Wolverine.DomainEvents/CONTEXT.md` forbids declaring
  `Intent.DomainEvents` because it is said to pull AutoMapper in transitively and break Wolverine-only applications. The
  `Intent.DomainEvents` 5.1.2 `.imodspec` in this repository lists no AutoMapper dependency. This module declares it
  anyway, because three role names its templates resolve are only filled by it. Whether the Wolverine claim still holds,
  and whether it matters for a Mediator application, is not established here.
- **`Intent.Infrastructure.DependencyInjection` is not declared.** `DomainEventService` registers itself through a
  `ContainerRegistrationRequest` with concern `Infrastructure`, which that module handles. Neither this module nor the
  core declares it, matching `Intent.MediatR.DomainEvents` and `Intent.Application.Wolverine.DomainEvents`. Without it
  the service would silently not be registered.
- **`Microsoft.Extensions.Logging` 10.0.9 is registered for every framework from net8.** Other modules in the repository
  pick the version per framework. Not checked against a net8 target.
