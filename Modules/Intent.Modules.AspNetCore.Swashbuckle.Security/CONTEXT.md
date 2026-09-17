# Context: Intent.AspNetCore.Swashbuckle.Security

## Purpose

Generates the `AuthorizeCheckOperationFilter` and its Swashbuckle/OpenAPI security-scheme
registration, applying the configured Authentication scheme (Bearer, OAuth2 Authorization Code,
OAuth2 Implicit) to Swagger UI for endpoints/controllers carrying `[Authorize]`.

## Architectural Decisions

- **OpenApi type names are resolved via `UseType($"{openApiNamespace}.TypeName")`, never
  hardcoded alongside a manually-maintained `AddUsing(openApiNamespace)`.** `openApiNamespace` is
  version-dependent (`Microsoft.OpenApi` on net8+/OpenApi 2.4.1, `Microsoft.OpenApi.Models`
  otherwise), and `AuthorizeCheckOperationFilterTemplatePartial.cs`'s `Apply` method emits
  different OpenApi type literals depending on that branch and on whether `[Authorize]` is
  present. Before 5.0.3 the namespace was pulled in unconditionally via
  `.AddUsing(openApiNamespace)` while the type names themselves (`OpenApiSecurityRequirement`,
  `OpenApiSecuritySchemeReference`, `OpenApiSecurityScheme`, `OpenApiReference`, `ReferenceType`)
  were spelled as raw strings inside `AddStatement($@"...")` blocks — the builder had no reference
  to track, so nothing caught it if a branch's usage drifted from the manual `AddUsing` list. Also
  fixed in the same version: a leftover `.AddUsing("System.Collections")` that had never matched
  any actual type in the generated file (only `System.Collections.Generic.List<>` is used) since it
  was added in 2023.

- **The same `UseType`-over-hardcoded-`AddUsing` rule applies to `SecurityConfigurationFactoryExtension.cs`.**
  `AddBearerSecurityScheme` unconditionally called `.AddUsing("Microsoft.AspNetCore.Authentication.JwtBearer")`,
  but `JwtBearerDefaults` (the only type from that namespace) is only spelled out in the pre-net8
  legacy scheme branch — the net8+ branch never references it. Also fixed in 5.0.3: the using is now
  resolved via `template.UseType(...)` at the point `JwtBearerDefaults.AuthenticationScheme` is
  actually built, inside that branch only.

## Invariants & Constraints

- Any new OpenApi type name introduced into `Apply`'s generated statements must go through
  `UseType($"{openApiNamespace}.X")`, not a raw string — this is what makes the using directive
  track the branch that actually emits, instead of requiring someone to keep a hand-maintained
  `AddUsing` list in sync with code inside string-templated statement bodies.
- The same applies to any type referenced only inside one branch of a conditional scheme-building
  method (e.g. `AddBearerSecurityScheme`'s legacy-vs-net8+ split) — resolve it with `UseType(...)`
  inside that branch, never as a top-level `AddUsing()` shared across both.
