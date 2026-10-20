# Context: Intent.Application.Mediator

## Purpose

Generates a CQRS dispatch stack on the source-generated Mediator library (`Mediator.Abstractions` and
`Mediator.SourceGenerator` 3.0.2) in place of MediatR (licensing exit) or Wolverine. It is the core of a
family: DomainEvents, FluentValidation, Controllers dispatch and FastEndpoints dispatch plug into it, and
EF Core, MassTransit, NServiceBus and Eventing.Wolverine each carry a small Mediator seam.

What sets it apart from the MediatR and Wolverine modules: **an AI writes every handler body.** Commands,
queries and their interactions are modelled, but no field mappings are, and `Intent.Application.DomainInteractions`
and `Intent.Application.CQRS.CRUD` are never used. The module generates stubs, skills and designer
rules; it never turns interactions into code.

The design source is `intent/.plans/2026-10-13-mediator-dispatch-modules.mdx` (decisions D# from the spike in
`Spike/MediatorSpike`, G# from planning). This file records what implementing it taught us.

## Architectural Decisions

- **The core owns the whole pipeline list, and seams only remove items.** `MediatorConfiguration` builds
  `options.PipelineBehaviors` as one literal `CSharpCollectionExpression` in a fixed order (Logging,
  UnhandledException, Performance, Authorization, MessageBusPublish, Validation, UnitOfWork, outermost first).
  Rejected: letting each module insert its own behaviour, as MediatR modules do. The Mediator source generator
  reads `PipelineBehaviors` as a literal at compile time, `CSharpCollectionExpression` has no ordering support,
  and a computed or reordered array silently produces no pipeline.
- **Each behaviour is included only when its own `CanRunTemplate()` passes.** `GetTypeName(templateId)` alone is
  not a valid check: a template of this always-installed module resolves its type name even when it would be
  skipped, so the pipeline would reference a class that is never generated. `GetPipelineBehaviours` finds the
  instance and asks it. The Validation item is the one exception to "in this module": it is resolved through the
  `Infrastructure.Mediator.Dispatch.Behaviours.ValidationBehaviour` role, which FluentValidation fills.
- **The behaviours live in Infrastructure, as Wolverine's middleware does** (developer decision, 2026-10-20). All
  seven, FluentValidation's included, have role `Infrastructure.Mediator.Dispatch.Behaviours.{Name}` and location
  `Dispatch/Behaviours`, mirroring Wolverine's `Infrastructure.Wolverine.Dispatch.Middleware.{Name}` at
  `Dispatch/Middleware`. They sit beside `MediatorConfiguration`, which lists them; Infrastructure references
  Application, so they still reach `ICommand`, `ICurrentUserService`, the bus and unit-of-work interfaces. The
  template ids did not change, so every module that finds a behaviour by id (EF Core, Eventing.Wolverine,
  FluentValidation) is unaffected. Rejected: MediatR's placement in `Application.Common` (`Common/Behaviours`), which
  this module copied until then. `MessageBusFlushOwner` is not a behaviour and stays at `Common/Eventing` in
  Application.
  - In Infrastructure the `TRequest` constraint is written `global::Mediator.IMessage`. The NServiceBus package adds
    `global using NServiceBus;` to that project, and its `IMessage` made the bare name ambiguous (CS0104, found in
    `Tests/TestMediator.Nsb`). `UseType` cannot see global usings that come from a package, so it would not qualify
    the name itself. The FluentValidation `ValidationBehaviour` does the same.
  - An output, once placed, stays put on a reinstall. An application that installed an earlier pre-release keeps its
    behaviours in `Application/Common/Behaviours` until their Template Outputs are moved to the Infrastructure
    project (or the module is uninstalled and reinstalled). After the move, the merge keeps the old
    `using {App}.Application.Common.Behaviours;` in `MediatorConfiguration.cs`, which no longer compiles; delete that
    line once and regeneration leaves it out (seen in `Tests/TestMediator.Nsb`, 2026-10-20).
- **Logging, UnhandledException, Performance and Authorization are always generated** (developer decision,
  2026-10-20), as Wolverine's middleware is. Rejected: the plan's opt-in settings (D9, the three `Enable ...
  behaviour` switches, removed with their Mediator Settings group) and gating Authorization on Identity plus a
  secured request. Without Identity, `AuthorizationBehaviour` is a pass-through (`return next(request,
  cancellationToken);`): no request can be secured there, because `CqrsModelTemplateHelpers` raises an
  `ElementException` for one that is. Wolverine's equivalent is an empty middleware class; Mediator needs a `Handle`
  to be a valid `IPipelineBehavior`.
- **`MessageBusFlushOwner` lives in the core** (G12), registered scoped, beside `MessageBusPublishBehaviour`.
  The flush owner lets a unit of work that spans a nested command (an integration handler Invoking a command)
  defer the flush to whoever owns the transaction, so messages never leave before the commit.
- **No Intent.Mediator.Common module.** Wolverine needs `Wolverine.Common` because Application and Eventing both
  contribute to `WolverineConfiguration` and to host registration. Here the core owns the whole pipeline list and
  eventing modules only remove the tagged flush item, so `MediatorConfiguration` lives in the core.
- **`UnitOfWorkBehaviour` uses `ApplyUnitOfWorkImplementations` with `fieldSuffix: "dataSource"`** so the field is
  named `_dataSource`; EF Core's external-transaction guard keys on that field name.
- **Handlers are Merge stubs** (G4): class `Merge, Signature = Fully`, `Handle` with `Signature = Fully`, an
  `IntentInitialGen` TODO and `throw new NotImplementedException`. The AI replaces the body, regeneration keeps it.
- **AI guidance travels as designer rules and generated skills, never as AI tasks.** AI task providers
  (`AITasksFactoryExtension` + `TemplateAITask`) are deprecated (developer decision, 2026-10-14), so the planned
  "Implement Command/Query Handler" (G16) and "Finish Integration Event Handler" (G17) tasks were removed. What they
  carried now lives in two places. The `AI Settings` rules on the Command, Query and Integration Event Handler element
  extensions reach an agent while it edits the model: they say the handler is AI-written and must be written or
  updated once the Software Factory has generated it. The generated skills say how, and include a "finding the work"
  section: search for `// IntentInitialGen` plus `NotImplementedException` stubs after a run, and for integration
  handlers, check for a `HandleAsync` without `Body = Mode.Ignore` whose subscription has an Invoke or an unmapped
  Publish/Send. Accepted cost: nothing proactively flags an unwritten handler; an agent that neither edits the model
  nor loads the skill will not find it. The skill text alone is the rule for that agent.
- **Satellites add their own guidance to the core's skills; the core's skill text names no satellite.** The
  `mediator-command-handler` skill has no Validator section of its own. `Intent.Application.Mediator.FluentValidation`
  injects one with a factory extension (`BeforeSection("Workflow", ...)`), so it appears only when that module is
  installed (developer decision, 2026-10-20). Rejected: unconditional text in the core ("when the command has a
  validator"), which tells an agent in an app without validation about files that do not exist. The skill is generated
  once per AI harness, so a satellite must use `FindTemplateInstances`, not `FindTemplateInstance` (which throws on the
  duplicates). The core's section titles (`Workflow`, `Output expectations`) are anchors for these injections; renaming
  one silently moves injected sections to the end of the file.
- **The Integration Event Handler rule is back.** It was first removed to keep the core free of the EventInteractions
  modeler, but the Software Factory re-adds that modeler to the imodspec from `modules.config` regardless, and with
  the Finish task gone the rule is the only model-side carrier of the takeover check.
- **No registration factory extension in the core.** A scaffolded `MediatorRegistrationFactoryExtension` was removed
  from the model: the `MediatorConfiguration` template registers `AddMediatorConfiguration` itself through a
  `ServiceConfigurationRequest`. A stale element would keep its id in the imodspec.
- **One icon for the whole family** (developer decision, 2026-10-20). The upstream library has no logo, so all five
  Mediator modules share the same icon: a teal circular badge with a central hub joined to four parts that only
  connect through it, which is the mediator pattern itself. A bolt in the hub (fast, source-generated dispatch) is
  hidden by an SVG media query below 30px, where it would only blot the hub. The Dispatch modules share it too,
  unlike MediatR's dispatch modules, which each have their own. Rejected: code brackets around a diamond (applied
  first, then replaced: it said "fast generated code", not "mediator", and blurred at list size), an "M" monogram
  (easy to confuse with MediatR), a three-node hub (reads as the "share" icon) and six spokes (busy at 16px).
  Teal keeps it apart from MediatR's blue and Intent's own. A new Mediator module should reuse this icon.

## Invariants & Constraints

- **The seam contract is metadata, not text.** `MediatorConfiguration` tags the `AddMediator` invocation with
  `mediator-config` and exposes the collection under `mediator-pipeline`. Every item carries `mediator-behaviour`
  (its name), and the flush item also `eventbus-flush`. A seam finds the invocation by metadata and removes the
  item with `collection.Statements.Remove(item)`.
  - `AddItem` does not set `Parent`, so `CSharpStatement.Remove()` throws on a collection item, and a recursive
    statement search never reaches inside the `PipelineBehaviors` assignment. Do not "simplify" a seam to
    `statement.Remove()`.
  - Seams run in `AfterBuild` at priority 1000; the items are added in an `OnBuild` of this template, so they exist.
- **An empty pipeline is legal, but no longer arises.** Probe (throwaway project, `PipelineBehaviors = []`): the
  source generator accepts an empty literal array, and `Tests/TestMediator.WolverineSubscribeOnly` generated one and
  ran while Logging, UnhandledException and Performance were opt-in. Those three and Authorization are now always in
  the list, so the pipeline always has at least four items. A subscribe-only Eventing.Wolverine app must still not
  get the flush item: putting it back made all that app's tests fail.
- **`NugetPackages` declares Mediator.SourceGenerator with `PrivateAssets=all`.** Declare every NuGet dependency in
  the template constructor, never in an `OnBuild`.
- **Authorization is never silently off:** a secured command or query in an app without Identity raises an
  `ElementException`. That error is what makes the pass-through `AuthorizationBehaviour` safe.
- **Hard-resolved ids and what they mean for the imodspec.** `MediatorConfiguration` calls the throwing
  `GetTypeName("Intent.Application.DependencyInjection.DependencyInjection")` for the assembly marker, so
  `Intent.Application.DependencyInjection` is a real dependency. `CqrsModelTemplateHelpers` / `AuthorizationBehaviour`
  use `Intent.Modules.Metadata.Security` (`SecurityModelHelpers`), so `Intent.Metadata.Security` is a real dependency.
  Neither is in `modules.config`, so the Software Factory does not add them: both are **hand-added** in the imodspec
  (added-if-absent, never pruned). Everything else from `Intent.Application.Identity` and `Intent.Eventing.Contracts`
  is resolved optionally (`TryGetTypeName`, `DoNotThrow`, `CanRunTemplate`), so neither is a dependency.
  `Intent.Modules.Constants`, `Intent.Modules.Common.UnitOfWork` and the `Intent.Modules.Security.Shared` shared
  project compile into the module; `Intent.Common.UnitOfWork` is declared because its behaviour is applied at generation.
- **Domain, Eventing and Services.EventInteractions modelers are in the imodspec but not needed by the code.** They
  appear because they are in `modules.config` (installed for designer context), and the Software Factory re-adds them
  while they are there. No C# reference to them exists. They only matter for the Integration Event Handler skill and
  task, which are gated at runtime on the Eventing.Contracts role and do not need the modelers installed by this
  module. To stop shipping them, uninstall them from the module application (never edit `modules.config` by hand).
- **Settings and designer rules are the only AI guidance the model carries.** The element extensions on Command
  and Query hold `AI Settings` rules (G22); the skills carry the same rule from the other side. An agent that edits
  the model without reading the element's rules misses it — accepted, documented in the README.
  **Known gap against the plan:** G22 also puts a rule on Integration Event Handler when Eventing.Contracts is
  present. The `Mediator Services Extensions` designer settings file only extends Command and Query. The README says
  so. Adding it needs a Module Builder change (not a code change).
- **No settings.** The three opt-in behaviour switches were removed (see Architectural Decisions). The
  `Create implicit domain event handlers` switch (G18) belongs to Mediator.DomainEvents, not the core.

## Module Interactions

- **Intent.EntityFrameworkCore** — `UnitOfWorkExternalTransactionExtension` gains a Mediator branch that finds
  `Intent.Application.Mediator.UnitOfWorkBehaviour` by id and inserts the `HasDbTransaction()` guard. It emits
  `next(request, cancellationToken)` itself: the MediatR branch only recognises `next(cancellationToken)` / `next()`.
- **Intent.Eventing.MassTransit / NServiceBus / Wolverine** — under a transactional outbox they remove the
  `eventbus-flush` item by id (`Intent.Application.Mediator.MediatorConfiguration`); no project reference.
  MassTransit's consumer and Wolverine's integration-event middleware claim `MessageBusFlushOwner`.
- **Satellites** (DomainEvents, FluentValidation) reference this project with `PrivateAssets=All`; the dispatch
  modules use string ids. The ids other modules depend on are listed in the plan under "Template ids other modules
  depend on"; renaming or moving any of those elements in the Module Builder breaks them silently.
- **imodspec interoperability.** Detects Controllers, FastEndpoints, FluentValidation and DomainEvents and installs the
  matching satellite at `1.0.0-pre.1`, and every satellite declares the core at `1.0.0-pre.1`. These family links are
  hand-maintained, and the Software Factory keeps a raised version. Move them together: FluentValidation `pre.1` fills
  the `Infrastructure.Mediator.Dispatch.Behaviours.ValidationBehaviour` role, and a `pre.0` core looks for the old one,
  so a mixed pair silently loses validation (found 2026-10-20, when they were all still at `pre.0`). Seam modules are pinned to the pre-release that carries the seam (EF Core
  5.2.1-pre.0, MassTransit 7.2.4-pre.0, NServiceBus 1.0.4-pre.0, Eventing.Wolverine 1.0.1-pre.0). These must match
  those modules' versions in their own packages; they were bumped for the seams. EF Core Repositories 4.8.2,
  AutoMapper 5.3.10 and Mapperly 1.1.4 are minimum-compatible pins copied from Wolverine's imodspec.
  These pins are recorded here and in the imodspec only; the README names the modules without versions
  (developer decision, 2026-10-14), so a version change never has to be mirrored into it.
- **First generated run (2026-10-15, `Tests/TestMediator.Publisher`).** The core, FluentValidation and Controllers
  dispatch modules generated into an empty Clean Architecture application. Only an empty model has been generated so
  far; handlers, domain events and the runtime seams are not yet exercised.
- **Where the spike scenarios (`Spike/MediatorSpike/README.md`) are covered by generated applications (2026-10-15).**
  Every `Tests/TestMediator.*` application has a hand-owned `*.IntegrationTests` project; handler bodies are the
  reference implementation, so these tests prove the generated pipeline and seams, not AI-written logic.
  S1 CRUD, S2 validation 400 with nothing buffered/committed, S3b anonymous `ISender` dispatch, S7 flush after the
  final commit, S7b outer-owner scope (plus the half the spike could not test: the consumer claims ownership and flushes
  after its own commit) and S8 queries never commit: `TestMediator.Publisher`. S9 FastEndpoints dispatch including the
  400: `TestMediator.Subscriber`. S5/S6: `Tests/TestMediator.RoundTrip` hosts Publisher and Subscriber against one broker.
  S4 and the nested-commit/rollback domain event checks: `TestMediator.Publisher` (an `Order` constructor raises an event whose handler
  sends a command). Pipeline shape, lifetimes and the notification publisher are pinned per application. **Not covered:** the
  spike's in-process domain-event ordering checks (all handlers run one at a time, an event raised by a handler is dispatched in
  the same save), S3 endpoint authorization (ASP.NET
  Core answers before Mediator), S10 handler idempotency (application code, not module behaviour), and the spike's
  `PerformanceBehaviour` reuse and two-async-validators checks.
