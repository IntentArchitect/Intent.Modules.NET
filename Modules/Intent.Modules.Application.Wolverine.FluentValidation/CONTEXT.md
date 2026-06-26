# CONTEXT.md — Intent.Modules.Application.Wolverine.FluentValidation

## Purpose

Generates FluentValidation validator classes for Commands and Queries in a Wolverine-based application.

---

## Key Architectural Decisions

### Shared base class reuse (non-negotiable)

Both `CommandValidator` and `QueryValidator` extend `DtoValidatorTemplateBase` from `Intent.Modules.FluentValidation.Shared` — the same base class used by the MediatR FluentValidation module. This is intentional and deliberate reuse, not coincidence.

**Do not copy validator generation logic from the MediatR module.** Both modules must remain in sync through the shared infrastructure. If the shared base changes, both modules benefit automatically.

### `modelParameterName` matches Wolverine's handler convention

- `CommandValidator` uses `modelParameterName: "command"`
- `QueryValidator` uses `modelParameterName: "query"`

These names match Wolverine's handler parameter naming convention, where handler methods receive the model directly by that name (e.g. `Handle(CreateOrderCommand command)`). Using a different name would produce a validator whose parameter name does not match the handler signature, causing a confusing mismatch.

### Repository injection and custom validation are both enabled

- `repositoryInjectionEnabled: true` — validators may inject repositories for database-level uniqueness or existence checks.
- `customValidationEnabled: true` — partial methods are scaffolded so developers can add bespoke rules without the SF cycle overwriting them.

Both flags must remain enabled. Disabling either removes capability that developers expect.

### Template roles must be fulfilled

Both templates fulfil the standard Intent roles:
- `CommandValidator` → `TemplateRoles.Application.Validation.Command`
- `QueryValidator` → `TemplateRoles.Application.Validation.Query`

These roles are how `Intent.Application.Wolverine` (and any other module that wires validation middleware) finds the validator template instances at SF time. Do not change or remove these role assignments.

---

## Interactions with Other Modules

| Module | Relationship |
|---|---|
| `Intent.Modules.FluentValidation.Shared` | Provides `DtoValidatorTemplateBase` — the only base class for both validators. |
| `Intent.Application.Wolverine` | Consumes the template roles (`TemplateRoles.Application.Validation.Command/.Query`) to wire validation middleware. |
| `Intent.Application.MediatR.FluentValidation` | Sibling module that uses the same shared base. Keep both in sync via the shared package — never diverge their base class strategy. |

---

## Anti-Patterns

- **Do not generate validator code from scratch.** The shared base handles the scaffolding, injection, and partial method pattern. Reinventing this creates drift with the MediatR variant.
- **Do not change `modelParameterName` to a generic value** like `"model"` or `"dto"`. The name is load-bearing — it must match Wolverine's handler method parameter name.
