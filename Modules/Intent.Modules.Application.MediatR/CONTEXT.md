# MediatR Module Context

> **Module:** `Intent.Modules.Application.MediatR`
> **Family:** `Intent.Modules.Application.MediatR.Behaviours` (pipeline behaviours),
> `Intent.Modules.Application.MediatR.CRUD` (auto-implemented handlers),
> `Intent.Modules.Application.MediatR.FluentValidation`, `Intent.Modules.MediatR.DomainEvents`,
> `Intent.Modules.Application.DependencyInjection.MediatR`. The controller-layer dispatch module
> (`Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR`) has its own narrow CONTEXT.md about
> `Folder` resolution — not duplicated here.
>
> **Purpose:** durable architectural context for this module family, organized around the *code
> paths* its generation logic has to keep working across — read this before changing templates,
> settings, or pipeline-behaviour ordering, and read it first if you are building a **new CQRS
> dispatch module for a different mediator library** (this repo's other example is
> `Intent.Application.Wolverine` — see the comparison section at the end).

---

## What this module is

The MediatR-based CQRS command/query dispatch provider: generates `IRequest`/`IRequestHandler`
command and query models and handlers, and (via the `.Behaviours` companion) the pipeline
behaviours that wrap them — validation, authorization, unit-of-work, message-bus flush, logging,
performance. This module is the *reference shape* several other modules explicitly mirror or
diverge from on purpose — most notably `Intent.Application.Wolverine`, whose own CONTEXT.md cites
this module's constructor-generation pattern by name.

---

## Code paths this module has to account for

| Axis | Values | Where the branch lives | Notes |
|---|---|---|---|
| File layout | consolidated (handler inlined into the Command/Query model file) / split (separate Handler file) | `CQRSSettings.ConsolidateCommandQueryAssociatedFilesIntoSingleFile()`; `CommandHandlerTemplate.CanRunTemplate()`/`QueryHandlerTemplate.CanRunTemplate()` skip the separate template when consolidated; `CommandModelsTemplatePartial`/`QueryModelsTemplatePartial` call `CommandHandlerTemplate.Configure(this, model)` to inline it instead | Also changes folder/namespace shape (`CqrsTemplateHelpers.GetCommandFolderPath/Namespace`) and which template id AI-task generation treats as "the handler" |
| Constructor generation for Commands/Queries | **always** constructor-based — this is not actually a runtime branch | `CommandModelsTemplatePartial`/`QueryModelsTemplatePartial` unconditionally build one constructor and assign every property in it | Contrast with DTOs, which use `ConstructorMapping` vs `ObjectInitializationMapping` depending on shape (`SendOnMediatorInteractionStrategy`'s `CommandQueryMappingResolver`) — Commands/Queries never take the object-initializer path. `CqrsTemplateHelpers.ShouldSetDefaultValue` enforces the C# rule that only properties at/after the last non-defaultable one may get a default value, widening defaulted collection params to nullable + `??` in the assignment |
| Request/response vs notification dispatch shape | Commands/Queries: single-handler `IRequestHandler<TReq[,TResp]>` / Domain events: fan-out `INotificationHandler<DomainEventNotification<T>>` | `CommandHandlerTemplatePartial`/`QueryHandlerTemplatePartial` vs `Intent.Modules.MediatR.DomainEvents`' `DomainEventHandlerTemplatePartial`/`DomainEventNotificationTemplatePartial`/`DomainEventServiceTemplatePartial` (the last reflectively wraps a raw domain event via `Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(...))` and calls `IPublisher.Publish`) | Two structurally different MediatR usage patterns coexist in the same app depending on whether the target is a Command/Query or a Domain Event |
| Dispatch call-site generation | constructor call always assumed | `InteractionStrategies/SendOnMediatorInteractionStrategy.cs` — matches when the target is a Command/Query with exactly one Mapping on the association end; branches on whether a response type exists (whether the result is tracked/captured) | Never has to detect "does a constructor exist" the way Wolverine's own dispatch code does, because this module's own model-generation guarantees the constructor unconditionally (see the Wolverine comparison at the end) |
| Handler implementation | stub (`throw new NotImplementedException`) / CRUD-generated | Base module's `Configure` always emits the stub; the `.CRUD` companion's `StrategyFactory`/`ICrudImplementationStrategy` (Create/Update/Delete/GetById/GetAll/GetAllPagination/ODataGetAll/DomainOp/DomainCtor strategies) replaces it when installed and the shape matches | The shipped `CommandHandlerDecorator`/`QueryHandlerDecorator` (`ITemplateDecorator`, `Priority`) contract is a **separate, unused-by-CRUD** extension point — don't assume CRUD hooks in through it |
| AI-task generation | present when a handler still has `NotImplementedException`, or (split-file mode only) when the contract changed but the handler didn't | `AITasksFactoryExtension.cs` — `HasMissingImplementation`, `GetOnlyContractChangedAITasks` (the latter is meaningless in consolidated mode, since there's no separate contract file to diverge) | |
| Authorization | zero, one, or many `Secured` stereotypes | `CqrsTemplateHelpers.AddAuthorization` via `SecurityModelHelpers.GetSecurityModels(element)` | `CommandModelStereotypeExtensions`/`QueryModelStereotypeExtensions` are **hand-maintained duplicates** of what's normally generated from `Intent.Metadata.Security` — a schema change there won't auto-propagate here |
| .NET/MediatR-version `cancellationToken` argument to `next(...)` | pass it / omit it | Repeated verbatim in 5 separate Behaviour templates: `Project.TryGetMaxNetAppVersion(...).Major is <= 2 or > 6` | Not centralized — a fix to this check must be applied in all 5 places |
| Bus interface naming | legacy `EventBusInterface` role (`eventBus` variable) checked **before** `MessageBusInterface` role (`messageBus` variable) | `MessageBusPublishBehaviourTemplatePartial.GetBusVariableName`/`GetMessageBusInterfaceName` | Code comment: "both interfaces have the `MessageBusInterface` role assigned" — the overlap is real, not a stale deprecation, so the check order is load-bearing |
| Bypass pipeline validation | request implements `IBypassPipelineValidation` / doesn't | `ValidationBehaviourTemplatePartial.cs` short-circuits `return await next(...)` before running any `IValidator<T>` | Presumably for CRUD-generated commands that validate differently or not at all |
| Commercial licensing | `UsePreCommercialVersion` on/off | `Intent.Application.DependencyInjection.MediatR`'s `MediatRSettings` | Emits `MediatR:LicenseKey` app-setting placeholder + `cfg.LicenseKey = ...` when off |

