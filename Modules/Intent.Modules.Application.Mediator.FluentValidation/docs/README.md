# Intent.Application.Mediator.FluentValidation

Adds [FluentValidation](https://fluentvalidation.net/) to the commands and queries of `Intent.Application.Mediator`. It generates a validator for every Command and Query, and a validation step in the Mediator request pipeline that runs those validators before the handler.


## What This Module Generates

| Output | What it is |
|---|---|
| `CommandValidator` | An `AbstractValidator<TCommand>` for each Command in the Services designer. |
| `QueryValidator` | An `AbstractValidator<TQuery>` for each Query in the Services designer. |
| `ValidationBehaviour<TRequest, TResponse>` | A Mediator pipeline behaviour (Infrastructure layer, `Dispatch/Behaviours`, beside the core's behaviours) that runs the validators registered for the request. |
| `IBypassValidation` | A marker interface (`Common/Interfaces`) that lets a request opt out of validation. |

It also adjusts the core's `UnhandledExceptionBehaviour` (see below).

## Installation

Installing `Intent.Application.FluentValidation` into an application that already has `Intent.Application.Mediator` installs this module as well (the core module declares it as interoperability). It requires `Intent.Application.Mediator`; install the core first if you add this module by hand.

The module has no settings of its own.

## Validators

Command and query validators are built on the same shared validator base used by the MediatR and Wolverine FluentValidation modules, but, like the handlers in this module family, most of their rules are written by an AI agent rather than generated. The module generates the rules modelled explicitly on a field with the validation stereotype, plus `NotNull`, enum and nested DTO checks. Rules derived from the target entity's constraints, such as maximum lengths, are generated only where the shared base can still reach the entity through the command's own associations, and unique constraint checks are never generated, so the unique constraint validation setting of `Intent.Application.FluentValidation` has no effect here. This module adds a Validator section to the `mediator-command-handler` skill from `Intent.Application.Mediator`, telling the agent to write those rules from the entity the command creates or updates, and to protect them with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on `ConfigureValidationRules`. The custom validation hook is enabled so you can add bespoke rules that survive regeneration. In the generated rules, the lambda parameter is named `command` for commands and `query` for queries.

The custom validation AI skill (`fluent-validation-custom-validation`) is owned by `Intent.Application.FluentValidation` and is shared by every transport; this module does not generate its own copy.

## The Validation Pipeline Behaviour

`ValidationBehaviour<TRequest, TResponse>` implements Mediator's `IPipelineBehavior`. For each request it:

1. Returns straight to the next step if the request implements `IBypassValidation`.
2. Otherwise, if any `IValidator<TRequest>` is registered, validates the request with each validator and collects every failure.
3. Throws a single `FluentValidation.ValidationException` carrying all the failures if there were any.
4. Otherwise continues to the next step in the pipeline.

A request with no registered validators is passed through untouched.

### Where it sits in the pipeline

You do not register this behaviour yourself. `Intent.Application.Mediator` owns the whole ordered list of pipeline behaviours and adds the validation step when it finds the `Infrastructure.Mediator.Dispatch.Behaviours.ValidationBehaviour` template role, which this module fills. The fixed order is, outermost first: Logging, UnhandledException, Performance, Authorization, MessageBusPublish, Validation, UnitOfWork. Validation therefore runs after authorization and before the unit of work is opened, so an invalid command never starts a transaction.

### Validators run one at a time

The validators for a request run sequentially, never in parallel. Asynchronous rules, such as unique constraint checks, use the scoped `DbContext`, and a `DbContext` does not support concurrent operations. Running them with something like `Task.WhenAll` would make validation intermittently fail with concurrency errors in applications that use Entity Framework Core.

### Bypassing validation

`IBypassValidation` is an empty marker interface. A command or query that implements it skips all validators. Use it for specialised messages where pipeline validation is redundant or deferred to a later stage. The module only generates the interface; it does not add it to any request.

## Validation Failures Are Not Logged As Unhandled Errors

This module inserts a `catch (ValidationException) { throw; }` block ahead of the general `catch (Exception)` in the core's `UnhandledExceptionBehaviour`. Validation failures are expected request failures, so without this block every rejected request would be logged as an unhandled exception. The exception is still rethrown, so it propagates up the pipeline to the caller unchanged.

## Dependencies

- `Intent.Application.Mediator` - owns the pipeline, the command and query contracts the validators are keyed to, and the `UnhandledExceptionBehaviour` this module adjusts.
- `Intent.Application.FluentValidation` - shared FluentValidation settings, registration and the custom validation AI skill.
- `Intent.Modelers.Services` and `Intent.Modelers.Services.CQRS` - the Command and Query designer elements.

## Related Modules

- [Intent.Application.FluentValidation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-fluentvalidation/intent-application-fluentvalidation.html) - the shared FluentValidation module.
- [Intent.Application.MediatR.FluentValidation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-mediatr-fluentvalidation/intent-application-mediatr-fluentvalidation.html) - the equivalent module for MediatR.
- [Intent.Application.Wolverine.FluentValidation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-wolverine-fluentvalidation/intent-application-wolverine-fluentvalidation.html) - the equivalent module for Wolverine.
- `Intent.Application.Mediator` - the core module of this family (no published page yet).
