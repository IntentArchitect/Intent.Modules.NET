# WORKING.md — feature/wolverine

## SF Cycle Status

### Closed cycles (implementation + build + SF verified)

| Module | Status |
|---|---|
| `Intent.Application.Wolverine` (core) | ✅ Complete |
| `Intent.Application.Wolverine.FluentValidation` | ✅ Complete |
| `Intent.Application.Wolverine.DomainEvents` | ✅ Complete |
| `Intent.AspNetCore.Controllers.Dispatch.Wolverine` | ✅ Complete |
| `Intent.AzureFunctions.Dispatch.Wolverine` | ✅ Complete — SF cycle closed against `Wolverine.AzureFunctions` test app, 0 errors |
| `Intent.FastEndpoints.Dispatch.Wolverine` | ✅ Complete — SF cycle closed against `Wolverine.AspNetCore.FastEndpoints` test app, 0 errors |
| `Intent.Aws.Lambda.Functions.Dispatch.Wolverine` | ✅ Complete — factory extension + template registration implemented; SF cycle closed against `Wolverine.AwsLambdaFunctions` test app, 0 errors |

---

## Outstanding work (before branch can merge)

1. ~~**Wolverine.AspNetCore.Controllers**~~ — ✅ Complete — SF cycle closed, `ProductsController` dispatches via `IMessageBus`, 0-error build.
2. **CONTEXT.md** — Architecture decisions need to be distilled into `CONTEXT.md` files for each module directory.
3. **Module docs** — `README.md` + `release-notes.md` required for all 7 modules (mandatory per AGENTS.md: docs in the same turn as code change, currently overdue).
4. **Skills update** — `module-increment-loop` and `tech-pattern-researcher` need updates from 7 learnings captured in the plan.

---

## Architectural constraints to preserve

- **AutoMapper isolation:** `Intent.Application.Wolverine.DomainEvents` must NOT declare `Intent.DomainEvents` (the full IA module) as an imodspec `<dependency>`. It transitively installs AutoMapper, which is absent in Wolverine-only apps and causes `KeyNotFoundException` at SF time. Safe dep: `Intent.Modelers.Domain.Events` (designer NuGet, no mapper chain).
- **Middleware registration placement:** All middleware `AddTransient` DI registrations belong inside `ApplicationHandlerPolicy.Apply()`, co-located with `AddMiddleware` calls. `WolverineConfiguration.Configure()` only handles assembly discovery and delegates to the policy.
- **`supportedClientVersions` floor:** All modules reference `Intent.SoftwareFactory.SDK` v3.14.0, which requires minimum IA client 5.0.0-a. Every `.imodspec` must use `[5.0.0-a,6.0.0)`.
- **Dispatch exclusivity:** Wolverine dispatch modules are mutually exclusive with their MediatR counterparts. Always use a dedicated Wolverine-only test app per platform.
- **Template registration classes must be `public`:** SF engine discovers them via `Assembly.GetExportedTypes()` — `internal` classes are silently skipped (0 staged changes).

---

## Follow-up branch (after this branch merges)

- `Intent.Blazor.Server.Dispatch.Wolverine` — Generates `IScopedMessageBus` + `ScopedMessageBus` (wraps `IMessageBus` via `IScopedExecutor` for per-call child DI scopes). Gated on `RenderMode = InteractiveServer` AND `Intent.Application.Wolverine` installed. Reference: `Modules/Intent.Modules.Blazor/Templates/Templates/Server/ScopedMediator*`.
