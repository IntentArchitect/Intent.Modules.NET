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
