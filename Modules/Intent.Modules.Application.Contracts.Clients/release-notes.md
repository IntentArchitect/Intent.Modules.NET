### Version 4.0.2

- Removed dependency on Domain module
- Migrated remaining templates to use CSharpFileBuilder.

### Version 4.0.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.0

- Updated to work with `Intent.Metadata.WebApi` version 4+.

### Version 3.3.11

- Enums will now generate comments captured in designers.

### Version 3.3.10

- Generated DTOs now respect the `Serialization Settings` stereotype.
- Used Enums will now also be generated.

### Version 3.3.8

- Fixed: Operation name suffix "Async" added.

### Version 3.3.7

- New: HttpClientRequestException added which contains its own response content in error scenarios.
- Update: Leveraging new internal interface for obtaining service proxy information.

### Version 3.3.6

- Update: Generates code from referenced services modeled in `Service Proxies` designer in the form of a Service Interface (that is fully `async`/`await` enabled) and DTO classes (including Commands and Queries).