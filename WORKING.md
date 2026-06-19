# WORKING.md — feature/wolverine

## SF Cycle Status

### Closed cycles (implementation + build + SF verified)

| Module | Status |
|---|---|
| `Intent.Application.Wolverine` (core) | ✅ Complete |
| `Intent.Application.Wolverine.FluentValidation` | ✅ Complete — SF confirmed in `Wolverine.CQRS.TestApplication` |
| `Intent.Application.Wolverine.DomainEvents` | ✅ Complete — SF confirmed in `Wolverine.CQRS.TestApplication` |
| `Intent.AspNetCore.Controllers.Dispatch.Wolverine` | ✅ Complete |

### Dispatch adapters — build verified, SF cycle requires dedicated test app

| Module | Build | SF Cycle |
|---|---|---|
| `Intent.AzureFunctions.Dispatch.Wolverine` | ✅ 0 errors | ⚠️ Open — needs dedicated test app |
| `Intent.FastEndpoints.Dispatch.Wolverine` | ✅ 0 errors | ⚠️ Open — needs dedicated test app |
| `Intent.AwsLambda.Dispatch.Wolverine` | 🔲 Not started | 🔲 Not started |

---

## Why dedicated test apps are needed for dispatch adapters

`AzureFunctions.Dispatch.Wolverine` and `FastEndpoints.Dispatch.Wolverine` are **mutually exclusive**
with their MediatR counterparts — you install one or the other, not both. All existing
`AzureFunctions.NET8` and `FastEndpoints` test apps already have MediatR dispatch installed.

Installing Wolverine dispatch alongside MediatR dispatch causes:
- `FindTemplateInstance` to find two `EndpointTemplate` instances per model ID → SF error
- Both factory extensions add dispatch code to the same templates → duplicate code

**To close these SF cycles, create dedicated Wolverine-only test apps** in `Tests/`:
- `AzureFunctions.Dispatch.Wolverine.TestApplication` — AzureFunctions + Application.Wolverine + Dispatch.Wolverine
- `FastEndpoints.Dispatch.Wolverine.TestApplication` — FastEndpoints + Application.Wolverine + Dispatch.Wolverine

---

## Next work

1. **AWS Lambda dispatch** (`Intent.AwsLambda.Dispatch.Wolverine`) — factory extension, adapt from
   `Intent.Aws.Lambda.Functions.Dispatch.MediatR`. Shells already scaffolded.
2. **Dedicated Wolverine test apps** for AzureFunctions and FastEndpoints dispatch (see above).
3. **CONTEXT.md updates** — distil architecture decisions into relevant module CONTEXT.md files.
4. **Module docs** — README.md + release-notes.md for each completed module.
