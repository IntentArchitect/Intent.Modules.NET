# Intent.Blazor.Authentication — Context

The durable _why_ behind this module's design. Read before modifying it.

## What the module is responsible for

Generating the authentication wiring for a Blazor application across three **Authentication** modes (ASP.NET Core Identity, JWT bearer, OIDC password flow) and three **Render Modes** (InteractiveServer, InteractiveWebAssembly, InteractiveAuto). Both axes vary independently, which is the single biggest source of complexity here.

## Invariant: some templates are shared across authentication modes

This is the trap to know about before changing anything.

`CanRunTemplate` for these three templates gates on **render mode only** — they run for every authentication mode:

| Template                                              | Gate                                    |
| ----------------------------------------------------- | --------------------------------------- |
| `AccessTokenResponseTemplate`                         | `!IsInteractiveServer()` OR JWT OR OIDC |
| `PersistentAuthenticationStateProviderTemplate`       | `!IsInteractiveServer()`                |
| `UserInfoTemplate`                                    | none — always runs                      |
| `PersistingServerAuthenticationStateProviderTemplate` | `IsInteractiveWebAssembly()`            |
| `ServerAuthorizationMessageHandlerTemplate`           | none — always runs (see below)          |

So a change that looks mode-specific is not. **Branch on `this.GetAuthenticationType()` inside the template rather than editing it for one mode**, or you will silently break another. The auth-service templates (`OidcAuthServiceConcrete`, `JwtAuthServiceConcrete`, `AspNetCoreIdentityAuthServiceConcrete`) _are_ one-per-mode and can be edited freely.

The concrete instance of this trap, from the 2.0.2 OIDC fixes: adding `[JsonPropertyName("access_token")]` to `AccessTokenResponse` is **required** for OIDC (RFC 6749 §5.1 token responses are snake_case) and **breaks JWT** (its ASP.NET Core Identity backend returns camelCase, which `JsonSerializerDefaults.Web` already matches case-insensitively). Both correct answers, one shared template.

## Decision: OIDC ships no browser-side token refresh

JWT keeps its browser-side refresh — `refresh` is a real ASP.NET Core Identity endpoint and the refresh token is already in the browser, so refreshing there costs nothing extra.

OIDC deliberately has none. An OIDC provider expects a form-encoded `grant_type=refresh_token` at `connect/token`; doing that from the browser needs a `client_id` there, which means a **public client registration plus browser-origin CORS**. That forfeits precisely the security property that makes the server-side OIDC design better than a browser-side one. "Access token expired → back to the login page" is the accepted behaviour instead.

**If refresh is ever wanted for OIDC, build it as a server-side endpoint**: read `refresh_token` from the auth cookie, call the IdP with `client_id` + `client_secret`, re-issue the cookie, return the new access token. That is the only variant needing neither a secret nor a public client in the browser.

## Decision: `UserInfo` carries no refresh token, and no `ClientId`

`UserInfo` is handed to the WebAssembly client via `PersistAsJson`, which embeds it **unencrypted in the prerendered HTML** and then keeps it in WASM memory — readable by any XSS, and a refresh token outlives an access token by a long way. It therefore carries `RefreshToken`/`RefreshUrl` only in JWT mode, where the browser-side refresh actually consumes them.

`RefreshUrl` is also a smell worth not repeating: it is application configuration (`TokenEndpoint:Uri`), identical for every user and request, riding on a per-user session DTO.

**Do not add a `ClientId` property.** This was considered and rejected — it is app config, not user info. An OAuth `client_id` is public per RFC 6749 §2.2, so if something client-side needs it the channel is the generated `<Client>/wwwroot/appsettings.json`, which the client `Program.cs` already loads via `LoadAppSettings` (the same route as `Urls:*`).

## Invariant: account routes come from the model, not from constants

The **"Security Type"** stereotype's page-tagging script creates the Login/Register/etc. `Component`/`Page` elements, stamps each with a `blazor-auth-page-id` metadata key, and sets the `Page` stereotype's `Route` (`/Account/Login` for the login page in all three modes). The user can then edit that route in the User Interface designer.

