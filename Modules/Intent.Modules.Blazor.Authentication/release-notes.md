### Version 2.0.3

- Improvement: `PersistingServerAuthenticationStateProvider` no longer implements `IAccessTokenProvider` and no longer throws from its persist callback when no authentication state was set. `ServerAuthorizationMessageHandler` has moved to the server project (its template id is now `Intent.Blazor.Authentication.Templates.Server.ServerAuthorizationMessageHandlerTemplate`).
- Improvement: The generated server `appsettings.json` now includes placeholders for the `TokenEndpoint:Uri` and `Authentication:OIDC:*` keys that OIDC applications read, so a freshly generated application no longer fails at runtime with no indication of which keys exist. See the module documentation for what each key does and why `TokenEndpoint:Uri` needs a trailing slash.
- Improvement: Account redirects across all templates now resolve the login route from the modelled login page's `Page` stereotype instead of hardcoding it, so editing the page's route in the designer is honoured everywhere.
- Improvement: OIDC applications no longer send a refresh token to the browser. `UserInfo` is embedded unencrypted in the prerendered HTML, and the browser-side refresh that consumed it could not work against an OIDC provider. JWT applications are unaffected and keep their refresh support.
- Improvement: `PersistentAuthenticationStateProvider.RequestAccessToken` now returns its `RequiresRedirect` result rather than also navigating inline, per the `IAccessTokenProvider` contract. The inline forced navigation tore the page out from under in-flight requests.
- Fixed: OIDC login could never succeed. The token request was sent as JSON instead of `application/x-www-form-urlencoded` (required by RFC 6749 §4.3), and its leading `/` discarded any sub-path in the configured `TokenEndpoint:Uri`.
- Fixed: The OIDC token response was deserialized with camelCase matching, so every snake_case property (`access_token`, `expires_in`, …) bound as `null`. `AccessTokenResponse` now carries `[JsonPropertyName]` attributes in OIDC mode. JWT mode is left on camelCase, which is what its ASP.NET Core Identity backend returns.
- Fixed: OIDC login threw when the identity provider issued no `refresh_token` — which is optional per RFC 6749 §4.3.3 and the norm without the `offline_access` scope. An empty token response or one with no access token now fails with a clear message instead of an `ArgumentNullException`.
- Fixed: The `ClaimsIdentity` created on OIDC login had no authentication type, so the principal reported `IsAuthenticated == false` and `UserInfo` was never persisted to the WebAssembly client even though the cookie had been issued.
- Fixed: OIDC sign-in re-read `IHttpContextAccessor.HttpContext` instead of the already-null-checked local, emitting a CS8602 possible-null-dereference warning in every generated application. Sign-in and sign-out now also pass the cookie authentication scheme explicitly rather than depending on `DefaultScheme` in a separate, user-editable generated file.
- Fixed: `PersistingServerAuthenticationStateProvider` redirected to `auth/login`, which matched no generated page.
- Fixed: JWT token refresh threw when the token response omitted `expires_in`, silently reporting a successful refresh as a failure, and never raised `NotifyAuthenticationStateChanged`, so consumers reading `access_token` off `AuthenticationState` kept seeing the stale token for the rest of the session.
- Fixed: The account stylesheet (`ux-account.css`) was never linked into `App.razor`, and was not shipped at all for Single Sign-On (OpenID Connect) applications despite those having their own local login page, leaving every account page with unstyled native inputs; it is now shipped and linked for every Authentication mode with a local account UI.
- Fixed: Authenticated API calls made while the server was rendering used the WebAssembly `AuthorizationMessageHandler`, because the generated `HttpClientConfiguration` is shared by both hosts. It threw `InvalidOperationException` on every such call during prerender, and would otherwise have cached one user's token across every request the server made. The server now uses a per-request handler that reads the token from the current `HttpContext` and only attaches it to the APIs it was issued for.
- Fixed: Interactive Auto applications threw `InvalidCastException` on startup when a service proxy required authorization. The server no longer registers `AddApiAuthorization()`, which is a WebAssembly API and cast the resolved `AuthenticationStateProvider` to `IAccessTokenProvider`.

### Version 2.0.2

- Improvement: Added Authentication selection type to the package, and model out the relevant pages bsed on selection
- Improvement: A number of styling updates to bootstrap pages to align more with Mudblazor styling.
- Improvement: Default stylesheets not overwritten by the software factory.
- Improvement: Added context menu to set the authentication type

### Version 2.0.1

- Improvement: Reduced unnecessary warnings by seeding `[SupplyParameterFromForm]` input models in `OnInitialized (Input ??= new())` instead of a property initializer which should rather be set to `default!`

### Version 2.0.0

- Improvement: Component code-behind and styles are now emitted as separate `razor.cs` and `razor.css` files, allowing improved separation of concerns of C# logic and scoped CSS.

> **NOTE**
>
> This is a breaking change for some components where you previously customized the generated code.
>
> **For `razor.cs`:** if your `@code { }` block was unchanged from what Intent generated, delete the block — Intent will now manage the code-behind in the separate `razor.cs` file. If you had customizations, migrate them into the newly generated `razor.cs` and appropriately use `[IntentIgnore]` to continue managing your own code
>
> **For `razor.css`:** there are no build failures. If your `<style>` block was unchanged from what Intent generated, you can safely delete it. If you had customizations, copy the content into the `*.razor.css` file. Intent will not interfere with your modifications to this file 

- Improvement: Default UI theming
- Fixed: Navbar icons did not work due to coupling with main Blazor modules running `Interactive` modes which is not supported by Blazor's ASP.NET Identity 

### Version 1.1.1

- Improvement: Updated NuGet package versions.

### Version 1.1.0

- Improvement: Updated references from the conversion of Blazor AI to use skills for Page implementation, and align with new AI implementation.

### Version 1.0.7

- Improvement: Updated NuGet package versions.

### Version 1.0.6

- Improvement: Updated NuGet package versions.
- Improvement: Fixed AntiForgery exception when re-logging in.
- Improvement: Refresh token support for WASM / JWT authentication.

### Version 1.0.5

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.4

- Improvement: Updated NuGet package versions.

### Version 1.0.3

- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Improvement: Updated NuGet package versions.
- Fixed: Syntax generation fix.

### Version 1.0.1

- Fixed: De-coupled an optional template.

### Version 1.0.0

- Fixed: No default authentication type was set.
- New Feature: Blazor Authentication Module.
