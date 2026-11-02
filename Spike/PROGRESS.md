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
