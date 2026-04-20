### Version 1.0.1-pre.0

- Fixed: `IPatchExecutor<T>` now exposes `ApplyToAsync` (returning `Task`, with optional `CancellationToken`) instead of the synchronous `ApplyTo`. `JsonMergePatchExecutor<T>` now calls `ValidateAsync` internally, resolving `AsyncValidatorInvokedSynchronouslyException` thrown when FluentValidation validators contain async rules (e.g. `CustomAsync`).

### Version 1.0.0

- New Feature: Introduces JSON (Merge) Patching to ASP.NET Core Controllers and ensuring that MediatR / Service Handlers will apply the patching capabilities to your codebase.  