### Version 2.0.8

- Improvement: Updated NuGet package versions.

### Version 2.0.7

- Improvement: Updated topic documentation format.

### Version 2.0.6

- Fixed: Incorrect Nuget package installation under wrong .NET version.

### Version 2.0.5

- Improvement: Enhanced the OpenApiResponse attributes added to the `Run` method

### Version 2.0.4

- Improvement: Added OpenAPI Operation stereotype, to allow OpenApiOperation customisation.

### Version 2.0.3

- Improvement: Updated NuGet package versions.

### Version 2.0.2

- Improvement: Updated NuGet package versions.

### Version 2.0.1

- Improvement: Updated NuGet package versions.

### Version 2.0.0

- Improvement: Updated code and dependencies in line with the Isolated Process upgrade.

### Version 1.1.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.1.2

- Improvement: Updated NuGet packages to latest stables.

### Version 1.1.1

- Improvement: Endpoints will now generate an `[OpenApiIgnoreAttribute]` if they have an _OpenAPI Settings_ Stereotype applied with its _Ignore_ property set to `true`.

### Version 1.1.0

- Improvement: Module project updated to .NET 8.
- Fixed: `OpenApiParameter` will only mark parameters as mandatory that are not nullable.

### Version 1.0.4

- Improvement: Updated icon to SVG format.

### Version 1.0.3

- Improvement: Now respects _OpenAPI Settings_ stereotype's _OperationId_ property for customizing the `operationId` for an endpoint, requires at least version `4.3.1` of the `Intent.Metadata.WebApi` module.

### Version 1.0.2

- Fixed: Resolved and issue around nullable types not generating valid C# in the OpenAPI attributes.

### Version 1.0.0

OpenAPI support for Azure Functions.
