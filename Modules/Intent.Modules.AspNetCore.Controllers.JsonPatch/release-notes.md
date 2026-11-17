### Version 1.1.0

- Improvement: Added .NET 11 support. With `Swashbuckle.AspNetCore` 11 or `Microsoft.AspNetCore.OpenApi` 11, the generated `JsonMergePatchOperationFilter` and `JsonMergePatchOperationTransformer` use the `Microsoft.OpenApi` 3.x API.
- Fixed: The `JsonMergePatchOperationTransformer` is only generated from .NET 10, where `Microsoft.AspNetCore.OpenApi` has the schema API it uses, so a .NET 9 application using Scalar no longer fails to compile.

### Version 1.0.1

- Fixed: `IPatchExecutor<T>` now exposes `ApplyToAsync` (returning `Task`, with optional `CancellationToken`) instead of the synchronous `ApplyTo`. `JsonMergePatchExecutor<T>` now calls `ValidateAsync` internally, resolving `AsyncValidatorInvokedSynchronouslyException` thrown when FluentValidation validators contain async rules (e.g. `CustomAsync`).

### Version 1.0.0

- New Feature: Introduces JSON (Merge) Patching to ASP.NET Core Controllers and ensuring that MediatR / Service Handlers will apply the patching capabilities to your codebase.