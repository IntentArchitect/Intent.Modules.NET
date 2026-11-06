# Context: Intent.AspNetCore.Swashbuckle

## Purpose
Installs and configures Swashbuckle.AspNetCore: Swagger document generation, Swagger UI, schema
identifiers, XML comments and the operation filters the generated endpoints need.

## Architectural Decisions
- **HTTP `QUERY` is described as OpenAPI 3.2 (since 6.1.0).** Swashbuckle drops `QUERY` actions and writes
  3.0/3.1, which have no `query` operation. `HttpQueryDocumentFilter` renders the `QUERY` descriptions through a
  second, nested `SwaggerGenerator` and adds them to the document, and `OpenApi32DocumentSerializer` (set as
  `SwaggerOptions.CustomDocumentSerializer`) writes the document as 3.2. Proven in the HttpQuery spike first.
- **Swagger UI is served from a generated index page loading swagger-ui 5.34.1 from unpkg.** The swagger-ui
  bundled with Swashbuckle cannot show `QUERY` (10.3.1 bundles 5.33.3; `QUERY` needs 5.34), and Swashbuckle's
  `SubmitMethod` enum has no `query` value, so the page also adds it to `supportedSubmitMethods`. Rejected:
  only raising the Swashbuckle pin, since no release bundles 5.34 yet. The page needs internet access; revisit
  once Swashbuckle bundles swagger-ui 5.34 or later.
- **Everything `QUERY` is gated on .NET 10 and on any operation using `QUERY`**
  (`HttpQueryHelper.NeedsHttpQueryOpenApi`), so other apps see no diff.

## Invariants & Constraints
- **Swashbuckle allows one `DocInclusionPredicate`, and the `QUERY` support takes it**
  (`HttpQueryDocumentFilter.ExcludeFromMainPass`: Swashbuckle's default group match, minus `QUERY`
  descriptions). No other module sets one today. A module that needs its own predicate must compose with
  this one, not replace it, or `QUERY` operations are rendered twice or lost.
- The nested generator shares the main generator's options and so runs the filter again; the filter's
  `RunningNested` guard is what stops it recursing.

## Module Interactions
- **Intent.AspNetCore.Controllers** — produces the `[HttpQuery]` actions this module describes.
- **Intent.Metadata.WebApi** — the `QUERY` verb needs 4.8.0-pre.0 or later.
