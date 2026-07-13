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

### The `CustomValidationSkill` AI agent skill file is inherited, never duplicated

This module deliberately does **not** own a `CustomValidationSkill` template. The skill file (`.agents/skills/fluent-validation-custom-validation/SKILL.md`) is owned by the base `Intent.Application.FluentValidation` module and is inherited via an imodspec `<dependency id="Intent.Application.FluentValidation" .../>` declaration — the exact same pattern `Intent.Application.MediatR.FluentValidation` uses for the same file.

This was not obvious the first time around: a duplicate `CustomValidationSkillTemplate` was mistakenly created directly inside this module before the sibling MediatR module's imodspec was checked. That duplicate collided on the exact same output path as the base module's template whenever both happened to be installed in the same app (which they usually are, since `Intent.Application.FluentValidation.Dtos` also depends on the base module) — one of the two templates silently never executed, and a "0 changes" SF result did not mean the duplicate worked; it meant the collision was masking it. The fix was to delete the duplicate template entirely and add the missing imodspec dependency instead.

**Do not re-add a `CustomValidationSkill` (or similarly named) template to this module.** If the skill content ever needs to change, change it in the base `Intent.Application.FluentValidation` module — both MediatR and Wolverine apps inherit it from there.

The dependency floor is pinned at `3.12.2` (not the lower `3.11.6` this module's `.csproj` compiles against) because `3.12.0` and earlier generated the skill file at a different, since-renamed output path (`custom-fluent-validation` instead of `fluent-validation-custom-validation`). A lower floor resolves to a version whose template deletes/recreates the file at the wrong path on install. If bumping this dependency in the future, verify the resolved base-module version still uses `fluent-validation-custom-validation` before lowering the floor.

---

## Interactions with Other Modules

| Module | Relationship |
|---|---|
| `Intent.Modules.FluentValidation.Shared` | Provides `DtoValidatorTemplateBase` — the only base class for both validators. |
| `Intent.Application.Wolverine` | Consumes the template roles (`TemplateRoles.Application.Validation.Command/.Query`) to wire validation middleware. |
| `Intent.Application.MediatR.FluentValidation` | Sibling module that uses the same shared base. Keep both in sync via the shared package — never diverge their base class strategy. |
| `Intent.Application.FluentValidation` | Declared as an imodspec dependency solely to inherit the `CustomValidationSkill` AI agent skill template — not for validator generation logic. |

---

## Anti-Patterns

- **Do not generate validator code from scratch.** The shared base handles the scaffolding, injection, and partial method pattern. Reinventing this creates drift with the MediatR variant.
- **Do not change `modelParameterName` to a generic value** like `"model"` or `"dto"`. The name is load-bearing — it must match Wolverine's handler method parameter name.
- **Do not add a duplicate `CustomValidationSkill` template to this module.** It already inherits one from `Intent.Application.FluentValidation` via the imodspec dependency; a second copy collides on the same output path.
