# WORKING.md — Intent.Application.Wolverine

## Current Goal
Implement Increment 3: `WolverineControllerDispatchExtension`.

## What's Done This Branch

### Increment 1 ✅
- All 6 core CQRS template bodies implemented and verified via SF on test app.
- Test app old sub-folder reference files deleted (Items/CreateItem/, Items/GetItemById/, Items/GetItems/) — these were MediatR-era hand-crafted files now replaced by flat module output.
- Test app builds green.

### Increment 2 — Middleware templates & Registration ✅
- Implemented middleware template bodies:
  - `Templates/ValidationMiddleware/ValidationMiddlewareTemplatePartial.cs`
  - `Templates/LoggingMiddleware/LoggingMiddlewareTemplatePartial.cs`
  - `Templates/PerformanceMiddleware/PerformanceMiddlewareTemplatePartial.cs`
  - `Templates/UnhandledExceptionMiddleware/UnhandledExceptionMiddlewareTemplatePartial.cs`
  - `Templates/UnitOfWorkMiddleware/UnitOfWorkMiddlewareTemplatePartial.cs`
  - `Templates/AuthorizationMiddleware/AuthorizationMiddlewareTemplatePartial.cs`
- Implemented `FactoryExtensions/WolverineRegistrationFactoryExtension.cs` to add `builder.Host.UseWolverine(...)` configuration statement in `Program.cs` and DI registration for the 6 middlewares in `DependencyInjection.cs`.
- Module compiles clean; test app runs and starts up without DI exceptions.

## Immediately Next
1. Implement `FactoryExtensions/WolverineControllerDispatchExtension.cs` to dispatch commands and queries from ASP.NET Core controllers using Wolverine's `IMessageBus` instead of MediatR's `IMediator`.
2. Build module → SF on test app → inspect → apply → build test app.

## Cross-Module Type IDs (confirmed)
| Type | Template ID |
|---|---|
| `ICurrentUserService` | `Intent.Application.Identity.CurrentUserServiceInterface` |
| `IValidatorProvider` | `Intent.Application.FluentValidation.Dtos.ValidatorProviderInterface` |
| `IBypassPipelineValidation` | `Intent.Application.MediatR.FluentValidation.BypassPipelineValidationInterface` |
| `IUnitOfWork` | `Intent.Entities.Repositories.Api.UnitOfWorkInterface` |
| `AuthorizeAttribute` | `Intent.Application.Identity.AuthorizeAttribute` |
| `ForbiddenAccessException` | `Intent.Application.Identity.ForbiddenAccessException` |

## Known Issues / Decisions
- `HasDbTransaction()` check omitted from `UnitOfWorkMiddleware.Before()` — that method is added to `IUnitOfWork` only when EF Core's `UnitOfWorkExternalTransactionExtension` is present. Keeping it simple for v1.
- `AddElseStatement` must be called on the **method** (not inside the if block) — it's a sibling, not a child.
- `WolverineOnException` attribute: add using via `UseType("Wolverine.Attributes.WolverineOnException")`, then emit as `[WolverineOnException]` attribute string on the method.
- **GetTypeName timing rule.** `GetTypeName(...)` must not be called directly in the constructor before the `CSharpFile` structure has been defined. Call it inside `AddClass`/`AddMethod` lambdas or in `OnBuild`/`AfterBuild`, after all template instances are registered.