---

## Pipeline behaviour registration order (load-bearing)

Each behaviour registers via `ContainerRegistrationRequest.ToRegister(...).ForConcern("MediatR").WithPriority(n)`,
collected by `Intent.Modules.Application.DependencyInjection.MediatR`'s
`DependencyInjectionFactoryExtension`, and emitted **`.OrderBy(x => x.Priority)`** as successive
`cfg.AddOpenBehavior(...)` calls. MediatR nests behaviours in *registration* order (first
registered = outermost), so ascending priority = outer → inner:

| Priority | Behaviour | Effect of position |
|---|---|---|
| 0 | `UnhandledExceptionBehaviour` | Outermost — its `catch` wraps every other behaviour and the handler |
| 1 | `PerformanceBehaviour` | Times everything below it |
| 2 | `AuthorizationBehaviour` | Rejects unauthorized calls before validation/UoW/handler run |
| 4 | `ValidationBehaviour` (FluentValidation) **and** `MessageBusPublishBehaviour` (Behaviours) — **same priority, a genuine tie** | Both sit outside `UnitOfWorkBehaviour`. Tie-break depends on which independently-versioned module's factory extension happened to run first — `OrderBy` is stable, but insertion order across two unrelated modules picking the same number is not something either module controls. Treat this as a latent footgun, not an intentional ordering guarantee |
| 5 | `UnitOfWorkBehaviour` | Innermost — commits immediately after the handler returns |

