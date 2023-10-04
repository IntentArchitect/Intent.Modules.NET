# Intent.AspNetCore.Swashbuckle

This module installs and configures `Swashbuckle.AspNetCore`.

Swashbuckle.AspNetCore is a popular open-source library for ASP.NET Core applications that simplifies the process of adding Swagger support. Swagger is a set of tools and specifications for describing and visualizing RESTful APIs. Swashbuckle.AspNetCore integrates Swagger into ASP.NET Core projects, enabling developers to automatically generate interactive API documentation, known as Swagger UI, and JSON-formatted API specifications, known as Swagger/OpenAPI specification. With Swashbuckle.AspNetCore, developers can easily expose the details of their Web API, including available endpoints, request and response models, and supported HTTP methods. This makes it easier for developers and third-party consumers to understand, test, and interact with the API, promoting better communication and collaboration between frontend and backend teams.

This modules specifically deals with

- Dependency Injection wiring
- Swashbuckle - Swagger and Swagger UI Configuration

For more information on `Swashbuckle.AspNetCore`, check out their [GitHub](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).

## XML comments

To enable XML comments, ensure that in the Visual Studio designer, `Generate Documentation File` is set to `true` for both the `<ApplicationName>.Api` and `<ApplicationName>.Application` projects. It is also recommended on these projects to append `;1591` to the `Suppress Warnings` property so as to prevent generation of [Missing XML comment for publicly visible type or member 'Type_or_Member'](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-messages/cs1591) warnings.

## Related Modules

### Intent.Intent.AspNetCore.Swashbuckle.Security

This modules introduces Authentication related patterns to this module.