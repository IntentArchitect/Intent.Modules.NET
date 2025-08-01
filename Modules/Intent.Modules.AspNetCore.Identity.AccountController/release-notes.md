### Version 4.1.8

- Improvement: Updated NuGet package versions.

### Version 4.1.7

- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 4.1.6

- Improvement: Updated to the lastes Intent.AspNetCore.Identity. ApplicationIdentityUser now inherits from the base type and can be modified within the domain designer.

### Version 4.1.5

- Improvement: Updated NuGet package versions.

### Version 4.1.4

- Improvement: Replaced IdentityModel with Duende.IdentityModel
- Improvement: Updated NuGet package versions.

### Version 4.1.3

- Improvement: Updated NuGet package versions.

### Version 4.1.2

- Improvement: Updated NuGet package versions.

### Version 4.1.1

- Improvement: Updated NuGet package versions.

### Version 4.1.0

> ⚠️ **NOTE**
> 
> Claims related changes has been introduced to improve the experience using the `CurrentUserService`.

- Fixed: `GetClaims` no longer provides a claim named `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier` but instead returns the `sub` claim that represents the User's ID.
- Fixed: `TokenService` will no longer automatically set the `sub` claim to be the `username` by default.
- Fixed: The `TokenService` has also been updated to accept `IList` of claims as opposed to `IEnumerable` of claims.

### Version 4.0.0

> ⚠️ **NOTE**
>
> The `appSettings.json` field names and value types for token expiries have changed and will need to be updated in your settings.
>
> `AuthTokenExpiryMinutes` has changed to `AuthTokenExpiryTimeSpan` and now has a default value of `02:00:00` (2 hours).
>
> `RefreshTokenExpiryMinutes` has changed to `RefreshTokenExpiryTimeSpan` and now has a default value of `3.00:00:00` (3 days).

- Improvement: Token Expiry settings now take in a TimeSpan value as a string. For information on the structure of TimeSpan strings please refer to [Microsoft's Documentation](https://learn.microsoft.com/dotnet/api/system.timespan.tostring#system-timespan-tostring).
- Fixed: The `GenerateRefreshToken` method on `TokenService` had `DateTime.UtcNow.AddDays(_configuration.GetSection("JwtToken:RefreshTokenExpiryMinutes")` (note that it was doing a `.AddDays` on a "Minutes" value).

### Version 3.1.5

- Fixed: Missed `Intent.JWT` module as dependency.

### Version 3.1.4

- Improvement: Updated module NuGet packages infrastructure.

### Version 3.1.3

- Fixed: updated module dependencies.

### Version 3.1.2

- Improvement: Updated NuGet packages to latest stables.

### Version 3.1.1

- Fixed: `ForgotPassword` and `ResetPassword` endpoints did not have `[AllowAnonymous]` set.

### Version 3.1.0

- Improvement: Added `POST /manage/info` endpoint for updating info including email address and/or password.
- Improvement: Added `GET /manage/info` endpoint for retrieving user info (only email address at this time).
- Improvement: Added `POST /forgotPassword` and `POST /resetPassword` endpoints.
- Fixed: `Refresh` endpoint no longer requires a valid access token and instead decrypts the refresh token to determine the user account of it.

### Version 3.0.1

- Improvement: Updated to be compatible with .NET 8.
- Fixed: Adding an Identity User will now receive the Refresh Token support in code.

### Version 3.0.0

- Improvement: The `RefreshToken` token endpoint has had its name changed to `Refresh` and it now uses a `POST` verb in line with industry best practice of not including sensitive data in request URLs.
- Improvement: Inline with [Microsoft's identity management API introduced with .NET 8](https://devblogs.microsoft.com/dotnet/whats-new-with-identity-in-dotnet-8/), the access token is is not supplied in the body or query string, and the endpoint is instead decorated with `[Authorized]` meaning that as usual for secured endpoints, the access token will now need to supplied in the header.
- Fixed: When using the refresh token endpoint, the returned access token now has its claims updated with the latest for the user. This is inline with the behaviour of Microsoft's own refresh token endpoint as part of their [identity management API introduced with .NET 8](https://devblogs.microsoft.com/dotnet/whats-new-with-identity-in-dotnet-8/).

### Version 2.1.3

- Fixed: Login not working with Postgres, issue related to non UTC DateTime.

### Version 2.0.2

- Update: Nuget package version of `Microsoft.AspNetCore.Authentication.JwtBearer` is now bumped to the latest version based on the configured .NET version to eliminate the known security vulnerabilities.

### Version 2.0.1

- Fixed: Fixed a issue around "role" claim not being configured.

### Version 2.0.0

- Added Refresh Token functionality.
- `AccountController` class refactored to have `TokenService` separately.

### Version 1.1.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.1.1

- Update: Role Claims are now introduced.

### Version 1.1.0

- Update: Updates applied due to changes made in `Intent.Security.JWT`.

### Version 1.0.0

- New: Lightweight Account Controller to handle Authentication requests and returning JWT tokens.