**Why priority 4 outside 5 matters:** `MessageBusPublishBehaviour` does `var response = await
next(...); await _messageBus.FlushAllAsync(cancellationToken);` — sitting outside
`UnitOfWorkBehaviour` means the flush only runs *after* the transaction has already committed. If
this were ever renumbered to ≥5, events could flush before (or despite) a rolled-back transaction.
`Intent.Application.Wolverine`'s CONTEXT.md explicitly cites these exact MediatR priorities as the
precedent for its own `MessageBusFlushMiddleware`-before-`UnitOfWorkMiddleware` ordering — **this
is the invariant to preserve in any new mediator-library module**: outer flush wraps inner
unit-of-work, however that library expresses pipeline nesting.

---

## Cross-module "flush after handler" interop with eventing broker modules

Two different tagging strategies coexist for the same underlying pattern — this module family
uses the **weaker** one, worth fixing forward rather than copying as-is:

- The `services.AddMediatR(cfg => {...})` invocation is tagged `.AddMetadata("mediatr-config",
  true)`, but the individual `cfg.AddOpenBehavior(typeof(MessageBusPublishBehaviour<,>))`
  statement carries **no metadata tag at all**. `Intent.Modules.Eventing.NServiceBus`'s
  `NServiceBusMessageBusInteropExtension` and `Intent.Modules.Eventing.MassTransit`'s
  `MessageBusInteropExtension` strip it (when a transactional outbox is selected) by finding the
  `mediatr-config`-tagged lambda and doing a **fragile string search**:
  `stmt.GetText("").Contains("MessageBusPublishBehaviour")`. Renaming that class silently breaks
  both extensions.
- By contrast, the equivalent flush call sites in controller dispatch and in
  `Intent.Application.Wolverine`'s `ApplicationHandlerPolicy` are tagged
  `.AddMetadata("eventbus-flush", true)` — the convention both eventing modules' interop
  extensions actually prefer, and the one to use for any *new* mediator/dispatch module.
- Once a durable outbox is detected on the eventing side, the generic post-handler flush is
  removed and spliced directly into `DbContext.SaveChanges`/`SaveChangesAsync` instead, regardless
  of which dispatch mechanism is in play.

**Takeaway for a new mediator-library module:** tag your own "flush the bus after the handler
succeeds" statement with `"eventbus-flush"` from day one, rather than leaving eventing modules to
find it by matching your generated class name in source text.

---

## `Handle` body mode is keyed to whether a CRUD module is installed

`CommandHandlerTemplate.Configure`/`QueryHandlerTemplate.Configure` (and so the consolidated
single-file path too) take the `Handle` attribute from
`Templates/HandlerBodyModeExtensions.GetHandleMethodIntentManagedAttribute`:

- **`Intent.Application.MediatR.CRUD` or `Intent.Application.CQRS.CRUD` installed** →
  `Fully` + `Body = Mode.Merge`, unchanged from published versions. A CRUD module overrides it to
  `Body = Mode.Fully` on the handlers it implements. MediatR.CRUD finds those handlers by this
  module's `TemplateId`, CQRS.CRUD by `TemplateRoles.Application.Handler.*`.
- **Neither installed** → `Fully` + `Body = Mode.Ignore`. Nothing can ever generate the body, so it
  belongs to the developer or AI from the first run.

The stub is emitted in both cases. The check is a soft `InstalledModules.Any(...)` string lookup, so
no `.imodspec` dependency on either CRUD module is needed or wanted (CRUD depends on this module, not
the other way round).

**The AI only needs one rule.** `mediatr-command-handler`/`mediatr-query-handler` keep the published
rule, now worded "Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write
its body" (4.7.7). A Merge stub is just "not Ignore yet", the same as a Fully body, so the AI never has
to reason about Merge semantics. Without a CRUD module the rule is already satisfied.

**Why Merge stays on unimplemented handlers when a CRUD module is installed** (decided 2026-10-14 with
the module owner, after trying the alternatives below on this branch, none published):

- It is upgrade-safe: CRUD apps see no change from published output.
- A mapping added to a handler after its first SF run still gets implemented (Merge→Fully is rewritten).

**Rejected alternatives:**

