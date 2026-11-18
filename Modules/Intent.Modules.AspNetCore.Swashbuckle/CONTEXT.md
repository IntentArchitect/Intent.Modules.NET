# Context: Intent.AspNetCore.Swashbuckle

## Purpose
Installs and configures Swashbuckle.AspNetCore: Swagger document generation, Swagger UI, schema
identifiers, XML comments and the operation filters the generated endpoints need.

## Architectural Decisions
- **HTTP `QUERY` is described as OpenAPI 3.2 (since 6.1.0).** Swashbuckle drops `QUERY` actions and writes
  3.0/3.1, which have no `query` operation. `HttpQueryDocumentFilter` renders the `QUERY` descriptions through a
  second, nested `SwaggerGenerator` and adds them to the document, and `OpenApi32DocumentSerializer` (set as
  `SwaggerOptions.CustomDocumentSerializer`) writes the document as 3.2. Proven in the HttpQuery spike first.
- **On Microsoft.OpenApi 3.x (Swashbuckle 11, .NET 11) Swashbuckle does `QUERY` itself (since 6.1.0).**
  Swashbuckle 11 describes `QUERY` actions and writes a native 3.2 document once
  `SwaggerOptions.OpenApiVersion = OpenApiSpecVersion.OpenApi3_2` (its default is 3.0). On that path the module
  sets only that option and keeps the Swagger UI index page. `HttpQueryDocumentFilter`, its
  `DocInclusionPredicate` and `OpenApi32DocumentSerializer` are not generated.
  - Rejected: the .NET 10 serializer on 3.x. It writes 3.1 and relabels to 3.2.0, and Microsoft.OpenApi 3.x writes
    `QUERY` in a 3.1 document as `x-oai-additionalOperations.QUERY`, so the document claimed 3.2.0 without a
    native `query` operation. The `SwashbuckleSettings.NET11` runtime tests caught it.
  - The custom Swagger UI index page is kept on .NET 11, because Swashbuckle 11's own page does not send
    `QUERY`. Swashbuckle 11.0.0 bundles swagger-ui 5.34.1, which can, but its `SubmitMethod` enum still has no
    `Query` and the default `supportedSubmitMethods` lists only the eight enum values, so "Try it out" is
    missing on `QUERY` operations. Checked 2026-11-13 in a headless browser: no button without the page;
    with `query` added to the bundled page's config, it sent `QUERY` with the JSON body and got a 200.
- **Swagger UI is served from a generated index page loading swagger-ui 5.34.1 from unpkg.** The swagger-ui
  bundled with Swashbuckle 10 cannot show `QUERY` (10.3.1 bundles 5.33.3; `QUERY` needs 5.34), and Swashbuckle's
  `SubmitMethod` enum has no `query` value, so the page also adds it to `supportedSubmitMethods`. Rejected:
  only raising the Swashbuckle pin, since no 10.x release bundles 5.34. The page needs internet access; revisit
  once Swashbuckle's `SubmitMethod` gets a `Query` value (then the bundled page is enough on 11 and later).
- **Everything `QUERY` is gated on .NET 10 and on any operation using `QUERY`**
  (`HttpQueryHelper.NeedsHttpQueryOpenApi`), so other apps see no diff.
- **The Microsoft.OpenApi 3.x code shape follows the Swashbuckle version, not the framework (since 6.1.0).**
  Swashbuckle 11 (the net11 entry) depends on `Microsoft.OpenApi` 3.x, where content values are
  `IOpenApiMediaType` with a read-only `Schema`. `MicrosoftOpenApiVersion.IsV3` checks the resolved
  `Swashbuckle.AspNetCore` major (>= 11). It is the single signal for the 3.x path:
  `HideRouteParametersFromBodyOperationFilter` pattern-matches to the concrete `OpenApiMediaType`,
  `TypeSchemaFilter` sets `Examples` (the single `Example` is obsolete in 3.x), and the `QUERY` wiring above
  depends on it.
  Rejected: a framework check, which would drift from a locked or consolidated package version.

## Invariants & Constraints
- **Swashbuckle allows one `DocInclusionPredicate`, and the `QUERY` support takes it**
  (`HttpQueryDocumentFilter.ExcludeFromMainPass`: Swashbuckle's default group match, minus `QUERY`
  descriptions). No other module sets one today. A module that needs its own predicate must compose with
  this one, not replace it, or `QUERY` operations are rendered twice or lost.
- The nested generator shares the main generator's options and so runs the filter again; the filter's
  `RunningNested` guard is what stops it recursing.
- **`SwashbuckleConfiguration` resolves `Path` with `UseType("System.IO.Path")`**, so it writes `Path.Combine` with
  `using System.IO;` (user decision, 2026-11-16: writing `System.IO.Path` literally diffed every consumer's generated
  file). Known gap: HotChocolate.AspNetCore's `build/HotChocolate.AspNetCore.targets` adds `global using HotChocolate;`
  when `ImplicitUsings` is on, and `HotChocolate.Path` then makes `Path` ambiguous (CS0104). `UseType` can't see that
  clash: type resolution knows neither package-supplied global usings nor `HotChocolate.Path`.
- **`HideRouteParametersFromBodyOperationFilter` reads `operation.Parameters` and `operation.RequestBody`
  without `!` on .NET 8 and later** (the Microsoft.OpenApi 2.4.1 branch, which the 3.x path shares), because
  its early-return guard already rules out null for both. SonarAnalyzer 10.35's S8969 flags a `!` there,
  which fails apps that treat warnings as errors. Keep the guard and the plain reads together. (Unverified in
  generated output as of 6.1.1-pre.0: the module could not be installed into TrainingModel.Tests to
  regenerate it. S8969 was confirmed on the old output only.)

## Module Interactions
- **Intent.AspNetCore.Controllers** — produces the `[HttpQuery]` actions this module describes.
- **Intent.AspNetCore.Controllers / Intent.AspNetCore.Controllers.JsonPatch** — their Swagger filters read
  this module's `Swashbuckle.AspNetCore` registration from the `NugetRegistry` to pick the
  `Microsoft.OpenApi` 3.x shape, so the version they see is the one this module registers.
- **Intent.Metadata.WebApi** — the `QUERY` verb needs 4.8.0-pre.0 or later.