Redirects are therefore resolved through `SecurityHelperExtensions.GetLoginRoute()`, which finds the element tagged `identity-login` / `jwt-login` / `oidc-login` and reads its `Page.Route`, falling back to `/Account/Login`. **Do not hardcode account routes in templates** — that is what drifted into `auth/login` and `/account/login` before 2.0.2.

Note `blazor-auth-page-id` is matched against an explicit set of three ids, not an `EndsWith("-login")` test, because `external-login` is a different page that would also match.

`/auth/logout` is _not_ emitted by this module — the page-tagging script has no logout route. The module's own logout target is `Account/Logout`, the endpoint mapped by `IdentityComponentsEndpointRouteBuilderExtensions`.

## Decision: the server never uses WebAssembly authentication APIs (2026-08-30)

The single most confusing thing about this module used to be that **WebAssembly auth APIs were being registered on the server**. Three consequences, all now removed:

1. `HttpClientConfiguration` (from `Intent.Blazor.HttpClients`) is generated into the shared `.Client` project, and in two-project render modes **both hosts call `AddHttpClients`** — the server reaches it via `AddClientServices`. So the `AuthorizationMessageHandler` it attaches by default landed in the server container. That handler caches the token and the built `Authorization` header in unsynchronised instance fields and refreshes only near expiry, so on the server **one cached token was shared across every request and every user** for the handler chain's lifetime (2 min default).
2. `ServerAddAuthentication` registered `services.AddApiAuthorization()` for every non-Interactive-Server mode. That is a WebAssembly API; it registers `IAccessTokenProvider` as a **cast** of the resolved `AuthenticationStateProvider`. In Interactive Auto that provider is `PersistingRevalidatingAuthenticationStateProvider`, which does not implement it — `InvalidCastException` on startup.
3. `PersistingServerAuthenticationStateProvider` implemented `IAccessTokenProvider` only to satisfy that cast. Its `RequestAccessToken()` called `GetAuthenticationStateAsync()`, which throws _"Do not call GetAuthenticationStateAsync outside of the DI scope for a Razor component"_ when the provider was built in `IHttpClientFactory`'s own scope — the exception the whole fix started from — and it reported every token as `Expires = DateTimeOffset.MaxValue`.

**The rule now: `AddApiAuthorization()`, `AuthorizationMessageHandler` and `IAccessTokenProvider` are browser-only.** The server forwards its token through `ServerAuthorizationMessageHandler`, which reads the `access_token` claim off `IHttpContextAccessor.HttpContext` **per call, caching nothing**, and only attaches it when the request URI matches one of the authorized URLs it was constructed with. The no-caching property is the entire point of the type — do not "optimise" it.

## Decision: the handler is attached per host, not baked into the shared class (2026-08-30)

The obvious fix — have `AddHttpClients` attach the right handler — is impossible: it is _one_ generated method in _one_ shared project, called by both hosts. So `Intent.Blazor.HttpClients` keeps `AuthorizationMessageHandler` as its default (it has no dependency on this module and must keep working without one) and additionally emits `AddApiAuthorizationHandler(configuration, handlerFactory)`. This module removes the default and calls that method once per host:

- `ServerAddAuthentication` → `new ServerAuthorizationMessageHandler(IHttpContextAccessor, urls)`
- `ClientAddAuthentication` → `AuthorizationMessageHandler.ConfigureHandler(authorizedUrls: urls)`, which is byte-for-byte what `AddHttpClients` used to build, so browser behaviour is unchanged.