- **Merge on CRUD-implemented bodies, with state-aware skill text** ("augment the body, don't edit
  generated statements"), so mapping changes could keep flowing into customised handlers. Withdrawn
  because Merge's contract confused the AI: it has to tell generated statements from its own, and
  editing a generated statement in place makes Intent re-add it as a duplicate. The accepted cost of
  the Fully/Ignore rule is that once a handler is switched to Ignore, mapping changes no longer reach it.
- **Fully on every handler, stubs included, when CRUD is installed** (to remove Merge from output).
  It silently deletes code in existing apps: developers implement handlers directly in published Merge
  stubs. Observed on RoslynWeaver 5.0.1: Merge→Fully rewrites the attribute and keeps the body on the
  next run, and a re-run with no output change keeps it too. But the next model change that alters the
  file's output replaces the body with the stub. The SF flags it as destructive only at that point.
- **Ignore on every unimplemented handler, even with CRUD installed.** Upgrade-safe, but Ignore is
  sticky: a handler whose mapping is added after its first SF run would never be implemented without a
  manual attribute switch.

**How RoslynWeaver treats the attribute on an existing handler** (RoslynWeaver 5.0.1, observed
2026-10-13/14, partly on service operations, same mechanics):

- **`Merge`→`Ignore`** (an app with no CRUD module upgrading): the attribute is rewritten and a
  hand-written body survives.
- **`Fully`→`Merge`**: the attribute is rewritten and the body is unchanged.
- **`Ignore` is sticky.** An existing `Body = Mode.Ignore` is never rewritten; its body survived a model
  change while the template generated `Fully`. So installing a CRUD module into an app whose handlers
  were generated without one does not implement those existing handlers.
- **Switching `Ignore` back to `Body = Mode.Fully` by hand hands the body back to CRUD.** It is replaced
  with the generated statements, flagged destructive, on the next run that changes the file's output,
  not on an unchanged re-run.
- **Uninstalling a CRUD module from an app** removes the statements it generated from bodies that are
  still Merge in that run (observed). Fully bodies were not re-tested.

**Adding a new handler-implementing module?** Add its module id to
`HandlerBodyModeExtensions.CrudImplementationModuleIds`. Otherwise its handlers will be generated
with `Body = Mode.Ignore` in apps without a CRUD module, and Ignore is sticky.

---

## Handler skills: trimmed text, no pointer rule (4.7.7)

Shared by the handler-skill family: this module, `Intent.Application.Mediator`,
`Intent.Application.Mediator.DomainEvents`, `Intent.Application.ServiceImplementations` and
`Intent.Application.Wolverine`. The other modules' `CONTEXT.md` files point here.

- **The skill text is the evaluated text, not a summary of it.** In November 2026 the handler skills
  were cut to the shape that held up in an evaluation of agent output, and only that shape is
  generated. Kept: the `Mode.Ignore` line, Domain/Application-only dependencies, load through
  repositories and change aggregates through domain behaviour, follow the nearest similar handler,
  no in-memory aggregation, the repository-method rule, the `cancellationToken`, helper methods are
  fine, and for queries "filter and page in the query" and "a query changes no state". Removed: text
  that restated those rules, the GPT-era "chatgpt should update the handler" descriptions, and the
  unit-of-work save note in the query skill (a query never saves). Before adding a rule back,
  evaluate it: length was measured to cost attention without changing what agents wrote.
- **Rules the trim dropped and the developer restored (developer decision, 2026-11-26).** The trim
  also dropped rules that were not repetition, and the evaluation could not have caught their loss:
  its rubric scores repository and domain use, no infrastructure, no unneeded save, mapping and code
  management on one command and one query, and none of these. Restored, one line each, without an
  evaluation, across the MediatR, Mediator, Mediator.DomainEvents, Wolverine and service-implementation
  skills: change only what the task needs and keep unrelated refactors out; let the domain model's
  terms drive the code; look for similar handlers, repository methods and domain operations before
  writing, and don't invent a new result or exception style; for commands and service writes, check
  existence and whether the caller may act (ownership, eligibility, status) the way nearby handlers
  do; for queries and service reads, apply nearby handlers' business filters and visibility rules and
  load only what the response needs. Mediator also got back "check a similar handler before asking"
  for ambiguous field mappings. The EF Repositories save rule got back "if unsure, leave it out",
  "once and before returning", the row-version and computed-column examples, and removing unneeded
  `SaveChangesAsync` and `Update` calls in code under review; AutoMapper and Mapperly got back "a
  projection that compiles, or a mapping for another type, is not proof" and "a missing mapping is
  not a reason to build a DTO by hand". A second pass, same day, put back the old Workflow and
  review sections as one line each: "Start by reading the handler, its request, … and the domain
  types it touches" (plus "work out the intended flow from their names and nearby features" where no
  model is the specification, so not in Mediator), and "Before finishing, review your code against
  every rule in this skill, the sections below included" (Mediator adds checking every modelled
  association). AutoMapper and Mapperly each got one short example block instead of the old long
  ones, and the AutoMapper rule now names the generated DTO's own `Mapping(Profile)` (`IMapFrom<T>`)
  and the generated `MapToXxxDto(_mapper)` extensions: before, it said only "in a `Profile`", which
  invited a duplicate profile beside the generated mapping. Still out on purpose: the repeated
  sections, the "cite file path + excerpt" gates and the long code examples. The "evaluate first"
  rule above applies to rules beyond these.
