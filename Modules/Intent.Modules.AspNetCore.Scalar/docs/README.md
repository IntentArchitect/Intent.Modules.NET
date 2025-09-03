# Intent.AspNetCore.Scalar

This module installs and configures `Microsoft.AspNetCore.OpenApi` and integrates with [Scalar](https://scalar.com/) for API documentation.

Scalar is a modern, fast, and user-friendly OpenAPI documentation viewer. It works with the new `Microsoft.AspNetCore.OpenApi` features introduced in .NET 9+, allowing you to generate OpenAPI documents and visualize them in an interactive UI. Scalar replaces older Swagger UI solutions such as Swashbuckle, providing a clean developer experience, native integration with ASP.NET Core, and support for OpenAPI 3.1. 

With Scalar, developers can easily expose the details of their Web API, including available endpoints, request and response models, and supported HTTP methods. This makes it easier for developers and third-party consumers to understand, test, and interact with the API, promoting better communication and collaboration between frontend and backend teams.

This module specifically deals with:

- Dependency Injection wiring
- OpenAPI document generation using `Microsoft.AspNetCore.OpenApi`
- Scalar UI configuration

For more information on `Scalar`, check out their [Website](https://scalar.com/) or [GitHub](https://github.com/scalar/scalar).

## Settings

### Use fully qualified schema identifiers

By default, schema identifiers have been configured to be the fully qualified type name so as to avoid conflicts with otherwise identically named types. When this option is enabled "simple" identifiers without a namespace are generated instead.

### Authentication

This module supports authentication in your API documentation by adding security schemes for both **Bearer tokens** and **OIDC Implicit Flow**. This enables the Scalar UI to provide an "Authorize" button where developers can authenticate and test endpoints against a secured API.

#### Bearer Token Authentication

Bearer authentication is automatically configured with the following security scheme:

- **Scheme:** `bearer`
- **Format:** `JWT`

This allows developers to paste a raw JWT token into the Scalar "Authorize" dialog and have it automatically included in all requests.

#### OIDC Implicit Flow Authentication

This module also supports OIDC (OpenID Connect) using the Implicit Flow. When enabled, the Scalar UI will provide a login button that redirects the user to your identity provider's `authorize` endpoint and automatically obtains an access token for use in API calls.

To enable OIDC implicit flow, you must provide the configuration values in `appsettings.json`:

```jsonc
{
  "OpenApi": {
    "Oidc": {
      "AuthorizationUrl": "https://auth.myapp.com/connect/authorize",
      "Scopes": [
        "openid",
        "profile",
        "api"
      ]
    }
  }
}
