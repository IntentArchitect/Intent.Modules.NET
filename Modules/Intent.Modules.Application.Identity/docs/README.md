# Intent.Application.Identity

## What This Module Does
Generates application-level identity and security contracts + services: current user access, authorization attribute, result modelling, and exceptions for forbidden access. It centralizes user context retrieval and guards application service operations with role/permission checks.

## Generated Artifacts
- `ICurrentUser` interface (async accessors for Id, Name, Roles/Claims).
- `ICurrentUserService` + implementation (distribution/services layer) bridging framework identity principal to application abstraction.
- `AuthorizeAttribute` (contracts layer) for annotating application services/operations.
- `ForbiddenAccessException` standardized exception type for authorization failures.
- `ResultModel` (success/fail payload pattern).
- Security configuration class (`ApplicationSecurityConfiguration`) to compose DI registration and settings.

## Settings
Group: Identity Settings
- UserId Type (guid/int/long/string): Adjusts generated interface property and conversion logic.
- Keep Sync Accessors: Controls inclusion of synchronous helpers alongside async variants.

## Usage Pattern
1. Inject `ICurrentUserService` or `ICurrentUser` in application services / behaviours.
2. Apply `[Authorize(...)]` attribute to service operation methods or classes.
3. Throw `ForbiddenAccessException` in manual policy checks; upstream behaviours translate to proper response codes (with MediatR behaviours / presentation layer integration).
4. Use `ResultModel` for operations requiring success + error semantics beyond exceptions.

## Interoperability
Detects optional security provider modules (JWT, MSAL) and MediatR behaviours; ensures they are installed with compatible versions for integration (e.g., pipeline authorization behaviour leveraging `ICurrentUser`).

## Customization Points
- Extend `ICurrentUser` with additional claims (tenant, locale) via partial interface + service merge.
- Enhance `ApplicationSecurityConfiguration` to register additional policies or claim mappings.
- Wrap `ResultModel` with generic payload types (e.g. `ResultModel<T>`) if needed; follow merge mode.

## When To Use
- Any application requiring standardized identity abstraction independent of hosting (ASP.NET Core, functions).
- Testing contexts where injecting a mock current user is desirable.

## When Not To Use
- Systems with external identity fully resolved at edge and no need for application-level access beyond simple `UserId` propagation.

## Related Modules
- `Intent.Application.MediatR.Behaviours` (AuthorizationBehaviour integration)
- `Intent.Security.JWT`, `Intent.Security.MSAL` (providers for actual authentication & token resolution)

