# Context: Intent.Application.Mediator.FluentValidation

## Purpose

FluentValidation for the Mediator module family: a validator per Command and Query, the pipeline behaviour that runs
them, the bypass marker interface, and a small patch so validation failures are not logged as unhandled errors. It is a
satellite of `Intent.Application.Mediator` (plan: `intent/.plans/2026-10-13-mediator-dispatch-modules.mdx`, decisions
G1, G5, D20, D21).

## Architectural Decisions

- **The module fills roles; the core places the behaviour.** `ValidationBehaviour` only fulfils
  `Application.Common.ValidationBehaviour`. The core's `MediatorConfiguration` resolves that role and adds the
  Validation item to its literal `PipelineBehaviors` array (order: ... MessageBusPublish, Validation, UnitOfWork).
  Rejected: inserting the item from this module, as the MediatR FluentValidation module does for its own pipeline. The
  Mediator source generator reads `PipelineBehaviors` as a literal at compile time and `CSharpCollectionExpression` has
  no ordering support, so only the module that owns the whole list can keep the order right. The role is also how the
  core knows validation exists at all; an app without this module simply has no Validation item.
- **Validators run one at a time (D21).** The behaviour awaits each `validator.ValidateAsync(context, ...)` in a
  `foreach` and aggregates the failures into one `ValidationException`. Rejected: running validators concurrently
  (`Task.WhenAll`). Async rules such as unique constraint checks share the scoped `DbContext`, which does not support
  concurrent operations. The generated code carries a comment saying so; do not "optimise" it.
- **`IBypassValidation` is a marker, not a convention.** It fills `Application.Common.BypassValidationInterface`, the
  same role the MediatR and Wolverine modules fill, and the behaviour short-circuits with
  `request is IBypassValidation`. Nothing in the family adds the interface to a request automatically.
- **Validators reuse `DtoValidatorTemplateBase` from `Intent.Modules.FluentValidation.Shared`** (a shared project
  imported into this csproj, as in the Wolverine module), keyed to the Mediator `CommandModels` / `QueryModels` template
  ids. Rejected: copying generation logic from a sibling module. All three transports must stay in sync through the
  shared base. `sourceElementAdvancedMappings` is `null`, so the base never injects a repository and emits no
  unique-constraint checks; it still emits explicit field-stereotype rules and whatever domain-constraint rules it can
  reach through the command's own associations. That is accepted, not a gap to fix: as with handlers, rule
  derivation belongs to the AI, guided by a Validator section that this module's `AISkillsExtension` injects into the
  core's `mediator-command-handler` skill, before its `Workflow` section, so the guidance exists only when this module
  is installed (developer decision, 2026-10-20). Only the command handler skill gets it: query validation rarely has
  entity constraints to derive. Rejected: passing the command's `AssociatedElements` as MediatR does, which would
  rebuild the deterministic path this family deliberately leaves behind. Whatever the base still generates by default
  is left alone.
- **`modelParameterName` is `"command"` for commands and `"query"` for queries.** This is what the Wolverine module's
  own CONTEXT.md documents for its validators (see the note under Module Interactions about that module's code). It
  only affects the lambda parameter name in the generated rules.
- **The custom validation AI skill is inherited, never duplicated.** The `fluent-validation-custom-validation` skill is
  owned by `Intent.Application.FluentValidation` and arrives through the imodspec dependency. A second template for the
  same output path collides silently with the base module's template (the Wolverine module's history).
- **The unhandled-exception patch is a factory extension on the core's template, found by id.**
  `UnhandledExceptionBehaviourExtension` looks up `Intent.Application.Mediator.UnhandledExceptionBehaviour` with
  `FindTemplateInstance` and inserts `catch (ValidationException) { throw; }` above the general `catch (Exception)`. The
  lookup is null-safe because the core only runs that template when its opt-in setting is on. Without the patch every
  400-class failure would be logged as an unhandled error. Mirrors the MediatR module's `UnhandledExceptionBahaviorExtension`.

## Invariants & Constraints

- **Reference the core by constants, not types.** The core project is referenced with `PrivateAssets=All`, so only
  constants (template ids, which the compiler inlines) are safe. The core's `UnhandledExceptionBehaviour` id is a string
  constant in the extension; `CommandModelsTemplate.TemplateId` and `QueryModelsTemplate.TemplateId` are used as inlined
  constants. Renaming or moving any of those File Templates in the core's Module Builder breaks this module silently.
- **The `ValidationBehaviour` and `BypassValidationInterface` template ids and roles are a contract with the core.**
  The core finds `Application.Common.ValidationBehaviour` by role; changing the role here removes validation from the
  pipeline without any error.
- **The patch depends on the core's catch block shape.** It matches a `CSharpCatchBlock` whose `ExceptionType` is
  exactly `"Exception"` and silently does nothing if the core changes it (the method is found with `FindMethod("Handle")`
  and `FindStatement`, both of which return null rather than throw). If the core's behaviour changes shape, re-check
  this extension.
- **Mediator signatures.** Handle is `ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest,
  TResponse> next, CancellationToken cancellationToken)`, with `TRequest : IMessage`, and calls `next(request,
  cancellationToken)`. Not interchangeable with MediatR's `next()`.
- **NuGet dependencies are declared in the template constructor** (`AddNugetDependency(NugetPackages.FluentValidation(...))`),
  never in an `OnBuild`.

## Module Interactions

- **Intent.Application.Mediator** - written against `1.0.0-pre.0`. Supplies the pipeline list, `CommandModels` /
  `QueryModels` templates and `UnhandledExceptionBehaviour`. Its imodspec interoperability installs this module when
  `Intent.Application.FluentValidation` is detected.
- **Intent.Application.FluentValidation** - floor `3.12.2`. Versions up to `3.12.0` generated the custom validation skill at
  a different, since-renamed path; a lower floor would delete and recreate the file at the wrong path on install (finding
  recorded in the Wolverine module's CONTEXT.md, adopted here unchanged).
- **Intent.Application.Wolverine.FluentValidation / Intent.Application.MediatR.FluentValidation** - siblings on the same
  shared validator base.
  - The Wolverine module's own CONTEXT.md says its `QueryValidator` uses `modelParameterName: "query"`, but its code
    passes `"command"` for queries (the MediatR module does the same). This module follows the documented intent, `"query"`.
    The difference only shows in the lambda parameter name of the generated rules, not in behaviour.

## Superseded

- Nothing yet.