- **The repository-method rule distinguishes modelled from hand-written methods.** "If a designer will
  use the method, for example mapped from a command or query, model it as a repository operation.
  Otherwise add it by hand to the repository interface and the repository." An unmodelled method the
  designer needs is invisible to mapping; a modelled method nothing maps to is model noise.
- **Persistence and mapping sections come from the modules that own them**, through `WithSection`, in
  factory-extension order: `Intent.EntityFrameworkCore` (Order 0) writes the loading rule,
  `Intent.EntityFrameworkCore.Repositories` (Order 1) adds the save and `Update` rules to the same
  "Persistence (EF Core)" section, and AutoMapper or Mapperly (Order 2) add "Mapping (…)". The core
  skills hold no anchor sections for them any more; `WithSection` appends at the end.
- **No pointer rule; standard skills only (developer decision, November 2026).** A glob-scoped
  instruction whose only job was to name the skill to load was built and then removed before release.
  In the evaluation (skills inventory report, §17) the trimmed skills loaded in every run without it:
  agents loaded them from the skill names `get_applications` lists, before the first handler write,
  with the same rubric score. Moving the skill bodies into glob-scoped instructions instead was rejected:
  no measurable saving, Codex reads no scoped instructions, OpenCode loads every instruction file each
  turn, and every consumer would need a migration for outputs it has already placed. So the skill's
  description carries the trigger: it says when to use the skill and names the file kind.

---

## Gotchas / footguns

- **Priority-4 tie** between `ValidationBehaviour` and `MessageBusPublishBehaviour` — see above.
- **String-based stripping of `MessageBusPublishBehaviour`** by NSB/MassTransit interop — a class
  rename silently breaks two other modules with no compile error.
- **`EventBusInterface`/`MessageBusInterface` role overlap** — the `if` check order in
  `GetBusVariableName`/`GetMessageBusInterfaceName` is significant, not incidental.
- **`.NET`-version-conditional `cancellationToken` argument** duplicated across 5 files rather than
  centralized.
- **`SendOnMediatorInteractionStrategy` is registered inside `CommandHandlerTemplateRegistration.GetModels`**,
  not a factory-extension `OnBeforeTemplateRegistrations` hook — an easy place to miss when
  auditing where a strategy gets wired up. It's registered once and matches both Commands and
  Queries, so the Query registration class does not re-register it.
- **`Mediator12Migration`** in `CommandHandlerTemplatePartial.cs` strips residual `return
  Unit.Value;` statements left over from MediatR v11→v12 removing `Unit` for void-returning
  requests — evidence this module has had to migrate previously-generated consumer code across a
  breaking upstream API change via `ITemplateMigration`, not just a settings toggle.
- **`Migration_04_03_00_Pre_00`** renamed a bespoke `Authorize` stereotype to the shared `Security`
  stereotype — `CommandModelStereotypeExtensions`/`QueryModelStereotypeExtensions` still carry a
  comment admitting they're "manually copied from Intent.Modules.Metadata.Security from what is
  normally generated" — a manual duplication that can silently go stale.

---

## Module settings and stereotypes

