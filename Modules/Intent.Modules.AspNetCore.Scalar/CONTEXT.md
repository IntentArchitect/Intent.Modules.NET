# Context: Intent.AspNetCore.Scalar

## Purpose
Configures `Microsoft.AspNetCore.OpenApi` document generation and the Scalar API reference UI for an
ASP.NET Core app, including the security schemes and the document/operation transformers the generated
endpoints need.

## Architectural Decisions
- **HTTP `QUERY` is described as OpenAPI 3.2 (since 1.1.0).** OpenAPI 3.1 has no `query` operation, so
  `QUERY` endpoints can only appear in a 3.2 document. `Microsoft.AspNetCore.OpenApi` on .NET 10 still drops
  them, so `HttpQueryDocumentTransformer` builds the `QUERY` operations from the API descriptions itself, and
  `UseHttpQueryOpenApi` answers `/openapi/{documentName}.json`: it serializes the document as 3.1 and rewrites
  the version to 3.2, which Scalar 2.18.7 reads and sends. Proven in the HttpQuery spike before templating.
- **Everything `QUERY` is gated on .NET 10 and on any operation using `QUERY`**
  (`HttpQueryHelper.NeedsHttpQueryOpenApi`), so other apps see no diff. On earlier targets the `QUERY`
  endpoints still work; they are just not documented.
- **`UseHttpQueryOpenApi` is registered with an `ApplicationBuilderRegistrationRequest` at priority -2**,
  published from `OnAfterTemplateRegistrations`. That lands it after `UseAuthorization` (-5) and before
  `MapOpenApi`, and Serilog's try-block wrapping moves it along with the rest.
  - Rejected: `StartupFile.AddAppConfiguration<CSharpStatement>(..., priority: -2)` in an `OnBuild` at 100. It
    placed the line correctly but left a stray blank line before `MapControllers` in apps without Serilog.
  - Rejected: the string overload of `AddAppConfiguration`, which drops the priority and lands after the endpoints.

## Invariants & Constraints
- `Scalar.AspNetCore` must stay at 2.18.7 or later; earlier versions cannot send `QUERY` requests.
- The `HttpQueryDocumentTransformer` template must add the `Microsoft.AspNetCore.Routing` using itself; apps
  without implicit usings (FastEndpointsTest) fail without it.

## Module Interactions
- **Intent.AspNetCore.Controllers / Intent.FastEndpoints** — produce the `QUERY` endpoints this module
  describes. The transformer reads endpoint metadata, so it works for both.
- **Intent.Metadata.WebApi** — the `QUERY` verb needs 4.8.0-pre.0 or later.
