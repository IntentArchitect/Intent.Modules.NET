### Version 1.0.0

- New Feature: generates an `AbstractValidator` for each Command and Query in the Services designer, and a `ValidationBehaviour` pipeline behaviour that runs the validators registered for a request before its handler.
- New Feature: the validators for a request run one at a time, never in parallel, because asynchronous rules use the scoped `DbContext`, which does not support concurrent operations. A request that implements the generated `IBypassValidation` marker interface skips validation.
- Improvement: validation failures are no longer logged as unhandled errors by the core module's `UnhandledExceptionBehaviour`.
