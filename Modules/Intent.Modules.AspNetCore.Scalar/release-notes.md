### Version 1.0.7

- Improvement: Added `HideRouteParametersFromBodyOperationTransformer` operation transformer that automatically removes properties from request body schemas when they are already defined as route parameters, preventing duplicate documentation of parameters that are supplied via the URL.

### Version 1.0.6

- Improvement: Updated NuGet package versions.

### Version 1.0.5

- Improvement: Compatibility added for `Microsoft.AspNetCore.OpenApi` versions 10 and greater applied conditionally depending on the target framework of the project.

### Version 1.0.4

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.3

- Improvement: Updated NuGet package versions.
- Fixed: Aligned with `Intent.AspNetCore.Swashbuckle` so that Schema identifier generation to ensure generated swagger schema is OpenAPI compliant.

### Version 1.0.2

- Improvement: Updated NuGet package versions.

> ⚠️ NOTE
>
> When using simple schema identifiers, generic types will use `Of` and `And` to separate generic types and parameters.
> When using full-namespace identifiers, the identifier will use `_Of_` and `_And_` to separate generic types and parameters.

### Version 1.0.1

- Improvement: Updated NuGet package versions.

### Version 1.0.0

Initial Release
