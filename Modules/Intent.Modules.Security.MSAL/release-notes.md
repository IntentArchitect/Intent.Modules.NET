### Version 4.2.2

- Improvement: Updated module NuGet packages infrastructure.
- Improvement: Internal updates to module project.
- Fixed: Changed `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();` to `JwtSecurityTokenHandler.DefaultMapInboundClaims = false;` to turn off JWT claim keys from being mapped to XML SOAP scheme URLs.

### Version 4.2.1

- Improvement: Updated NuGet packages to latest stables.
- Improvement: Added support for different UserId Types (guid, long, int).

### Version 4.2.0

- Improvement: Upgraded module to support new 4.1 SDK features.
 
### Version 4.1.7

- Improvement: Updated to be compatible with .NET 8.

### Version 4.1.6

- Improvement: Added `SignUpSignInPolicyId` configuration option for specifying the PolicyId / User Flow for B2C.

### Version 4.1.4

- Improvement: Nuget package version of `Microsoft.AspNetCore.Authentication.JwtBearer` is now bumped to the latest version based on the configured .NET version to eliminate the known security vulnerabilities.

### Version 4.1.3

- Improvement: Internal dependency version update to keep compatibility with other modules.

### Version 4.1.2

- Improvement: dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.1

- Improvement: supported client version to [3.4.0-pre.0, 5.0.0).

### Version 4.1.0

- Improvement: Incorporated changes based on changes made in `Intent.Application.Identity`.

### Version 3.3.1

- Improvement: Null check added to HttpContext in the event that this gets used outside of an HttpContext scope.

### Version 3.3.0

- New: Authentication library for Microsoft's Azure Active Directory.
