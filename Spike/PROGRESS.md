# HTTP QUERY spike – progress

## Done

- Solution `Spike/intent/HttpQuery.isln`, shared model scripts.
- FastEndpoints + Swashbuckle gate failed (Intent blocks it); the user moved FastEndpoints to Scalar.
- OpenAPI shape: native `query` in an OpenAPI 3.2 document (user decision after the UI matrix).
- `HttpQuery.Controllers.Swashbuckle`: 7/7, Swagger UI + Scalar, validator, SF re-run clean.
- `HttpQuery.Controllers.OpenApi` (Yolo's target stack): 8/8, Swagger UI + Scalar, validator, SF re-run clean.
- `HttpQuery.FastEndpoints`: 7/7, Scalar, validator, SF re-run clean.
- `HttpQuery.Client`: generated proxy over QUERY, 2/2 against the live API, SF re-run clean.
- `README.md`: decision, template change list, protections, verb inventory, findings.

## Next

- Module plan, from README "The code each surface needed" and "Verb decisions in the modules", starting with `Intent.Metadata.WebApi` (Intent.Modules repo).

## Blocked

- Nothing.

## Module work (plan `intent/.plans/2026-11-03-http-query-module-support.mdx`), updated 2026-11-03

### Done (code + `dotnet build` only, not yet proven on the spike apps)

- Step 4 version gate + WebApi 4.8.0-pre.0 refs (`093db3c472`)
- Step 5 Controllers + dispatch (`94304b43fa`), Step 6 FastEndpoints + HttpClients (`5931183ce2`)
- Step 7 Swashbuckle: gated `HttpQueryDocumentFilter`, `OpenApi32DocumentSerializer`, Swagger UI index registered in `SwashbuckleConfiguration` (`945ab676ad`)
- Step 8 Scalar: gated `HttpQueryDocumentTransformer`, `UseHttpQueryOpenApi`, Scalar.AspNetCore 2.18.7 (`552dcb4a39`)

### Next

- Step 9: prove on the four spike apps. Step 10: permanent test-app endpoints + tests. Step 11: close-out per module, remove `Spike/`.

### Blocked

- Step 9: the spike solution cannot find the locally built NET modules. `Intent.Metadata.WebApi` 4.8.0-pre.0 installed from the global local feed (`C:\Dev\.intent-modules`), but `Intent.AspNetCore.Controllers` 7.2.0-pre.0 fails with "An error occurred trying to get details of the module". The `.imod` files are in `Modules/Intent.Modules/` (worktree), listed only by `Modules/intent.repositories.config`, not by the spike solution. Needs an asset-repository entry for that folder (the user's call); then restart Intent Architect if it still does not show.
