### Version 1.0.0

- New Feature: generates an `AbstractValidator` for each Command and Query in the Services designer, and a `ValidationBehaviour` pipeline behaviour that runs the validators registered for a request before its handler.
- New Feature: the validators for a request run one at a time, never in parallel, because asynchronous rules use the scoped `DbContext`, which does not support concurrent operations. A request that implements the generated `IBypassValidation` marker interface skips validation.
- Improvement: validation failures are no longer logged as unhandled errors by the core module's `UnhandledExceptionBehaviour`.
- New Feature: the core module's `mediator-command-handler` skill gains a Validator section that tells an AI agent to write each command validator's rules from the entity the command creates or updates, including unique constraint checks through a repository. Most rules are written this way rather than generated.
- Improvement: the module has its own icon, a teal hub-and-spoke badge shared by every Mediator module and distinct from MediatR's.
