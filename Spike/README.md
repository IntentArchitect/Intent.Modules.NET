# HTTP QUERY spike

Working samples of the HTTP `QUERY` method (RFC 10008) on .NET 10, built before any module work so the module templates copy proven code. Plan: `intent/.plans/2026-11-02-http-query-working-sample.mdx`.

The spike is committed on `agent/http-query` and is removed before that branch's PR merges, once module verification (Phase 1) no longer needs it.

## What works

Every app has the same Product model (from `scripts/model-domain.js` and `scripts/model-services.js`). The QUERY endpoints are modelled as POST with a 200 response, because the Http Settings stereotype has no QUERY verb yet, and the generated code is then hand-edited to QUERY.

| App                                 | Surface                                                                 | Tests                    | OpenAPI UI                                  | Try-it sends QUERY |
| ----------------------------------- | ----------------------------------------------------------------------- | ------------------------ | ------------------------------------------- | ------------------ |
| `HttpQuery.Controllers.Swashbuckle` | MVC controllers + Swashbuckle 10.3.1                                    | 7/7                      | Swagger UI 5.34.1, Scalar                   | yes, both          |
| `HttpQuery.Controllers.OpenApi`     | MVC controllers + Microsoft.AspNetCore.OpenApi 10 (Yolo's target stack) | 8/8                      | Swagger UI 5.34.1, Scalar.AspNetCore 2.18.7 | yes, both          |
| `HttpQuery.FastEndpoints`           | FastEndpoints 8.2 + Microsoft.AspNetCore.OpenApi 10                     | 7/7                      | Scalar.AspNetCore 2.18.7                    | yes                |
| `HttpQuery.Client`                  | Intent-generated HttpClient proxy calling the Swashbuckle app           | 2/2 against the live API | –                                           | –                  |

Each test suite was run red before its QUERY code existed. A Software Factory re-run on every app proposes no changes, so the hand edits survive regeneration. Microsoft.OpenApi 3.11 reads every app's document as OpenAPI 3.2 with no errors.

## The OpenAPI decision

ASP.NET Core 10, Swashbuckle 10 and Microsoft.OpenApi 2.x stop at OpenAPI 3.1, and 3.1 has no slot for QUERY. Out of the box, Swashbuckle throws (`The "QUERY" HTTP method is not supported.`) and Microsoft.AspNetCore.OpenApi drops the endpoint.

Two shapes were compared on a 3.0 document:

| Shape                                              | Swagger UI  | Scalar | Try-it      | Microsoft.OpenApi reader             |
| -------------------------------------------------- | ----------- | ------ | ----------- | ------------------------------------ |
| A: native `query` key in a 3.0/3.1 document        | no          | yes    | Scalar only | 2.x: "query is not a valid property" |
| A: native `query` key, document labelled **3.2.0** | yes (5.34+) | yes    | yes         | 3.x: valid OpenAPI 3.2               |
| B: `x-oai-additionalOperations`                    | no          | no     | no          | valid                                |

**Decision (user):** native `query` operations in a document labelled OpenAPI 3.2. A 3.1 document plus `query` operations is a valid 3.2 document, since 3.2 only adds to 3.1. This is also what ASP.NET Core 11 and Swashbuckle 11 will produce, so the bridge code can be switched off then, by target framework or package version, without changing the output.

Minimum UI versions found:

- **Swagger UI 5.34.0.** Swashbuckle 10.2.3 bundles 5.32.7 and 10.3.1 bundles 5.33.3. Both show nothing, so the samples serve swagger-ui-dist 5.34.1 through `SwaggerUIOptions.IndexStream`.
- **Swagger UI's `supportedSubmitMethods` needs `query`.** Swashbuckle's `SubmitMethod` enum has no QUERY, so the custom index page adds it, otherwise "Try it out" is hidden.
- **Scalar.AspNetCore: 2.16.11 ignores `query` operations; 2.18.7 shows them.** The exact minimum between those was not pinned down.

## The code each surface needed (the template change list)

### Controllers (both OpenAPI stacks)

- `HttpQueryAttribute` (`Api/Controllers/HttpQueryAttribute.cs`): an `HttpMethodAttribute` subclass for `HttpMethods.Query`, mirroring `HttpPostAttribute`. ASP.NET Core 10 has no such attribute. Note that `Microsoft.AspNetCore.Http.HttpMethods` has to be qualified inside it, because `HttpMethodAttribute` has its own `HttpMethods` property.
- The action uses `[HttpQuery("route")]` where the template emits `[HttpPost("route")]`.
- Everything else in the generated action is unchanged: `[FromBody]` binding, the 200 response type and `Ok(result)` (from Success Response Code 200 in the model), and the route/body mismatch check.

### Swashbuckle (`HttpQuery.Controllers.Swashbuckle`)

- `Api/Filters/HttpQueryDocumentFilter.cs`:
  - A `DocInclusionPredicate` keeps QUERY descriptions out of the main pass. It is the default group match, minus QUERY.
  - A document filter runs copies of those descriptions, re-labelled POST, through a second `SwaggerGenerator` built from the same options. Every operation filter, XML comment and response type still applies. The filter then moves each operation into the path item's `query` slot.
  - It merges component schemas, tags and security schemes, and rebinds tag references to the main document.
  - An `AsyncLocal` re-entrancy guard stops the nested generator running this filter again. Dependencies are resolved inside `Apply`, not the constructor, because Swashbuckle builds filters while it is still building its options.
- `Api/Filters/OpenApi32DocumentSerializer.cs`: an `ISwaggerDocumentSerializer` that writes OpenAPI 3.1 and labels it 3.2.0.
- `Api/Configuration/HttpQueryOpenApiConfiguration.cs`: one `ConfigureHttpQueryOpenApi()` call that registers the predicate, the filter, the serializer and the Swagger UI index page (5.34.1, with `query` in the submit methods).

### Microsoft.AspNetCore.OpenApi (`HttpQuery.Controllers.OpenApi`, `HttpQuery.FastEndpoints`)

- `Api/Filters/HttpQueryDocumentTransformer.cs`: a document transformer. An operation transformer never sees QUERY, and ASP.NET Core's operation generation is internal. So the transformer builds each operation from the ApiExplorer description:
  - path, query and header parameters, plus any route-template parameter the description lacks (FastEndpoints describes none);
  - the body and the declared responses;
  - tags, summary, description and operation id from endpoint metadata, falling back to the controller name for tags.

  It turns every schema with an `x-schema-id` into a component reference at every depth, reusing existing components. It then runs the generated `HideRouteParametersFromBodyOperationTransformer` on the operation.
- `Api/Configuration/HttpQueryOpenApiConfiguration.cs`:
  - `ConfigureHttpQueryOpenApi()` registers the transformer for all documents.
  - `UseHttpQueryOpenApi()` answers `/openapi/{name}.json` ahead of `MapOpenApi`. It uses the public `IOpenApiDocumentProvider`, writes 3.1 and labels it 3.2.0, because ASP.NET Core 10 has no serializer hook.
  - In the controllers app it also serves Swagger UI on the same document, using `Swashbuckle.AspNetCore.SwaggerUI` only (UI, no generator).
- Gap: XML-comment operation transformers do not run on the hand-built operations. Summaries come from endpoint metadata only.

### FastEndpoints

- `Configure()` calls FastEndpoints' own `Query("route")` where the template emits `Post("route")`. FastEndpoints 8.2 binds the request DTO from the body for QUERY, as for POST. Category binds from the route without being repeated in the body.

### HttpClient proxy (`Integration.HttpClients.Shared`)

- The generated method uses `new HttpRequestMessage(HttpMethod.Query, ...)` where it emits `HttpMethod.Post`. Body and route handling are unchanged. `HttpMethod.Query` exists from .NET 10; older targets would need `new HttpMethod("QUERY")`.

## Protections and spike-only edits (strip before Phase 1)

| App                     | File                                                             | Edit                                                                                                                                                      | Protection                                    |
| ----------------------- | ---------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------- |
| Controllers.Swashbuckle | `Api/Controllers/ProductsController.cs`                          | `SearchProducts`, `SearchCategoryProducts` use `[HttpQuery]`                                                                                              | `[IntentManaged(Mode.Ignore)]` per action     |
| Controllers.Swashbuckle | `Api/Program.cs`                                                 | `ConfigureHttpQueryOpenApi()`; spike-only `EnsureCreated` behind `HTTPQUERY_ENSURE_DB`                                                                    | Merge-mode file                               |
| Controllers.Swashbuckle | `Api/*.csproj`                                                   | Swashbuckle.AspNetCore 10.2.3 → 10.3.1                                                                                                                    | SF never lowers versions                      |
| Controllers.OpenApi     | `Api/Controllers/ProductsController.cs`                          | as above                                                                                                                                                  | `[IntentManaged(Mode.Ignore)]` per action     |
| Controllers.OpenApi     | `Api/Program.cs`                                                 | `ConfigureHttpQueryOpenApi()`, `UseHttpQueryOpenApi()`, spike-only `EnsureCreated`                                                                        | Merge-mode file                               |
| Controllers.OpenApi     | `Api/*.csproj`                                                   | Scalar.AspNetCore 2.16.11 → 2.18.7; add Swashbuckle.AspNetCore.SwaggerUI 10.3.1                                                                           | –                                             |
| FastEndpoints           | `Api/FastEndpoints/Products/Search*QueryEndpoint.cs`             | `Configure()` uses `Query(...)`                                                                                                                           | `[IntentManaged(Mode.Ignore)]` on `Configure` |
| FastEndpoints           | `Api/Program.cs`                                                 | as Controllers.OpenApi                                                                                                                                    | Merge-mode file                               |
| FastEndpoints           | `Api/*.csproj`                                                   | Scalar.AspNetCore → 2.18.7; stale `FastEndpoints.Swagger.Swashbuckle` and `Swashbuckle.AspNetCore` removed (left behind when Swashbuckle was uninstalled) | –                                             |
| Client                  | `Infrastructure/HttpClients/Products/ProductsProxyHttpClient.cs` | two search methods use `HttpMethod.Query`                                                                                                                 | `[IntentManaged(Mode.Ignore)]` per method     |
| Client                  | `HttpQuery.Client.ProxyChecks/`                                  | hand-written check project, not in the solution                                                                                                           | not generated                                 |

New, non-generated files (the attribute, filters, transformers, configuration, test helpers, `OpenApi*` tests) are not listed; Intent leaves them alone. Handler bodies are the usual `Body = Mode.Ignore` implementations.

`// IntentIgnore` on a statement inside a Fully-managed method did **not** survive regeneration (`SwashbuckleConfiguration.ConfigureSwagger`), so the registration moved to the Merge-mode `Program.cs`.

## Verb decisions in the modules (hand-over to the module plan)

Where the modules branch on the verb today, and how QUERY should be treated:

| Decision                        | Where                                                                                                                                                                                                                                   | QUERY should                                                            |
| ------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------- |
| Verb list and parsing           | `Intent.Metadata.WebApi` (Intent.Modules repo): `HttpVerb`, Http Settings options, `Enum.Parse`                                                                                                                                         | gain `Query` / `QUERY`                                                  |
| Body allowed                    | `Intent.Metadata.WebApi` `HttpEndpointModel` (body only on POST/PUT/PATCH), `HttpEndpointModelFactory` `GetHttpInputSource` and `verbAllowsBody`                                                                                        | behave like POST (body)                                                 |
| Verb in generated code          | Controllers `ControllerTemplatePartial` `[Http{Verb}]`; FastEndpoints `EndpointTemplatePartial` helper switch; `Integration.HttpClients.Shared` `HttpMethod.{Verb}`                                                                     | emit `[HttpQuery]` (+ attribute file), `Query(...)`, `HttpMethod.Query` |
| Response code, docs and result  | Controllers `ControllerTemplatePartial` (XML response doc, `ProducesResponseType`, both `default: throw`), `Utils.GetReturnStatement`; FastEndpoints `EndpointTemplatePartial` `Produces`, `Utils`; Azure Functions dispatch extensions | behave like GET (200, `Ok`)                                             |
| Can return 404                  | Controllers `Utils.CanReturnNotFound`; Lambda `LambdaFunctionClassTemplatePartial`                                                                                                                                                      | behave like GET                                                         |
| Skip Unit of Work / event flush | `ServiceContractControllerInstaller`, FastEndpoints and Lambda `ServiceContractEndpointExtension`, `AzureFunctions.EntityFrameworkCore`                                                                                                 | behave like GET (read-only)                                             |
| Output caching                  | `AspNetCore.OutputCaching.Redis` `ControllerExtension` (`== "GET"`)                                                                                                                                                                     | decide: QUERY responses are cacheable only when keyed on the body       |
| OpenAPI                         | Swashbuckle and Scalar modules                                                                                                                                                                                                          | add the filter / transformer and the 3.2 document serialization above   |
| Not supported                   | AWS Lambda (`LambdaHttpMethod` has no Query); Azure Functions `HttpTrigger("query")` unverified; Ocelot upstream enum                                                                                                                   | block with a validation error, or verify first                          |

## Findings outside this spike (parked, not fixed)

- `Intent.FastEndpoints` checks module id `Intent.Swashbuckle.AspNetCore` (the real id is `Intent.AspNetCore.Swashbuckle`), so half of its "incompatible with Swashbuckle v6+" guard never fires. The incompatibility is real: `FastEndpoints.Swagger.Swashbuckle` 2.3.0 throws `TypeLoadException` against Swashbuckle 10.
- Clean Architecture + FastEndpoints without the JWT component crashes at startup: `UseAuthorization` without `AddAuthorization`.
- FastEndpoints + Microsoft.AspNetCore.OpenApi documents no route parameters (the generated POST operation lacks `category` too).
- Generated controllers repeat route values in the body: the body's `Category` must match the route, and the documents keep it in the body schema.

## Running it

- Build and test from a short path. The deep worktree makes test paths exceed 260 characters, so the test host gets a `\\?\` base directory that breaks Swashbuckle's XML comments and the xUnit v3 launcher: `subst H: <worktree>\Spike`, then `dotnet test H:\HttpQuery.Controllers.Swashbuckle\HttpQuery.Controllers.Swashbuckle.sln` (and the same for the other apps).
- `scripts/serve-and-check.sh`: starts an API and runs the headless UI check (`ui-check.js`, playwright-core driving Edge).
- `scripts/validate-openapi.cs`: reads a document with Microsoft.OpenApi 3.11.
- `scripts/client-check.sh`: runs the generated proxy against a live Swashbuckle API.
- Evidence (test runs, documents, screenshots): `C:\Dev\.evidence\Intent.Modules.NET\agent\http-query\` with `INDEX.md`.
