### Version 5.0.3

- Fixed: `AuthorizeCheckOperationFilter.cs` no longer emits an unused `using System.Collections;` — only `System.Collections.Generic.List<>` was ever used in the generated file.
- Fixed: the Swashbuckle configuration's Bearer security scheme no longer emits an unused `using Microsoft.AspNetCore.Authentication.JwtBearer;` on net8+ apps — `JwtBearerDefaults` is only referenced by the pre-net8 legacy scheme branch, so the using is now introduced there only.
- Improvement: the OpenApi type names referenced in `AuthorizeCheckOperationFilter.cs` (`OpenApiSecurityRequirement`, `OpenApiSecuritySchemeReference`, `OpenApiSecurityScheme`, `OpenApiReference`, `ReferenceType`) are now resolved through `UseType(...)` against the version-dependent `openApiNamespace` (`Microsoft.OpenApi` vs `Microsoft.OpenApi.Models`) instead of being hardcoded alongside a manually-maintained `.AddUsing(openApiNamespace)`. Generated code is unchanged; the namespace is now introduced only when a given branch's code is actually emitted.

### Version 5.0.2

- Improvement: Added default Template Classification and Priorities.

### Version 5.0.1

- Fixed: Swagger security schemes are now applied to each ASP.NET Core project's Swashbuckle configuration, instead of failing the Software Factory when an application has more than one.

### Version 5.0.0

- Improvement: Now supports OAuth2 - Authorization Code flow with improved code generation using builder patterns.
- Improvement: Updated to work with Microsoft.OpenApi (2.4.1) library version.

> ⚠️ NOTE
> The migration to Microsoft.OpenApi (2.4.1) includes API changes that may impact custom Swashbuckle Filters. Please review and update your filter implementations to ensure compatibility after upgrading.

### Version 4.0.11

- Improvement: Updated module documentation to use centralized documentation site.

### Version 4.0.10

- Improvement: Swashbuckle interface no longer shows the lock icon for unsecure endpoints.

### Version 4.0.9

- Improvement: Improved generated code quality by making `AuthorizeCheckOperationFilter.HasAuthorize` static

### Version 4.0.8

- Improvement: Small updated to more align code styling with best practices

### Version 4.0.7

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.0.6

- Improvement: Upgraded module to support new 4.1 SDK features.
 
### Version 4.0.5

- Improvement: Updated dependencies.

### Version 4.0.4

- Changed text of Swagger security JWT Bearer Token security scheme from:
  
  ``Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token` ``

  To:

  ``Enter a Bearer Token into the `Value` field to have it automatically prefixed with `Bearer ` and used as an `Authorization` header value for requests.``



### Version 4.0.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Fixed: Security misconfigured for JWT Bearer tokens in the Swagger UI

### Version 4.0.0

- Update: Combined all the various security modules related to Swashbuckle into one Module.
