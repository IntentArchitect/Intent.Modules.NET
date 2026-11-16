# Context: Intent.AspNetCore.ODataQuery

## Purpose

Adds OData query options (`$filter`, `$orderby`, `$top`, …) to queries stereotyped `ODataQuery`: it configures OData on each host's controllers, rewrites the generated controller action to take `ODataQueryOptions<T>`, and describes the query options in Swagger through `ODataQuerySwaggerFilter`.

## Architectural Decisions

- **Every lookup is scoped to one host (1.1.0).** An application can have several hosts, each with its own startup, Swagger configuration and controllers. `StartupExtension` and `SwaggerConfigExtension` loop over every instance; `QueryExtension` updates every controller for the query's folder, one per host that exposes it. A host references the filter in its own project (`OutputTarget.FindTemplateInstance`), never application-wide. The singular `application.FindTemplateInstance` threw "More than one instance" as soon as a second host existed. This follows the convention recorded in `Intent.AspNetCore`'s `CONTEXT.md` ("History").
- **`ODataQuerySwaggerFilter` only generates in a project that configures Swagger.** Its `CanRunTemplate` checks for a Swagger configuration in its own project, so a worker host that has the filter's Template Output gets no orphan filter.
- **A host that configures Swagger but has no filter is skipped with a Warning.** Unlike `Intent.Modules.AspNetCore.MultiTenancy`, which logs its equivalent at Debug, this one is a Warning: Swagger without the filter is a real gap (the OpenAPI document omits the query options), not a normal setup. The message names the Template Output to add.

## Verification

The SF test suite `AspNetCore.MultiHost.Tests` (sibling branch `agent/dotnet-11-sf-tests`) failed on 1.0.20 and passes on 1.1.0. Its baseline holds each host's `Program.cs` (`AddOData`), `SwashbuckleConfiguration.cs` (`OperationFilter<ODataQueryFilter>`) and controller (`ODataQueryOptions<T>`), for a net10.0 host and a net11.0 host. The single-host test apps (`AdvancedMappingCrud.Repositories.Tests`, `AdvancedMappingCrud.Cosmos.Tests`, `CleanArchitecture.Comprehensive`, all net8.0) regenerate with 0 changes on 1.1.0.
