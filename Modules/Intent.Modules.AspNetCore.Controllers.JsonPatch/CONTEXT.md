# AspNetCore.Controllers.JsonPatch — Context

## Current state

This module is currently coupled to **MediatR** as its CQRS transport. The factory extensions (`MediatRCommandExtension`, `MvcControllerMediatRCommandExtension`) target MediatR command templates by role (`TemplateRoles.Application.Command`) and decorate them directly.

In particular, `MediatRCommandExtension` hardcodes a dependency on the MediatR FluentValidation bypass interface:

```csharp
template.TryGetTypeName(
  "Intent.Application.MediatR.FluentValidation.BypassPipelineValidationInterface",
    out var bypassValidationInterface)
```

This works today because PATCH commands need to bypass pipeline validation (the patch executor handles its own application), and MediatR FluentValidation is the only validation pipeline this module currently knows about.

## Future direction — Wolverine support

When adding Wolverine support to this module, the following adjustments will be needed:

1. **Bypass interface** — swap the hardcoded MediatR template ID for the shared role `Application.Common.BypassValidationInterface`. Both MediatR FluentValidation and Wolverine FluentValidation fulfil this role, so the lookup will work regardless of which transport is installed:
   ```csharp
   template.TryGetTypeName("Application.Common.BypassValidationInterface", out var bypassValidationInterface)
   ```

2. **Handler decoration** — the `MediatRCommandExtension` targets commands via `TemplateRoles.Application.Command`. A Wolverine equivalent would need to target Wolverine handler templates similarly (or a shared command role if one is introduced).

3. **Controller wiring** — `MvcControllerMediatRCommandExtension` and `MvcControllerTraditionalServiceExtension` target MVC controller templates that dispatch via MediatR. A Wolverine variant would target the Wolverine dispatch controller templates (`AspNetCore.Controllers.Dispatch.Wolverine`).

4. **Module descriptor** — the `.imodspec` interoperability block and `factoryExtensions` list will need Wolverine-aware entries and detection blocks.

Consider introducing a `WolverineCommandExtension` factory extension (mirroring `MediatRCommandExtension`) that applies the same PATCH command decoration logic for Wolverine handlers, rather than modifying the existing MediatR extension.