- **`method.Async()` turns a `ValueTask` return type into `Task`.** The SDK's `Async()` means `Async(asValueTask: false)`,
  which deliberately rewrites `ValueTask` / `ValueTask<T>` to `Task` / `Task<T>`. Every Mediator handler, behaviour and
  the notification publisher must return `ValueTask`, so each template calls `method.Async(asValueTask: true)`. The
  first run generated `async Task<TResponse> Handle` in three behaviours and the publisher; only the behaviours
  failed to compile (CS0738). Any new template here that returns `ValueTask` must use the overload.
- **Not supported in version 1, grouped by what goes wrong.** Not documented in the README (developer decision,
  2026-10-14); recorded here so a future change knows what it would have to fix.
  - *Software Factory crashes:* `Intent.AspNetCore.Controllers.JsonPatch` (any PATCH command; it looks for an
    `IRequest` interface).
  - *AI-written bodies overwritten:* `Intent.Application.CQRS.CRUD` finds handlers by role, sets `Body = Fully` on what
    it fills, and fills query handlers that have no interactions.
  - *Application does not compile:* `Intent.Eventing.MassTransit.RequestResponse` (emits a `MediatR.ISender` consumer),
    Hangfire and Quartz (emit `using MediatR` and `ISender`; may fail at generation instead), gRPC CQRS service, Dapr
    PubSub.
  - *Silently missing or wrong:* `Intent.Entities.Constants` (skips the Mediator validators), OData query and
    Pagination defaults, Blazor `ScopedMediator`, Modular Monolith, and Azure Functions / AWS Lambda / GraphQL dispatch
    (no Mediator dispatch module exists for them).
  - *Works, minus flush ownership:* Azure Service Bus, Kafka, AWS SQS, Solace, Azure Event Grid and Azure Queue Storage
    eventing get the pipeline flush but not the nested-dispatch flush ownership the MassTransit and Wolverine seams add.
  - *Combined dispatchers:* running alongside `Intent.Application.MediatR` or `Intent.Application.Wolverine` in one
    application is not guarded by the module and not supported.
