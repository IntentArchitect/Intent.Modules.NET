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

## Microsoft.OpenApi 3.x (since 1.1.0)

`Swashbuckle.AspNetCore` 11 and `Microsoft.AspNetCore.OpenApi` 11 depend on `Microsoft.OpenApi` 3.x, where request body content is an `IOpenApiMediaType` with a read-only `Schema`. The filter and the transformer switch to the 3.x shape (pattern-match to the concrete `OpenApiMediaType`) from the package version the app resolves, not from the framework, so the code moves with the NuGet entry.

This module registers neither package. It reads them with `NugetRegistry.GetVersion`, which throws for a package no installed module registered, so each lookup is guarded by `ExecutionContext.InstalledModules` (`Intent.AspNetCore.Swashbuckle` for the filter, `Intent.AspNetCore.Scalar` for the transformer), the same guard MassTransit uses. Rejected: adding module dependencies on Swashbuckle and Scalar only to read a version, since each template already only runs when its host module's configuration template exists.

## Transformer only from .NET 10 (since 1.1.0)

The transformer calls `OpenApiOperationTransformerContext.GetOrCreateSchemaAsync`, which only exists from `Microsoft.AspNetCore.OpenApi` 10. On .NET 9 it never compiled (broken since 1.0.0), so it is only generated, and only registered in the Scalar configuration, from .NET 10. This matches how the Swashbuckle filter skips older frameworks. Consequence: on .NET 9 with Scalar the OpenAPI document shows the raw `JsonMergePatchDocument<T>` request body. Rejected: a separate .NET 9 code path that builds the payload schema by hand, which has no reference implementation and was chosen against (2026-11-14).
