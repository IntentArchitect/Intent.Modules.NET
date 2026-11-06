# Intent.AspNetCore.Swashbuckle

This module installs and configures `Swashbuckle.AspNetCore`.

Swashbuckle.AspNetCore is a popular open-source library for ASP.NET Core applications that simplifies the process of adding Swagger support. Swagger is a set of tools and specifications for describing and visualizing RESTful APIs. Swashbuckle.AspNetCore integrates Swagger into ASP.NET Core projects, enabling developers to automatically generate interactive API documentation, known as Swagger UI, and JSON-formatted API specifications, known as Swagger/OpenAPI specification. With Swashbuckle.AspNetCore, developers can easily expose the details of their Web API, including available endpoints, request and response models, and supported HTTP methods. This makes it easier for developers and third-party consumers to understand, test, and interact with the API, promoting better communication and collaboration between frontend and backend teams.

This module specifically deals with:

- Dependency Injection wiring
- Swashbuckle - Swagger and Swagger UI Configuration
- Automatic removal of duplicate route parameters from request body schemas
- HTTP `QUERY` endpoints in the Swagger document and Swagger UI
- XML documentation comments integration

For more information on `Swashbuckle.AspNetCore`, check out their [GitHub](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).

## Features

### Automatic Route Parameter Handling

This module automatically includes the `HideRouteParametersFromBodyOperationFilter` operation filter, which removes properties from request body schemas when they are already defined as route parameters. This prevents duplicate documentation of parameters that are supplied via the URL, ensuring cleaner and more accurate API documentation.

### HTTP QUERY endpoints

The HTTP `QUERY` method can only be described from OpenAPI 3.2, which Swashbuckle does not produce on its own. When the application targets .NET 10 and at least one operation uses the `QUERY` verb, the module generates:

- `HttpQueryDocumentFilter`, which adds the `QUERY` operations to the Swagger document.
- `OpenApi32DocumentSerializer`, which serves the document as OpenAPI 3.2.
- A Swagger UI index page that loads swagger-ui 5.34.1 from unpkg, since the swagger-ui bundled with Swashbuckle cannot send `QUERY` requests. The page needs internet access to load.

On earlier .NET versions, or with no `QUERY` operations, none of this is generated and `QUERY` endpoints are left out of the Swagger document.

## Settings

### Mark non-nullable fields as required

Default setting: _enabled_

Controls whether or not non-nullable properties cause `"nullable": true` to be added to them in their generated schema.

### Use simple schema identifiers

Default setting: _disabled_

By default, schema identifiers have been configured to be the fully qualified type name so as to avoid conflicts with otherwise identically named types. When this option is enabled "simple" identifiers without a namespace are generated instead.

**Schema Identifier Format:**
- **Nested types:** Separated by `.` (e.g., `ParentType.NestedType`)
- **Generic types with simple identifiers:** Use `Of` and `And` to separate generic types and parameters (e.g., `ListOfString`)
- **Generic types with full-namespace identifiers:** Use `_Of_` and `_And_` to separate generic types and parameters (e.g., `System.Collections.Generic.List_Of_System.String`)

## Inclusion of XML documentation comments

To enable inclusion of XML documentation comments, ensure that in the Visual Studio designer, `Generate Documentation File` is set to `true` for the `<ApplicationName>.Api`, `<ApplicationName>.Application`, and optionally the `<ApplicationName>.Domain` projects. It is also recommended on these projects to append `;1591` to the `Suppress Warnings` property so as to prevent generation of [Missing XML comment for publicly visible type or member 'Type_or_Member'](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-messages/cs1591) warnings.

**Note:** Comments from the Domain Model are automatically included in the generated Swagger documentation when XML comments are enabled for the Domain project.

## Related Modules

### Intent.Intent.AspNetCore.Swashbuckle.Security

This module introduces Authentication related patterns to this module.
