# Intent.AspNetCore.Swashbuckle.Security

This module add security related concerns to the `Intent.AspNetCore.Swashbuckle` module.

Swashbuckle.AspNetCore is a popular open-source library for ASP.NET Core applications that simplifies the process of adding Swagger support. Swagger is a set of tools and specifications for describing and visualizing RESTful APIs. Swashbuckle.AspNetCore integrates Swagger into ASP.NET Core projects, enabling developers to automatically generate interactive API documentation, known as Swagger UI, and JSON-formatted API specifications, known as Swagger/OpenAPI specification. With Swashbuckle.AspNetCore, developers can easily expose the details of their Web API, including available endpoints, request and response models, and supported HTTP methods. This makes it easier for developers and third-party consumers to understand, test, and interact with the API, promoting better communication and collaboration between frontend and backend teams.

This module specifically deals with:

- Adding various security schemes to Swashbuckle.
- Adding relevant SwaggerUI security related configuration.

## Module Settings

This module introduces the `Swagger Settings -> Authentication`, this setting has the following options:

- **Bearer**, this setting configures the Swagger UI so you can authenticate using a bearer token.
- **OAuth 2.0 - Implicit**, this setting configures the Swagger UI so you can authenticate using OAuth 2.0 implicit flows.

### OAuth 2.0 - Implicit

For this setting you will need to add the relevant configuration into you `app.settings`:

```JSON
"Swashbuckle": {
  "Security": {
    "OAuth2": {
      "Implicit": {
        "AuthorizationUrl": "[AuthorizationUrl]",
        "TokenUrl": "[TokenUrl]]",
        "Scope": {
          "[Scope Description]": "[Scope URL]"
        },
        "ClientId": "[ClientId]"
        //"ClientSecret": "2baa1cc5-1006-40ee-8db1-1abcc54ff08b" OPTIONAL
      }
    }
  }
}
```

For more information on `Swashbuckle.AspNetCore`, check out their [GitHub](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