Both sides are found **structurally, by metadata** — `authorization-handler` on the statements to strip, `api-authorization-handler` on the `CSharpFile` to test before wiring. The previous implementation matched the emitted text `"AddHttpMessageHandler"`, which broke as soon as anything else added a handler to the same chain. See `Intent.Modules.Blazor.HttpClients/CONTEXT.md` for the contract and why the file-level key must be read rather than the generated method (`AddClass`'s callback does not run until build time, so `Classes[].Methods` is empty during registration — an ordering trap that cost a full debugging cycle).

There is deliberately **no `<dependency>` on `Intent.Blazor.HttpClients`** — an application can have authentication and no service proxies. Both hooks null-guard the template instance and skip silently when the metadata is absent, which is also what happens against an older `Intent.Blazor.HttpClients`.

`ServerAuthorizationMessageHandlerTemplate` was roled `Blazor.Client` in `Components/Account/Shared`, which would have put a class needing `IHttpContextAccessor` into the WebAssembly project. It is now `Startup,Blazor` in `Common`, and lives under `Templates/Server/`. **Its template id changed** to `Intent.Blazor.Authentication.Templates.Server.ServerAuthorizationMessageHandlerTemplate`.

## Config keys this module owns

Emitted as placeholders via `ApplyAppSetting` in each auth service template's `BeforeTemplateExecution`, consumed by `ServerAddAuthentication`:

- `TokenEndpoint:Uri` — base address of the `"jwtClient"` / `"oidcClient"` `HttpClient`. **The OIDC value needs a trailing slash** because `OidcAuthService` posts to the _relative_ `connect/token`; without it a sub-path such as `/identity/` is discarded. JWT posts to `/login` with a leading slash, so its value has no trailing slash — the difference is deliberate, not an oversight.
- `Authentication:OIDC:{ClientId,ClientSecret,DefaultScopes}` — bound to `OidcAuthenticationOptions`.

## Generated-code gotchas found the hard way

- **The `CSharpFile` builder splits zero-argument invocations.** `AddStatement("x = Foo();")` and `AddAssignmentStatement("x", new CSharpStatement("Foo();"))` both emit `Foo(\n);`. Statements with arguments are fine. Emit such calls inside a raw `AddStatements(@"…").ConvertToStatements()` block.
- **Do not mark a generated method `Async()` unless the emitted body awaits something.** Gating a branch out of a method can leave it awaitless, which emits a CS1998 warning into every generated application. `PersistentAuthenticationStateProvider.RequestAccessToken` is `async` for JWT and wraps its returns in `ValueTask.FromResult` for the other modes for exactly this reason.
- `Templates/TemplateExtensions.cs` is `Mode.Fully` generated — put hand-written helpers in `Api/SecurityHelperExtensions.cs` instead.
- Bumping this module's version in a JWT consumer adds a `Components/Account/Shared/_Imports.razor` entry to its `.managed-files.xml` without emitting the file. Pre-existing, reproduces with unmodified templates, unrelated to template changes — noted so it is not re-diagnosed as a regression.

## Verifying a change to this module

`dotnet build` proves nothing about generated output. The consumer test applications under `Tests/` are the real check — eleven of them install this module:

`Blazor.Interactive{Server,Auto,WebAssembly}.{AspNetCoreIdentity,Jwt,Oidc}`, `BlazorNoMudBlazor`, `BlazorServerTests`.

Because the two axes are independent, **an OIDC-motivated change must be regenerated and rebuilt against the JWT and Identity apps too**, not just the OIDC ones.

## Invariant: OIDC has a LOCAL login page — `!IsOidc()` is never the account-UI gate (2026-08-31)

This module's OIDC mode is the **resource-owner password credentials flow**, not an IdP redirect. `OidcAuthService` posts `grant_type=password` to `connect/token`, and the "Security Type" page-tagging script creates an `oidc-login` page which `AuthPageDefaultContentFactoryExtension` maps to `LoginPageContent` — the same hand-rolled `login-input-*` form Identity and JWT get.

So an OIDC application has account UI and needs everything that styles it. The comment _"OIDC redirects to an external IdP — no local account UI"_ had been copied into four gates (`AccountThemeStaticContentTemplateRegistration`, `MudBlazorAccountThemeStaticContentTemplateRegistration`, and both account-CSS wireups) and was wrong in all four: OIDC applications got the markup and no stylesheet. **The correct gate for anything account-UI is `!IsNone()`** — every other mode has a local login. If OIDC ever gains a true redirect-to-IdP variant, that is a new mode, not a reinterpretation of this one.

## Decision: the account stylesheet link anchors on `{Project}.styles.css` (2026-08-31)

`WireupAccountCssExtension` inserts `<link href="ux-account.css">` **above** the `{Project}.styles.css` link. Rejected alternatives and why:

- **`app.css` (the previous anchor)** — `Intent.Blazor` now emits that link conditionally (`TemplateHelper.ShipsAppCss`), so it is absent from most applications and the wireup would silently no-op. It was already the wrong anchor for a second reason: nothing guarantees it exists.
- **Below `ux-mudblazor.css`** — that element is itself inserted by another module's `AfterBuild` callback at the same priority (100), so whether it exists when this callback runs depends on registration order. Anchoring to it is a race.
- **Below `ux-components.css`** (what `Intent.Blazor.Components.MudBlazor` uses) — same race, with the worse failure: if MudBlazor's callback runs _after_ this one, `ux-mudblazor.css` lands _below_ `ux-account.css` and the cascade inverts.

`{Project}.styles.css` is emitted unconditionally by `AppRazorTemplate`, sits last in the head, and no module repositions it. Inserting above it gives both orderings the sheet needs: **after** `ux-mudblazor.css` (sections 2–4 of `ux-account.css` beat `.ux-gradient-primary` by cascade order, not `!important` — link it earlier and the full-bleed frosted-banner bug returns) and **before** the scoped bundle, so a page's own `.razor.css` still wins.

The two variants (`content/Theme` and `content/ThemeMudBlazor`) share one href, so the wireup is deliberately MudBlazor-agnostic and there is one extension, not two. `WireupMudBlazorAccountCss` was deleted; it differed only by an inverted MudBlazor check and emitted the identical link.

## Known latent issue: the scoped `.razor.css` seed for account pages never fires (2026-08-31)

`AuthPageDefaultContentFactoryExtension` looks up a `RazorComponentStyleTemplate` per auth page and seeds it from `*PageContent.BuildStyleContent`. **That branch is unreachable in every configuration.**

`RazorComponentStyleTemplateRegistration.GetModels` (in `Intent.Blazor`) filters `.Where(model => !model.HasRenderOnServer())`, and the "Security Type" stereotype's page-tagging script stamps `Render On Server` on **every** Account page and shared component unconditionally, in all three Authentication modes — deliberately, because several of them reference Server-only generated helpers (e.g. `IdentityRedirectManagerTemplate`) and a Client→Server reference would close a cycle. So no style template instance exists for any account page, `styleTemplate` is always null, and the loop `continue`s before it can seed anything.

Confirmed empirically: no `Components/Account/**/*.razor.css` appears in any test application's `.application.managed-files.xml`. **The `.razor.css` files sitting in the test applications are untouched leftovers from the removed per-page template pairs** — still tracked in git, no longer regenerated by anything. Deleting one does not bring it back.

Consequences to know before "fixing" this:

- `BuildStyleContent` on ~20 `*PageContent` classes is dead code today. It is kept because it is the intended shape, and because the section below explains why the global sheet must carry the same rules regardless.
- The global `ux-account.css` is therefore the **only** source of account-page styling in practice, which makes the overlap described below load-bearing rather than merely defensive.
- A `File.Exists` guard was added around the seed in 2.0.2-pre.7 so that it is once-off _if_ it ever becomes reachable — matching the two Home-page extensions. It changes nothing observable today.

Making it reachable is a real design change, not a one-line fix: it means either registering style templates for server-rendered components (deciding which project the `.razor.css` lands in) or accepting the global sheet as the single source and deleting the dead builders.

## Decision: section 7 of `ux-account.css` deliberately overlaps the scoped page CSS (2026-08-31)

`LoginPageContent.BuildStyleContent` (and ~20 sibling `*PageContent` classes) emit the identity input/checkbox styling into each page's scoped `.razor.css` using `::deep`, per the direction in `Intent.Modules.Blazor/docs/css-architecture.md`. Section 7 of `ux-account.css` says the same thing globally. **Keep both.** Default content is first-generation-only, so any application generated before the scoped CSS existed has the markup and no page-level styling — the global sheet is its only source. Sections 1–5 are not duplicated anywhere and cannot be: they target MudBlazor's own component-rendered markup (`.mud-paper.ux-gradient-primary`, `.mud-main-content`), which no page-scoped sheet can reach.
