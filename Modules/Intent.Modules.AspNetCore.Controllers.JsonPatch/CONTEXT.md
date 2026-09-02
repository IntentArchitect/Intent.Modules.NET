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

This is a real coupling, not an oversight: no other validation pipeline exists in this module's dependency graph yet, so the hardcoded MediatR template ID is the only lookup that currently makes sense. If a second CQRS transport (e.g. Wolverine) is ever added to this module, this coupling — and the hardcoded `MediatRCommandExtension`/`MvcControllerMediatRCommandExtension` targeting — is the first thing that will need to change; record the actual decision here once that work is undertaken, rather than the plan for it.
