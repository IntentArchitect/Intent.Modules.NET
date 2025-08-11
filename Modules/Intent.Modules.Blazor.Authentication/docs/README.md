# Intent.Blazor.Authentication

The **Intent.Blazor.Authentication** module provides a fully integrated authentication setup for Blazor applications generated with Intent Architect.  
It supports all three Blazor rendering modes — **InteractiveServer**, **InteractiveWebAssembly**, and **InteractiveAuto** — and can be configured to work with multiple authentication strategies:

- **ASP.NET Core Identity**
- **JWT Bearer Authentication**
- **OIDC Password Flow**

## Rendering Mode Support

The appropriate services, `AuthenticationStateProvider` implementations, and dependency injection (DI) registrations are generated according to the selected rendering mode to ensure a seamless authentication flow.

Each rendering mode uses a tailored implementation of the `AuthenticationStateProvider` to match its runtime environment.  
By default, authentication state is persisted using cookies in all modes, unless overridden.

## Supported Authentication Modes

You can configure this module to support:

- **ASP.NET Core Identity** – For individual accounts using a local store. This mode sets up an Entity Framework Core database to store user data and account information. It includes built-in functionality for user registration, login, password reset, and email confirmation. **Note:** This mode does not support attaching a token to a third-party API.
  
- **JWT (JSON Web Token)** – For stateless authentication using tokens. A `TokenEndpoint:Uri` must be configured in your app settings. The module handles typical user flows including login, registration, password reset, and forgot password — assuming your token provider supports these endpoints.
  
- **OIDC Password Flow** – For integration with third-party identity providers like IdentityServer, Auth0, or Azure AD B2C. This mode supports the **Resource Owner Password Credentials (ROPC)** grant flow, allowing users to authenticate using their username and password directly against the identity provider.

In all three modes, the authenticated `ClaimsPrincipal` is stored in either the `IdentityCookie` or a general `Cookie`, depending on the configuration.

## Third-Party API Authentication

Authentication for secure third-party APIs can be achieved using either **JWT** or **OIDC** modes.

In these modes, the access token (`auth_token`) retrieved from the configured token provider is automatically attached to any outgoing HTTP requests targeting `Secured` API endpoints.  
This ensures that all protected resources are accessed with the appropriate authorization headers without requiring manual token handling.
