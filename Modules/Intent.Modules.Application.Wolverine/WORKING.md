# WORKING.md — Intent.Application.Wolverine

## Current Goal
Implement Increment 2: middleware template bodies + `WolverineRegistrationFactoryExtension`.

## What's Done This Branch

### Increment 1 ✅
- All 6 core CQRS template bodies implemented and verified via SF on test app.
- Test app old sub-folder reference files deleted (Items/CreateItem/, Items/GetItemById/, Items/GetItems/) — these were MediatR-era hand-crafted files now replaced by flat module output.
- Test app builds green.

### Increment 2 — Middleware templates 5/6 ✅
Implemented template bodies (constructor only, no stubs):
- `Templates/ValidationMiddleware/ValidationMiddlewareTemplatePartial.cs`
- `Templates/LoggingMiddleware/LoggingMiddlewareTemplatePartial.cs`
- `Templates/PerformanceMiddleware/PerformanceMiddlewareTemplatePartial.cs`
- `Templates/UnhandledExceptionMiddleware/UnhandledExceptionMiddlewareTemplatePartial.cs`
- `Templates/UnitOfWorkMiddleware/UnitOfWorkMiddlewareTemplatePartial.cs`

Module builds clean (`dotnet build` exits 0) after these changes.

## Immediately Next
1. Implement `FactoryExtensions/WolverineRegistrationFactoryExtension.cs`:
   - Find `App.Program` template via `FindTemplateInstance<IProgramTemplate>(TemplateDependency.OnTemplate("App.Program"))`  
   - Add `builder.Host.UseWolverine(opts => { ... })` with 6 `AddMiddleware` calls using `typeof(ICommand)` as assembly anchor  
   - Find Application DI template via `FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DependencyInjection)`  
   - Add 6 `services.AddTransient<XMiddleware>()` calls
2. Build module → SF on test app → inspect → apply → build test app
3. Move to Increment 3: `WolverineControllerDispatchExtension`

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