| Setting | Owner | Gates |
|---|---|---|
| `ConsolidateCommandQueryAssociatedFilesIntoSingleFile` | this module | File-layout axis |
| `UsePreCommercialVersion` | `Intent.Application.DependencyInjection.MediatR` | MediatR license-key emission |
| `UseAmbientTransactions` / `AutomaticallyPersistUnitOfWork` | `.Behaviours` | Passed through to `UnitOfWorkBehaviourTemplatePartial`'s shared `ApplyUnitOfWorkImplementations` helper |

| Stereotype/marker | Applies to | Gates |
|---|---|---|
| `Secured`/`Unsecured` | Command/Query models | `[Authorize]`-equivalent attribute via `CqrsTemplateHelpers.AddAuthorization` |
| `IBypassPipelineValidation` (interface, not a stereotype) | Command/Query implementations | Short-circuits `ValidationBehaviour` |

`.imodspec` `<interoperability>` auto-installs, when detected: `Intent.AzureFunctions.Dispatch.MediatR`,
`Intent.HotChocolate.GraphQL.Dispatch.MediatR`, `Intent.Application.MediatR.Behaviours`,
`Intent.Application.MediatR.CRUD`, `Intent.Application.MediatR.FluentValidation`,
`Intent.MediatR.DomainEvents`, `Intent.Dapr.AspNetCore.StateManagement`,
`Intent.EntityFrameworkCore(.Repositories)`, `Intent.Application.AutoMapper`,
`Intent.Application.Dtos.Mapperly` — this module is deliberately the root of a family that
self-assembles based on what else is installed.

---

## Designer validation: default value order on Commands and Queries

- **The warning lives here, not in the designer module.** `designers/MediatR Services Extensions` holds Command and
  Query Element Extensions whose Extension Settings › Validate Function warns when a property without a default
  value follows one with a default value. It exists because `CqrsTemplateHelpers.ShouldSetDefaultValue` silently
  drops any default that is not part of the trailing run, so the warning belongs to the module whose template drops
  it. Rejected: leaving it in `Intent.Modelers.Services.CQRS` (its home until 6.0.8). There it fired in applications
  that generate no constructor at all.
- **Intent appends an extension's Validate Function; it does not replace the element's own.** The client evaluates
  `settings.validateFunctions` as an array, so every installed module's check runs and each one adds its own
  message. The generated XML calls it `validateFunctionOverride`, but nothing is overridden.
- **Ungated.** Commands and Queries always get a constructor, so there is no setting to check.
- **`Intent.Application.Wolverine` carries an identical check.** The two modules are alternative dispatch
  implementations and are never installed together, so neither guards against the other. Keep the two scripts
  identical.
- **Version contract with `Intent.Modelers.Services.CQRS`:** the floor is 6.0.9 (written against `6.0.9-pre.0`), the
  first version without its own copy of the check. That module's `<interoperability>` updates this one to 4.7.6 in
  return. Lowering either side lets an application run both copies (a duplicate warning) or neither.

---

## How this compares to `Intent.Application.Wolverine`

`Intent.Application.Wolverine`'s CONTEXT.md explicitly credits this module's constructor-generation
shape (§1/axis 2 above) as the pattern it mirrors, and cross-references these exact pipeline
priorities to justify its own middleware ordering. The two modules diverge deliberately, not
accidentally, in one place: this module's Commands/Queries **must** implement `IRequest<T>`/
`IRequestHandler<TReq,TResp>` because MediatR's entire dispatch model is built on those interfaces
— there's no way around that coupling here. Wolverine's own anti-pattern list explicitly forbids
the equivalent (`IWolverineHandler`/`[WolverineHandler]` marker interfaces) because Wolverine
discovers handlers by naming convention instead, so it can and does avoid the coupling MediatR is
forced into. A new mediator-library module should decide early which side of that line its own
library falls on, rather than assuming either shape is "the" correct one.

Similarly, because this module's model-generation guarantees a constructor unconditionally,
`SendOnMediatorInteractionStrategy` never has to *detect* whether one exists at a given call site —
Wolverine's own dispatch code does have to detect it, precisely because Wolverine's message types
don't universally get a generated constructor the way Commands/Queries here always do.
